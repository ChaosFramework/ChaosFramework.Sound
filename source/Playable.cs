using ChaosFramework.Core;

namespace ChaosFramework.Sound
{
    public abstract class Playable : Disposable
    {
        public readonly MarkerList markers = new MarkerList();

        public abstract long totalSampleCount { get; }
        public abstract int sampleRate { get; }
    }
}
