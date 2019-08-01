using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Ardenfall.Audio
{
    [CreateAssetMenu(menuName ="Ardenfall/Audio/Filters/Lowpass Filter")]
    public class ArdenAudioLowpassFilter : ArdenAudioFilter
    {
        [Range(0, 22000)]
        public float cutoffFrequency = 5000;

        [Range(1, 10)]
        public float lowpassResonanceQ = 1;

        public override bool AttachToObject(GameObject gameObject,ArdenAudioFilterInstance instance)
        {
            if (instance.IsAttached(gameObject))
                return true;

            //Detect filter is already added
            if (gameObject.GetComponent<AudioLowPassFilter>())
                return false;

            AudioLowPassFilter filter = gameObject.AddComponent<AudioLowPassFilter>();

            filter.cutoffFrequency = cutoffFrequency;
            filter.lowpassResonanceQ = lowpassResonanceQ;

            instance.Attach(gameObject, new List<Component> { filter });
            RegisterInstance(gameObject, instance);

            return true;

        }

    }

}