using UnityEngine;

namespace Ardenfall
{
    //A small wrapper to control a arden audio source
    [System.Serializable]
    public class ArdenAudioAsset : ScriptableObject
    {

        public virtual ArdenAudioInstance PlayAtPoint(Vector3 location, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return PlayAtPoint(new AudioSourceSettings(), location, volume, group, looped);
        }

        public virtual ArdenAudioInstance Play(float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return Play(new AudioSourceSettings(), volume, group, looped);
        }

        public virtual ArdenAudioInstance PlayAtPoint(AudioSourceSettings settings,Vector3 location, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return null;
        }

        public virtual ArdenAudioInstance Play(AudioSourceSettings settings, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return null;
        }


    }

    public class ArdenAudioInstance
    {
        //Reference to asset tied to this isntance
        public ArdenAudioAsset asset;
        public AudioSourceSettings sourceSettings;

        protected bool fadingOut = false;
        protected bool fadingIn = false;

        public bool completed = false;
        public bool stopped = false;

        //Internal access to volume
        protected float _volume { get { return 0; } set { } }

        protected Vector3 position;

        public delegate void OnCompleteAction();
        public event OnCompleteAction OnComplete;

        private Coroutine fadingCoroutine;

        public ArdenAudioInstance(ArdenAudioAsset asset,AudioSourceSettings sourceSettings)
        {
            this.asset = asset;
            this.sourceSettings = sourceSettings;
        }

        public virtual void RandomTime()
        {

        }

        public virtual Vector3 GetPosition()
        {
            return position;
        }

        public virtual void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public virtual float GetVolume()
        {
            return _volume;
        }

        public virtual void SetVolume(float volume)
        {
            //Do not allow setting of volume externally when fading in/out
            //        if (fadingOut || fadingIn)
            //            return;

            _volume = volume;
        }

        //Stops and disposes audio
        public virtual void Stop()
        {
            if (completed)
                return;

            stopped = true;

            if (fadingCoroutine != null)
                Utility.CoroutineManager.StopACoroutine(fadingCoroutine);

        }

        protected virtual void _OnComplete()
        {
            if (stopped)
                return;
            completed = true;
            if (OnComplete != null)
                OnComplete();
        }

        public virtual void FadeOut(float time)
        {
            if (fadingCoroutine != null)
                Utility.CoroutineManager.StopACoroutine(fadingCoroutine);

            fadingCoroutine = Utility.CoroutineManager.RunExternalCoroutine(_Fade(0, GetVolume() / time));

            //For now, just instantly "fade out"
         //   SetVolume(0);
      //      Stop();
        }

        private System.Collections.IEnumerator _Fade(float target,float speed)
        {
            float volume = GetVolume();

            if (target < volume)
            {
                while (target < volume)
                {
                    volume -= speed * Time.deltaTime;
                    SetVolume(volume);
                    yield return null;
                }
            }
            else if (target > volume)
            {
                while (target > volume)
                {
                    volume += speed * Time.deltaTime;
                    SetVolume(volume);
                    yield return null;
                }
            }

            if(target == 0)
                Stop();

        }

        public virtual void FadeIn(float time)
        {
            if (fadingCoroutine != null)
                Utility.CoroutineManager.StopACoroutine(fadingCoroutine);

            fadingCoroutine = Utility.CoroutineManager.RunExternalCoroutine(_Fade(1, 1 / time));

            //For now, just instantly "fade in"
            //SetVolume(1);
        }
    }

}