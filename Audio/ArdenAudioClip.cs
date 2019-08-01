using UnityEngine;

using UnityEngine.Serialization;

namespace Ardenfall
{
    [System.Serializable]
    public class ArdenAudioClip : ArdenAudioAsset
    {
        [Header("Audio")]
        public AudioClip audioClip;

        [Range(0,1)]
        public float volume = 1;

        public bool pitch;

        public float pitchMin;

        public float pitchMax;

        public float GetPitch()
        {
            if (pitch)
            {
                return Random.Range(pitchMin, pitchMax);
            }
            return 1;
        }

        [Header("Subtitle")]
        public string subtitle;
        public bool isSFX = false;

        public override ArdenAudioInstance PlayAtPoint(AudioSourceSettings settings,Vector3 location, float volume=1,MixerGroup group=MixerGroup.sfx, bool looped = false)
        {

            PooledAudioSource source = SoundManager.PlayPooledClip(audioClip, location, group, volume * this.volume, GetPitch(), looped, settings);

            ArdenAudioClipInstance instance = new ArdenAudioClipInstance(this, settings);
            instance.Init(source);

            return instance;
        }

        public override ArdenAudioInstance Play(AudioSourceSettings settings, float volume, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            PooledAudioSource source = SoundManager.PlayPooledClip(audioClip, group, volume * this.volume, GetPitch(), looped, settings);

            ArdenAudioClipInstance instance = new ArdenAudioClipInstance(this, settings);
            instance.Init(source);

            return instance;
        }

    }

    public class ArdenAudioClipInstance : ArdenAudioInstance
    {
        private PooledAudioSource source;
        private ArdenAudioClip Clip
        {
            get { return (ArdenAudioClip)asset; }
        }

        public ArdenAudioClipInstance(ArdenAudioAsset asset,AudioSourceSettings settings) : base(asset,settings) {    }

        public void Init(PooledAudioSource source)
        {
            this.source = source;

            position = this.source.audioSource.transform.position;

            source.onComplete += () => { _OnComplete(); };
        }

        public override void RandomTime()
        {
            source.RandomTime();
        }
        public override void SetPosition(Vector3 position)
        {
            this.position = position;

            if(this.source.audioSource != null)
                this.source.audioSource.transform.position = position;
        }

        public override void SetVolume(float volume)
        {
            if (this.source.audioSource == null)
                return;

            this.source.audioSource.volume = volume * Clip.volume;
        }

        public override void Stop()
        {
            base.Stop();
            SoundManager.StopPooledClip(source);
        }
    }


}