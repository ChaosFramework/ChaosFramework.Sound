namespace ChaosFramework.Sound.Markers
{
    public class CuePoint
        : Marker<CuePoint>
    {
        public static CuePoint Parse(string argString)
        {
            string[] args = argString.Split(';');

            long pos;
            return args.Length == 2
                && long.TryParse(args[0], out pos)
                ? new CuePoint(pos, args[1].Trim())
                : null;
        }

        public readonly string name;

        public CuePoint(long position, string name)
            : base(position)
        {
            this.name = name;
        }

        public override string GenerateArgsString()
            => $"{sampleIndex}; {name}";

        public override bool CompareContents(CuePoint other)
            => name == other.name;
    }
}
