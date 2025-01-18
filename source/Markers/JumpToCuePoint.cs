using ChaosFramework.Collections;
using ChaosFramework.Collections.Immutable;
using ChaosUtil.Primitives;
using System.Linq;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Markers
{
    public abstract class JumpToCuePoint<T>
        : JumpMarker<JumpToCuePoint<T>>
        where T : JumpToCuePoint<T>
    {
        protected static string ParseCuePointName(string s)
            => s.Trim();

        protected readonly ImmutableHashSet<string> targetNames;

        public JumpToCuePoint(long position, SysCol.HashSet<string> targetNames)
            : base(position)
        {
            this.targetNames = targetNames;
        }

        public override sealed bool CompareContents(JumpToCuePoint<T> other)
            => targetNames.All(other.targetNames.Contains) && CompareContents((T)other);

        protected virtual bool CompareContents(T other)
            => true;

        public override long Perform(Playable sender)
        {
            LinkedList<CuePoint> targets = new LinkedList<CuePoint>(
                sender.markers
                    .OfType<CuePoint>()
                    .Where(ContainsTarget)
                );

            return targets
                .Skip(Random.instance.RndInt(targets.length))
                .FirstOrDefault()
                ?.sampleIndex ?? -1;
        }

        protected bool ContainsTarget(CuePoint cuePoint)
            => targetNames.Contains(cuePoint.name);
    }

    public class JumpToCuePoint
        : JumpToCuePoint<JumpToCuePoint>
    {
        public static JumpToCuePoint Parse(string argString)
        {
            string[] args = argString.Split(';');

            long pos;
            return args.Length >= 2
                && long.TryParse(args[0], out pos)
                ? new JumpToCuePoint(pos, new SysCol.HashSet<string>(args.Skip(1).Select(ParseCuePointName)))
                : null;
        }

        public JumpToCuePoint(long position, SysCol.HashSet<string> targetNames)
            : base(position, targetNames)
        { }

        public override string GenerateArgsString()
            => $"{sampleIndex}; {string.Join("; ", targetNames)}";
    }
}
