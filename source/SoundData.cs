using ChaosFramework.Core;

namespace ChaosFramework.Sound
{
    public abstract class SoundData : Disposable
    {
        internal float[,] samples;

        public short channels { get; protected set; }
        public int samplesPerSecond { get; protected set; }

        public float lengthInSeconds
            => (float)numSamples / samplesPerSecond;

        public abstract long numSamples { get; }

        protected SoundData() { }

        public short[] GetPcmData(bool makeMono)
        {
            int numChannels = samples.GetLength(0);
            short[] pcmData = new short[makeMono ? numSamples : numChannels * numSamples];
            if (makeMono)
                for (int sample = 0; sample < numSamples; sample++)
                    pcmData[sample] = (short)(samples[0, sample] * short.MaxValue);
            else
                for (int sample = 0; sample < numSamples; sample++)
                    for (int channel = 0; channel < numChannels; channel++)
                        pcmData[sample * numChannels + channel] = (short)(samples[channel, sample] * short.MaxValue);

            return pcmData;
        }

        public abstract MarkerList GetMarkers();

        internal abstract void LoadData();
    }
}
