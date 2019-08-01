using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall.Audio
{
    [CreateAssetMenu(menuName = "Ardenfall/Audio/Material Sound List")]
    public class MaterialSoundList : ScriptableObject
    {
        public List<MaterialSound> materialSounds;

    }
}