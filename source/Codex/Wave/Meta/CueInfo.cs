using ChaosFramework.IO;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Wave.Meta
{
    public partial class CueInfo
    {
        public CuePoint[] cuePoints;

        public static void FillSample(WaveData data, SysCol.List<int> offsets)
        {
            int offset = offsets[0];
            data.reader.BaseStream.Position = offset + 0x8;
            data.cueInfo = new CueInfo();
            data.cueInfo.cuePoints = new CuePoint[data.reader.Read<int>()];

            for (int i = 0; i < data.cueInfo.cuePoints.Length; i++)
                data.cueInfo.cuePoints[i] = CuePoint.FromWaveStream(data, offset + 0x18 * i + 0xC);
        }
    }
}
