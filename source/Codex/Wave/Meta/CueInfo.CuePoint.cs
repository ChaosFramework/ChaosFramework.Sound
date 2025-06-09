using ChaosFramework.IO;

namespace ChaosFramework.Sound.Codex.Wave.Meta
{
    partial class CueInfo
    {
        public class CuePoint
        {
            public readonly int id,
                                position,
                                fccChunk,
                                chunkStart,
                                blockStart,
                                sampleOffset;

            public string name { get; internal set; } = "";

            private CuePoint(int id, int position, int fccChunk, int chunkStart, int blockStart, int sampleOffset)
            {
                this.id = id;
                this.position = position;
                this.fccChunk = fccChunk;
                this.chunkStart = chunkStart;
                this.blockStart = blockStart;
                this.sampleOffset = sampleOffset;
            }

            public static CuePoint FromWaveStream(WaveData data, int offset)
            {
                data.reader.BaseStream.Position = offset;
                return new CuePoint(
                    id: data.reader.Read<int>(),
                    position: data.reader.Read<int>(),
                    fccChunk: data.reader.Read<int>(),
                    chunkStart: data.reader.Read<int>(),
                    blockStart: data.reader.Read<int>(),
                    sampleOffset: data.reader.Read<int>()
                );
            }
        }
    }
}
