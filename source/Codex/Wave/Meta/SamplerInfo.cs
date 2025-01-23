using ChaosFramework.IO;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Wave.Meta
{
    public class SamplerInfo
    {
        public readonly int manufacturer,
                            product,
                            samplePeriod,
                            midiUnityNode,
                            midiPitchFraction,
                            sampleFormat,
                            sampleOffset,
                            samplerData;

        public readonly SampleLoop[] sampleLoops;

        private SamplerInfo(
            int manufacturer,
            int product,
            int samplePeriod,
            int midiUnityNode,
            int midiPitchFraction,
            int sampleFormat,
            int sampleOffset,
            int samplerData
            )
        {
            this.manufacturer = manufacturer;
            this.product = product;
            this.samplePeriod = samplePeriod;
            this.midiUnityNode = midiUnityNode;
            this.midiPitchFraction = midiPitchFraction;
            this.sampleFormat = sampleFormat;
            this.sampleOffset = sampleOffset;
            this.samplerData = samplerData;
            sampleLoops = new SampleLoop[samplerData];
        }

        public static void FillSample(WaveData data, SysCol.List<int> offsets)
        {
            int samplerIndex = 0;
            data.samplerInfo = new SamplerInfo[offsets.Count];
            foreach (int offset in offsets)
            {
                data.reader.BaseStream.Position = offset + 8;
                SamplerInfo info = new SamplerInfo(
                    manufacturer: data.reader.Read<int>(),
                    product: data.reader.Read<int>(),
                    samplePeriod: data.reader.Read<int>(),
                    midiUnityNode: data.reader.Read<int>(),
                    midiPitchFraction: data.reader.Read<int>(),
                    sampleFormat: data.reader.Read<int>(),
                    sampleOffset: data.reader.Read<int>(),
                    samplerData: data.reader.Read<int>()
                    );

                // 0x28 length of extra data (ignored)
                int cursor = offset + 0x2C;
                for (int i = 0; i < info.sampleLoops.Length; i++)
                {
                    info.sampleLoops[i] = SampleLoop.FromWaveStream(data, cursor);
                    cursor += 0x18;
                }

                data.samplerInfo[samplerIndex++] = info;
            }
        }
    }
}
