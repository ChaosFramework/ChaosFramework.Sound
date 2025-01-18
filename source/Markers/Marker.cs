using ChaosUtil.Reflection;
using System;

namespace ChaosFramework.Sound.Markers
{
    [AssemblyManager.ListSubTypes]
    public interface Marker : IComparable<Marker>
    {
        long sampleIndex { get; }
        string GenerateArgsString();
    }

    [AssemblyManager.ListSubTypes]
    public abstract class Marker<T>
        : Marker
        where T : Marker<T>
    {
        public readonly long sampleIndex;
        long Marker.sampleIndex => sampleIndex;

        public Marker(long sampleIndex)
        {
            this.sampleIndex = sampleIndex;
        }

        public virtual string GenerateArgsString()
            => sampleIndex.ToString();

        public static bool operator ==(Marker<T> a, Marker<T> b)
        {
            if (a == default(object) ^ b == default(object)) return false;
            if (a == default(object) && b == default(object)) return true;
            return a.Equals(b);
        }

        public static bool operator !=(Marker<T> a, Marker<T> b)
            => !(a == b);

        public override bool Equals(object obj)
            => Equals(obj as Marker<T>);

        public bool Equals(Marker<T> other)
            => other != null && sampleIndex == other.sampleIndex && CompareContents((T)other);

        public override int GetHashCode()
            => (int)sampleIndex;

        public abstract bool CompareContents(T other);

        int IComparable<Marker>.CompareTo(Marker other)
            => sampleIndex.CompareTo(other.sampleIndex);
    }
}
