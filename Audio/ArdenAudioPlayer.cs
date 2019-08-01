using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ardenfall.Audio
{
    public class ArdenAudioPlayer : MonoBehaviour
    {

        public ArdenAudioAsset asset;
        public ArdenAudioClipList clips;

        public MixerGroup group = MixerGroup.ambient;

        public bool customSettings = false;

        public AudioSourceSettings settings;

        [Tooltip("Whether to attach sound to position of this object")]
        public bool keepAttached = false;

        [Range(0, 1)]
        public float volume = 1;
        public bool looped;

        [Tooltip("If true, then the loop will begin at a random starting location")]
        public bool randomStart = false;

        [Tooltip("Whether to stop playing audio when player is destroyed or disabled")]
        public bool stopOnDestroy = false;

        public bool is2D = false;
        
        private ArdenAudioInstance instance;
        private PooledAudioSource audioSource;

        private bool quitting;

        private void Update()
        {
            if(keepAttached)
            {
                if (instance != null)
                    instance.SetPosition(transform.position);

                if (audioSource != null)
                    audioSource.audioSource.transform.position = transform.position;
            }
            
        }

        private AudioSourceSettings Settings
        {
            get
            {
                if (customSettings)
                    return settings;
                else
                    return null;
            }
        }

        void OnEnable()
        {
            if (clips != null)
            {
                if (is2D)
                    audioSource = SoundManager.PlayAudio(clips.audioClips, group, volume,looped, Settings);
                else
                    audioSource = SoundManager.PlayAudioAtPoint(clips.audioClips, transform.position, group, volume,  looped, Settings);

                if (looped && randomStart && audioSource != null)
                    audioSource.RandomTime();
            }
            
            if (asset != null)
            {
                if (is2D)
                    instance = asset.Play(Settings, volume, group, looped);
                else
                    instance = asset.PlayAtPoint(Settings, transform.position, volume, group, looped);

                if (looped && randomStart && instance != null)
                    instance.RandomTime();
            }
        }

        protected void OnDisable()
        {
            if (quitting)
                return;

            if(looped || stopOnDestroy)
            {
                if (instance != null)
                    instance.Stop();

                if (audioSource != null)
                    SoundManager.StopPooledClip(audioSource);
            }
        }

        protected void OnApplicationQuit()
        {
            quitting = true;
        }

        protected void OnDestroy()
        {
            if (quitting)
                return;

            if (looped || stopOnDestroy)
            {
                if (instance != null)
                    instance.Stop();

                if (audioSource != null)
                    SoundManager.StopPooledClip(audioSource);
            }
        }

    }

}
