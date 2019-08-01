using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall.Audio
{
    [CreateAssetMenu(menuName = "Ardenfall/Audio/Material Sound")]
    public class MaterialSound : ScriptableObject
    {
        [System.Serializable]
        public class MaterialInteraction
        {
            public MaterialSound material;
            public List<ArdenAudioClip> sounds;

        }

        public List<MaterialInteraction> interactions;

        public MaterialInteraction GetInteraction(MaterialSound material)
        {
            foreach(MaterialInteraction interaction in interactions)
            {
                if (interaction.material == material)
                    return interaction;
            }
            return null;
        }

    }
}