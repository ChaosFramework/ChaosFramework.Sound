namespace ChaosFramework.Sound.Markers
{
    public class Stop : JumpMarker<Stop>
    {
        public static Stop Parse(string args)
        {
            long pos;
            if (long.TryParse(args.Trim(), out pos))
                return new Stop(pos);

            return null;
        }

        public Stop(long sampleIndex)
            : base(sampleIndex)
        { }

        public override long Perform(Playable sender)
            => -1;

        public override string GenerateArgsString()
            => sampleIndex.ToString();

        public override bool CompareContents(Stop other)
            => true;
    }
}
