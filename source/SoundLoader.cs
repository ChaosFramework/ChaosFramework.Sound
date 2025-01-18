using System;
using System.Collections.Generic;
using System.IO;

namespace ChaosFramework.Sound.Codex
{
    public static class SoundLoader
    {
        /// <param name="stream"> The ownership of this instance is to be transferred to the constructed <see cref="SoundData"/>. </param>
        delegate SoundData Loader(Stream stream);

        static Dictionary<string, Loader> fileLoaders = new Dictionary<string, Loader>()
        {
            { "wav", Wave.Loader.Load },
            { "ogg", Ogg.Loader.Load },
        };

        /// <param name="stream"> The ownership of this instance is transferred to the constructed <see cref="SoundData". </param>
        public static SoundData Load(Stream stream, string fileEnding)
        {
            fileEnding = fileEnding.ToLower();
            Loader load;
            if (fileLoaders.TryGetValue(fileEnding, out load))
            {
                SoundData output = load(stream);
                try
                {
                    output.LoadData();
                    return output;
                }
                catch
                {
                    output.Dispose();
                    throw;
                }
            }
            else
            {
                stream.Dispose();
                throw new NotSupportedException($"There is no loader for .{fileEnding} files.");
            }
        }
    }
}
