using ChaosFramework.IO;

namespace ChaosFramework.Sound.Codex.Wave.Meta
{
    public class SampleLoop
    {
        public int id,
                   type,
                   start,
                   end,
                   fraction,
                   playCount;

        private SampleLoop(int id, int type, int start, int end, int fraction, int playCount)
        {
            this.id = id;
            this.type = type;
            this.start = start;
            this.end = end;
            this.fraction = fraction;
            this.playCount = playCount;
        }

        public static SampleLoop FromWaveStream(WaveData data, int offset)
        {
            data.reader.BaseStream.Position = offset;
            return new SampleLoop(
                id: data.reader.Read<int>(),
                type: data.reader.Read<int>(),
                start: data.reader.Read<int>(),
                end: data.reader.Read<int>(),
                fraction: data.reader.Read<int>(),
                playCount: data.reader.Read<int>()
                );
        }
    }
}
