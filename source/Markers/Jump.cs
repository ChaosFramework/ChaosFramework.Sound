namespace ChaosFramework.Sound.Markers
{
    public class Jump
        : JumpMarker<Jump>
    {
        public static Jump Parse(string argString)
        {
            string[] args = argString.Split(new char[] { ';' });

            long from, to;
            return args.Length == 2
                && long.TryParse(args[0].Trim(), out from)
                && long.TryParse(args[1].Trim(), out to)
                ? new Jump(from, to)
                : null;
        }

        public readonly long targetSampleIndex;

        public Jump(long position, long target)
            : base(position)
        {
            targetSampleIndex = target;
        }

        public override long Perform(Playable sender)
            => targetSampleIndex;

        public override string GenerateArgsString()
            => $"{sampleIndex}; {targetSampleIndex}";

        public override bool CompareContents(Jump other)
            => targetSampleIndex == other.targetSampleIndex;
    }
}
