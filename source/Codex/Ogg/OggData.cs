using ChaosFramework.Collections;
using System;
using System.IO;
using SysCol = System.Collections.Generic;

namespace ChaosFramework.Sound.Codex.Ogg
{
    public sealed class OggData : SoundData
    {
        readonly SysCol.Dictionary<string, string> meta = new SysCol.Dictionary<string, string>();
        readonly NVorbis.VorbisReader vorbis;

        public override long numSamples
            => vorbis.TotalSamples;

        public OggData(Stream source)
        {
            vorbis = new NVorbis.VorbisReader(source, false);
            meta = Loader.GetMeta(vorbis);
            samplesPerSecond = vorbis.SampleRate;
            channels = (short)vorbis.Channels;
        }

        public string GetMeta(string key)
        {
            string value;
            return meta.TryGetValue(key.ToLower(), out value) ? value : null;
        }

        public Tuple<string, string>[] GetAllMeta()
        {
            LinkedList<Tuple<string, string>> lst = new LinkedList<Tuple<string, string>>();
            foreach (SysCol.KeyValuePair<string, string> comment in meta)
                lst.Add(new Tuple<string, string>(comment.Key, comment.Value));

            return lst.ToArray();
        }

        internal override void LoadData()
        {
            float[] buffer = new float[numSamples * vorbis.Channels];
            vorbis.DecodedPosition = 0;
            vorbis.ReadSamples(buffer, 0, buffer.Length);

            samples = new float[vorbis.Channels, numSamples];
            for (int i = 0; i < numSamples; i++)
                for (int channel = 0; channel < vorbis.Channels; channel++)
                    samples[channel, i] = buffer[i * vorbis.Channels + channel];
        }

        public override MarkerList GetMarkers()
            => Loader.ParseMarkers(meta);

        protected override void DoDispose()
        {
            base.DoDispose();
            vorbis.Dispose();
        }
    }
}
