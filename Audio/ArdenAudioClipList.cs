using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall
{
    [CreateAssetMenu(menuName = "Ardenfall/Audio/AudioClip List")]
    public class ArdenAudioClipList : ArdenAudioAsset
    {
        public List<ArdenAudioClip> audioClips;


        public override ArdenAudioInstance PlayAtPoint(AudioSourceSettings settings, Vector3 location, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            int r = Random.Range(0, audioClips.Count);
            return audioClips[r].PlayAtPoint(settings,location,volume,group,looped);
        }

        public override ArdenAudioInstance Play(AudioSourceSettings settings, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            int r = Random.Range(0, audioClips.Count);
            return audioClips[r].Play(settings, volume, group, looped);
        }


    }
}