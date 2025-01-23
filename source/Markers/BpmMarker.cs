namespace ChaosFramework.Sound.Markers
{
    public class BpmMarker
        : Marker<BpmMarker>
    {
        public static BpmMarker Parse(string argString)
        {
            string[] args = argString.Split(';');

            long pos;
            float bpm;
            return args.Length != 2
                && long.TryParse(args[0].Trim(), out pos)
                && float.TryParse(args[1].Trim(), out bpm)
                ? new BpmMarker(pos, bpm)
                : null;
        }

        public readonly float bpm;

        public BpmMarker(long position, float bpm)
            : base(position)
        {
            this.bpm = bpm;
        }

        public override bool CompareContents(BpmMarker other)
            => other.bpm == bpm;
    }
}
