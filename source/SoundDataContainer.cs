using ChaosFramework.IO.Containers;
using ChaosFramework.IO.Streams;
using ChaosFramework.Sound.Codex;
using System.IO;

namespace ChaosFramework.Sound
{
    public class SoundDataContainer : AssetContainer<SoundData>
    {
        public SoundDataContainer(StreamSource streamSource, bool monitoring = true)
            : base(streamSource, monitoring)
        { }

        protected override void DisposeItem(SoundData obj)
            => obj?.Dispose();

        protected override SoundData LoadFromStream(Key key, Stream resource, CancellationToken cancel)
            => SoundLoader.Load(resource, Path.GetExtension(key.key).TrimStart('.'));
    }
}
