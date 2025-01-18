using ChaosFramework.IO;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Wave.Meta
{
    partial class AdtlInfo
    {
        public class LablInfo
        {
            public readonly int index;
            public readonly string name;

            public LablInfo(int index, string name)
            {
                this.index = index;
                this.name = name;
            }
        }

        public static void FillSample(WaveData data, SysCol.List<int> offsets)
        {
            if (data.adtlInfos == null)
                data.adtlInfos = new AdtlInfo();

            foreach (int offset in offsets)
            {
                data.reader.BaseStream.Position = offset + 0x8;
                int index = data.reader.Read<int>();
                string name = string.Empty;
                char readCharacter;
                while ((readCharacter = (char)data.reader.Read<byte>()) != 0x00)
                    name += readCharacter;

                data.adtlInfos.lablInfos.Add(new LablInfo(index, name));
            }
        }
    }
}
