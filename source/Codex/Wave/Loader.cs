using ChaosFramework.IO;
using System;
using System.IO;
using System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Wave
{
    using Meta;

    internal static class Loader
    {
        public delegate void LoadChunk(WaveData data, List<int> chunkOffsets);
        public delegate float[,] LoadSamples(WaveData data, List<int> chunkOffsets, long startSample, long length);

        const int DATA_BLOCK_SIZE = 40960;

        static Loader()
        {
            // set chunk loaders (data and fmt are loaded separately)
            waveChunkLoaders[RiffTags.smpl] = SamplerInfo.FillSample;
            waveChunkLoaders[RiffTags.cue] = CueInfo.FillSample;
            waveChunkLoaders[RiffTags.labl] = AdtlInfo.FillSample;
            waveChunkLoaders[RiffTags.LIST] = LoadListChunk;

            // set data loaders
            waveDataLoaders[WaveFormats.Pcm] = LoadDataFromPcm;
            waveDataLoaders[WaveFormats.Float] = LoadDataFromFloat;
        }

        public static readonly Dictionary<RiffTags, LoadChunk> waveChunkLoaders = new Dictionary<RiffTags, LoadChunk>();
        public static readonly Dictionary<WaveFormats, LoadSamples> waveDataLoaders = new Dictionary<WaveFormats, LoadSamples>();

        public static SoundData Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] currentChunk = reader.ReadBytes(0xC);

            if (currentChunk[0x0] != 'R' || currentChunk[0x1] != 'I' || currentChunk[0x2] != 'F' || currentChunk[0x3] != 'F' ||
                currentChunk[0x8] != 'W' || currentChunk[0x9] != 'A' || currentChunk[0xA] != 'V' || currentChunk[0xB] != 'E')
                throw new InvalidDataException("Not a valid wav file.");

            Dictionary<RiffTags, List<int>> chunkOffsets = new Dictionary<RiffTags, List<int>>();
            FindOffsets(reader, chunkOffsets);

            // load wave format
            if (!chunkOffsets.ContainsKey(RiffTags.fmt))
                throw new InvalidDataException("Wave file does not specify a format.");

            reader.BaseStream.Position = chunkOffsets[RiffTags.fmt][0] + 4;
            int chunkLength = reader.Read<int>();
            reader.BaseStream.Position = chunkOffsets[RiffTags.fmt][0];
            WaveData data = new WaveData(reader.ReadBytes(chunkLength + 8));
            data.reader = reader;
            data.chunkOffsets = chunkOffsets;

            // load additional info
            LoadChunk loader;
            foreach (RiffTags tag in Enum.GetValues(typeof(RiffTags)))
                if (chunkOffsets.ContainsKey(tag) && waveChunkLoaders.TryGetValue(tag, out loader))
                    loader(data, chunkOffsets[tag]);

            // load data
            if (chunkOffsets.ContainsKey(RiffTags.data))
            {
                if (waveDataLoaders.ContainsKey(data.formatTag))
                {
                    data.reader.BaseStream.Position = chunkOffsets[RiffTags.data][0] + 0x4;
                    data._numSamples = data.reader.Read<int>() / data.blockAlign;
                }
                else
                    throw new NotImplementedException($"Wave format {(int)data.formatTag : X8} is not supported.");
            }
            else
                throw new InvalidDataException("Wave file does not define a data chunk.");

            // set cue names
            if (data.cueInfo != null)
                foreach (CueInfo.CuePoint cuePoint in data.cueInfo.cuePoints)
                    foreach (AdtlInfo.LablInfo label in data.adtlInfos.lablInfos)
                        if (label.index == cuePoint.id)
                        {
                            cuePoint.name = label.name;
                            break;
                        }

            return data;
        }

        static void FindOffsets(BinaryReader reader, Dictionary<RiffTags, List<int>> offsets, int cursor = 0xC, int length = -1)
        {
            int end = Math.Min(cursor + length, (int)reader.BaseStream.Length - 0x8);
            if (length == -1)
                end = (int)reader.BaseStream.Length - 0x8;

            while (cursor < end)
            {
                reader.BaseStream.Position = cursor;
                RiffTags header = (RiffTags)reader.Read<int>();
                int chunkLength = reader.Read<int>();
                if (Array.IndexOf(Enum.GetValues(typeof(RiffTags)), header) != -1)
                {
                    if (!offsets.ContainsKey(header))
                        offsets[header] = new List<int>();
                    offsets[header].Add(cursor);
                }
                else
                {
                    // ignore unknown chunk tags
                }

                cursor += (chunkLength + 1) / 2 * 2 + 8; // RIFF chunk lengths must be even
            }
        }

        static float[,] LoadDataFromPcm(WaveData data, List<int> dataChunkOffsets, long startSample, long length)
        {
            int dataChunkOffset = dataChunkOffsets[0];
            length = Math.Min(startSample + length, data.numSamples) - startSample;
            float[,] newData = new float[data.channels, length];

            ulong maxValue = ulong.MaxValue >> (64 - (data.bitsPerSample - 1));

            data.reader.BaseStream.Position = dataChunkOffset + 0x8 + data.blockAlign * startSample;
            byte currentByte = 0;
            byte[] dataBlock = new byte[DATA_BLOCK_SIZE];
            int dataBlockOffset = 0;

            for (int sample = 0; sample < length; sample++)
            {
                int currentBitIndex = 0;
                for (int channel = 0; channel < data.channels; channel++)
                {
                    long currentSample = 0;
                    for (int bit = 0; bit < data.bitsPerSample; bit++)
                    {
                        if (currentBitIndex % 8 == 0)
                        {
                            if (dataBlockOffset == 0)
                                data.reader.BaseStream.Read(dataBlock, 0, dataBlock.Length);

                            currentByte = dataBlock[dataBlockOffset];
                            dataBlockOffset = (dataBlockOffset + 1) % dataBlock.Length;
                        }

                        int bitValue = currentByte & (0x1 << (currentBitIndex % 8));
                        if (bit == data.bitsPerSample - 1 && bitValue > 0)
                            currentSample |= (long)(ulong.MaxValue ^ maxValue);

                        currentSample |= (long)(bitValue << ((bit / 8 % 8) * 8));
                        currentBitIndex++;
                    }

                    newData[channel, sample] = (float)currentSample / maxValue;
                }
            }

            return newData;
        }

        static float[,] LoadDataFromFloat(WaveData data, List<int> dataChunkOffsets, long startSample, long length)
        {
            int dataChunkOffset = dataChunkOffsets[0];
            data.reader.BaseStream.Position = dataChunkOffset + 0x8;

            length = Math.Min(startSample + length, data.numSamples) - startSample;
            float[,] newData = new float[data.channels, length];

            byte[] dataBlock = new byte[DATA_BLOCK_SIZE];
            int dataBlockOffset = 0;

            long cursor = dataChunkOffset + 0x8 + data.blockAlign * startSample;
            for (int sample = 0; sample < length; sample++)
            {
                data.reader.BaseStream.Position = cursor;
                for (int channel = 0; channel < data.channels; channel++)
                {
                    if (dataBlockOffset == 0)
                        data.reader.BaseStream.Read(dataBlock, 0, dataBlock.Length);

                    dataBlockOffset = (dataBlockOffset + 0x4) % dataBlock.Length;
                    newData[channel, sample] = BitConverter.ToSingle(dataBlock, dataBlockOffset);
                }

                cursor += data.blockAlign;
            }

            return newData;
        }

        static void LoadListChunk(WaveData data, List<int> listChunkOffsets)
        {
            foreach (int offset in listChunkOffsets)
            {
                data.reader.BaseStream.Position = offset + 0x4;
                int len = data.reader.Read<int>();
                ListChunkTypes type = (ListChunkTypes)data.reader.Read<int>();

                if (Array.IndexOf(Enum.GetValues(typeof(ListChunkTypes)), type) != -1)
                {
                    Dictionary<RiffTags, List<int>> chunkOffsets = new Dictionary<RiffTags, List<int>>();
                    FindOffsets(data.reader, chunkOffsets, offset + 0xC, len - 0x4);

                    LoadChunk loader;
                    foreach (RiffTags tag in Enum.GetValues(typeof(RiffTags)))
                        if (chunkOffsets.ContainsKey(tag) && waveChunkLoaders.TryGetValue(tag, out loader))
                            loader(data, chunkOffsets[tag]);
                }
                else
                    throw new InvalidDataException($"Unknown ListChunk:\n{GetTagString((int)type)}");
            }
        }

        static string GetTagString(int tag)
            => string.Empty
                + (char)((tag >> 0x00) & 0xFF)
                + (char)((tag >> 0x08) & 0xFF)
                + (char)((tag >> 0x10) & 0xFF)
                + (char)((tag >> 0x18) & 0xFF) + "\nhex:\t"
                + ((tag >> 0x00) & 0xFF).ToString("X")
                + ((tag >> 0x08) & 0xFF).ToString("X")
                + ((tag >> 0x10) & 0xFF).ToString("X")
                + ((tag >> 0x18) & 0xFF).ToString("X");
    }
}
