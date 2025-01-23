using ChaosFramework.Sound.Markers;
using System;
using System.IO;
using System.Linq;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Wave
{
    using Meta;

    public sealed class WaveData : SoundData
    {
        static Marker ConvertWavCuePoint(CueInfo.CuePoint cp)
            => new CuePoint(cp.sampleOffset, cp.name);

        public SamplerInfo[] samplerInfo = null;
        public CueInfo cueInfo = null;
        public AdtlInfo adtlInfos = null;

        public readonly short bitsPerSample;
        public readonly WaveFormats formatTag;
        public readonly int avgBytesPerSecond;
        public readonly short blockAlign;

        internal BinaryReader reader;
        internal SysCol.Dictionary<RiffTags, SysCol.List<int>> chunkOffsets;

        internal long _numSamples;
        public override long numSamples => _numSamples;

        internal WaveData(byte[] formatChunk)
        {
            formatTag = (WaveFormats)BitConverter.ToInt16(formatChunk, 0x8);
            channels = BitConverter.ToInt16(formatChunk, 0xA);
            samplesPerSecond = BitConverter.ToInt32(formatChunk, 0xC);
            avgBytesPerSecond = BitConverter.ToInt32(formatChunk, 0x10);
            blockAlign = BitConverter.ToInt16(formatChunk, 0x14);
            bitsPerSample = BitConverter.ToInt16(formatChunk, 0x16);
        }

        internal override void LoadData()
            => samples = Loader.waveDataLoaders[formatTag](this, chunkOffsets[RiffTags.data], 0, numSamples);

        public override MarkerList GetMarkers()
        {
            MarkerList result = new MarkerList();
            if (cueInfo != null)
                result.Add(cueInfo.cuePoints.Select(ConvertWavCuePoint));

            return result;
        }

        protected override void DoDispose()
        {
            base.DoDispose();
            reader.Dispose();
        }
    }
}
