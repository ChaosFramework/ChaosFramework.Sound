using ChaosFramework.Sound.Markers;
using ChaosUtil.Reflection;
using System;
using System.IO;
using System.Linq;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Ogg
{
    public static class Loader
    {
        static readonly char[] MARKER_SEPARATORS = new[] { '#', '\n' };

        static string SelectKey(SysCol.KeyValuePair<string, SysCol.IReadOnlyList<string>> kvp)
            => kvp.Key;

        static string SelectFirstValue(SysCol.KeyValuePair<string, SysCol.IReadOnlyList<string>> kvp)
            => kvp.Value.Single();

        public static SysCol.Dictionary<string, string> GetMeta(NVorbis.VorbisReader vorbis)
            => ParseMeta(vorbis.Comments);

        public static SysCol.Dictionary<string, string> ParseMeta(string[] comments)
        {
            SysCol.Dictionary<string, string> meta = new SysCol.Dictionary<string, string>();
            foreach (string comment in comments)
            {
                string[] keyValue = comment.Split(new char[] { '=' }, 2);
                meta[keyValue[0].ToLower().Trim()] = keyValue[1];
            }

            return meta;
        }

        public static SoundData Load(Stream source)
            => new OggData(source);

        public static MarkerList ParseMarkers(SysCol.Dictionary<string, string> meta)
        {
            MarkerList markers = new MarkerList();
            foreach (Type t in AssemblyManager.SubTypesOf(typeof(Marker)))
            {
                string line;
                if (meta.TryGetValue(t.Name.ToLower(), out line))
                    foreach (string args in line.Split(MARKER_SEPARATORS))
                        markers.Add(MarkerFactory.ParseMarker(t, args));
            }

            return markers;
        }
    }
}
