using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall.Audio
{
    public class MaterialSoundObject : MonoBehaviour
    {
        //TODO: Hide this field when ShowMaterialSlot() is false
        public MaterialSound material;

        protected virtual bool ShowMaterialSlot()
        {
            return true;
        }

        public virtual List<ArdenAudioClip> GetFootstep(MaterialSound external,Vector3 position)
        {
            if (external == null || material == null)
                return null;

            MaterialSound.MaterialInteraction matint = material.GetInteraction(external);
            
            return matint.sounds;
        }
        
    }
}