using ChaosFramework.Collections.Immutable;
using System.Linq;

namespace ChaosFramework.Sound.Markers
{
    public class TimeSignature
        : Marker<TimeSignature>
    {
        public static TimeSignature Parse(string argString)
        {
            string[] args = argString.Split(';');
            if (args.Length < 2)
                return null;

            long pos;
            if (!long.TryParse(args[0], out pos))
                return null;

            float[] eighthNotes = new float[args.Length - 1];
            for (int i = 0; i < args.Length - 1; i++)
            {
                string[] split = args[i + 1].Split('/');
                if (split.Length != 2)
                    return null;

                int denom;
                int count;
                if (!int.TryParse(split[0].Trim(), out count) || !int.TryParse(split[1].Trim(), out denom))
                    return null;

                eighthNotes[i] = 8 * (count / (float)denom);
            }

            return new TimeSignature(pos, eighthNotes);
        }

        public readonly ImmutableArray<float> eighthNotes;
        public readonly float totalEighths;

        public TimeSignature(long position, float[] eighthNotes)
            : base(position)
        {
            this.eighthNotes = eighthNotes;
            totalEighths = eighthNotes.Sum();
        }

        public override bool CompareContents(TimeSignature other)
            => eighthNotes.SequenceEqual(other.eighthNotes);
    }
}
