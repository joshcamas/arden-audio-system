using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall
{
    [System.Serializable, CreateAssetMenu(menuName = "Ardenfall/Audio/Mixer 2.0")]
    public class ArdenAudioMixer : ArdenAudioAsset
    {
        public List<MixerLayer> layers;


        public override ArdenAudioInstance Play(AudioSourceSettings settings,float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return Play_Internal(settings, volume, group, looped, false, new Vector3());
        }

        public override ArdenAudioInstance PlayAtPoint(AudioSourceSettings settings, Vector3 location, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return Play_Internal(settings,volume, group, looped, true, location);
        }

        //A single function to unify play and play at point, under the hood
        protected ArdenAudioInstance Play_Internal(AudioSourceSettings settings, float volume, MixerGroup group, bool looped, bool playAtPoint, Vector3 location)
        {
            if (layers == null)
                return null;

            if (layers.Count == 0)
                return null;

            ArdenAudioMixerInstance instance = new ArdenAudioMixerInstance(this, settings);
            instance.Init(volume, group, playAtPoint, location);

            return instance;
        }

        [System.Serializable]
        public class MixerLayer
        {
            public ArdenAudioAsset asset;

            [Range(0,1)]
            public float volume = 1;

            public bool loop = true;

            public ShotMode shotMode;

            public int shotValue;

            public float randomChance = 1;

            //Returns how long to delay until this shot should be played
            //In seconds
            public float GetShotDelay()
            {
                if (shotMode == ShotMode.Hour)
                    return 3600 / shotValue;

                if (shotMode == ShotMode.Minute)
                    return 60 / shotValue;

                if (shotMode == ShotMode.Second)
                    return 1 / shotValue;

                return 0;
            }
        }

        public enum ShotMode { Second,Minute,Hour}
    }

    public class ArdenAudioMixerInstance : ArdenAudioInstance
    {

        private MixerGroup group;
        private bool playAtPoint;
        private Vector3 location;
        private float volume;

        private List<ArdenAudioInstance> currentlyPlayingChildren;

        private ArdenAudioMixer Mixer { get { return (ArdenAudioMixer)asset; } }
        
        public ArdenAudioMixerInstance(ArdenAudioAsset asset,AudioSourceSettings settings) : base(asset,settings) { }

        public void Init(float volume, MixerGroup group, bool playAtPoint, Vector3 position)
        {

            this.volume = volume;
            this.group = group;
            this.playAtPoint = playAtPoint;
            this.position = position;

            currentlyPlayingChildren = new List<ArdenAudioInstance>();
            Play();
        }

        private void Play()
        {
            for (int i = 0; i < Mixer.layers.Count;i++)
            {
                PlayLayer(Mixer.layers[i]);
            }
                
        }

        private void PlayLayer(ArdenAudioMixer.MixerLayer layer)
        {
            
            if (layer.loop)
            {
                PlayLayerAudio(layer, true);
            } else
            {
                //Wait for the delay, then play a shot, and start timer again
                float delay = layer.GetShotDelay();

                Utility.CoroutineManager.RunDelayingCoroutine(delay, () =>
                {
                    if (completed || stopped)
                        return;
                    
                    //Random chance utilized
                    if(Random.Range(0,1) < layer.randomChance)
                        PlayLayerAudio(layer, false);

                    //Play layer again
                    PlayLayer(layer);
                });
            }
        }
        
        private void PlayLayerAudio(ArdenAudioMixer.MixerLayer layer,bool looping)
        {
            ArdenAudioInstance instance;

            if (playAtPoint)
                instance = layer.asset.PlayAtPoint(sourceSettings,location, layer.volume * volume, group, looping);
            else
                instance = layer.asset.Play(sourceSettings, layer.volume * volume, group, looping);

            currentlyPlayingChildren.Add(instance);

            instance.OnComplete += () =>
            {
                currentlyPlayingChildren.Remove(instance);
            };
        }


        public override void SetPosition(Vector3 position)
        {
            this.position = position;

            foreach (ArdenAudioInstance instance in currentlyPlayingChildren)
                instance.SetPosition(position);

        }

        public override void SetVolume(float volume)
        {
            this._volume = volume;

            foreach (ArdenAudioInstance instance in currentlyPlayingChildren)
                instance.SetVolume(volume);

        }

        public override void Stop()
        {
            if (completed)
                return;

            base.Stop();

            for(int i=0;i<currentlyPlayingChildren.Count;i++)
            {
                currentlyPlayingChildren[i].Stop();
            }
        }
    }
}