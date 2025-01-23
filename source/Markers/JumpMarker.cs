namespace ChaosFramework.Sound.Markers
{
    public interface JumpMarker : Marker
    {
        bool DoJump(Playable sender);
        long Perform(Playable sender);
    }

    public abstract class JumpMarker<T>
        : Marker<T>, JumpMarker
        where T : JumpMarker<T>
    {
        public JumpMarker(long sampleIndex)
            : base(sampleIndex)
        { }

        public virtual bool DoJump(Playable sender)
            => true;

        public abstract long Perform(Playable sender);
    }
}
