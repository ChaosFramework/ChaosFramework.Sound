using ChaosFramework.Collections.Immutable;
using ChaosUtil.Primitives;
using System.Linq;

namespace ChaosFramework.Sound.Markers
{
    public class ProbabilityJump
        : JumpToCuePoint<ProbabilityJump>
    {
        public static ProbabilityJump Parse(string argString)
        {
            string[] args = argString.Split(';');

            long pos;
            float prob;
            return args.Length > 2
                && long.TryParse(args[0], out pos)
                && float.TryParse(args[1], out prob)
                ? new ProbabilityJump(pos, prob, new ImmutableHashSet<string>(args.Skip(2).Select(ParseCuePointName)))
                : null;
        }

        readonly float probability;

        public ProbabilityJump(long position, float probability, ImmutableHashSet<string> targetNames)
            : base(position, targetNames)
        {
            this.probability = probability;
        }

        protected override bool CompareContents(ProbabilityJump other)
            => probability == other.probability;

        public override bool DoJump(Playable sender)
            => Random.instance.RndBool(probability);
    }
}
