using ChaosUtil.Reflection;
using System;
using System.Reflection;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Markers
{
    public static class MarkerFactory
    {
        delegate Marker Parse(string args);

        static SysCol.Dictionary<string, Parse> parsers = new SysCol.Dictionary<string, Parse>();

        static MarkerFactory()
        {
            Type[] parserArgTypes = new[] { typeof(string) };

            // TODO: Stop depending on initialization order (AssemblyManager.Init before static ctor)
            foreach (Type t in AssemblyManager.SubTypesOf(typeof(Marker)))
            {
                MethodInfo factory = t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, parserArgTypes, null);
                if (factory != null)
                    parsers[t.Name.ToLower()] = args => (Marker)factory.Invoke(null, new object[] { args });
            }
        }

        public static Marker ParseMarker(Type markerType, string args)
            => ParseMarker(markerType.Name, args);

        static Marker ParseMarker(string markerType, string args)
        {
            Parse parse;
            if (parsers.TryGetValue(markerType.ToLower(), out parse))
                return parse(args);

            return null;
        }
    }
}
