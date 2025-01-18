using ChaosFramework.Collections;
using ChaosFramework.Sound.Markers;
using System.Collections;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound
{
    public sealed class MarkerList : SysCol.IEnumerable<Marker>
    {
        // TODO: this detail should not be public
        /// <summary>
        ///     This field should only be used by implementations of <see cref="Playable"/>
        ///     and is not meant for consumer purposes.
        /// </summary>
        public readonly AdvancedLinkedList<Marker> list = new AdvancedLinkedList<Marker>();

        public int length => list.length;

        public void Add(Marker marker)
        {
            if (marker != null && !list.Contains(marker))
                list.AddSorted(marker);
        }

        public void Add(SysCol.IEnumerable<Marker> markers)
        {
            foreach (Marker m in markers)
                Add(m);
        }

        public void Remove(Marker marker)
            => list.Remove(marker);

        public void Clear()
            => list.Clear();

        public SysCol.IEnumerator<Marker> GetEnumerator()
            => ((SysCol.IEnumerable<Marker>)list).GetEnumerator();

        SysCol.IEnumerator<Marker> SysCol.IEnumerable<Marker>.GetEnumerator()
            => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
