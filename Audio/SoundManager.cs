using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ardenfall.Utility;
using UnityEngine.Audio;

namespace Ardenfall
{
    public enum MixerGroup
    {
        music, sfx, voice, ambient, ui, footstep, master
    }

    public class PooledAudioSource
    {
        public bool gameTimebased = false;

        public bool complete;
        public Coroutine coroutine;
        public System.Action onComplete;
        
        public AudioSource audioSource;
        public Transform attached;

        public PooledAudioSource()
        {
        }

        public void Update()
        {
            if(attached != null)
            {
                audioSource.gameObject.transform.position = attached.position;
            }
        }

        public void Dispose()
        {
            attached = null;
        }

        public void OnPauseToggle(bool paused)
        {
            if (complete)
                return;

            if(gameTimebased)
            {
                if (paused)
                    audioSource.Pause();
                else
                    audioSource.UnPause();
            }
        }

        public void RandomTime()
        {
            //Can't make time too close to the end
            float max = audioSource.clip.length - 0.5f;
            if (max < 0) max = 0;

            audioSource.time = Random.Range(0, max);
        }
    }

    [System.Serializable]
    public class AudioSourceSettings
    {
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        public float minDistance = 1;
        public float maxDistance = 500;
    }

    //Pools Audio
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        [System.Serializable]
        private struct MixerGroupObject
        {
            public MixerGroup group;
            public AudioMixerGroup asset;
        }

        [SerializeField]
        private List<MixerGroupObject> mixerObjects;

        //Static pool
        private List<AudioSource> audioPool = new List<AudioSource>();
        private List<PooledAudioSource> playingAudio = new List<PooledAudioSource>();

        private GameObject audioPoolObject;
        private int maxAudioPool = 30;

        //Define's the engine's default settings.
        protected static void ApplyDefaultSettings(AudioSource source)
        {
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.maxDistance = 500;
            source.minDistance = 0;
        }

        protected void Update()
        {
            for(int i =0;i< playingAudio.Count;i++)
            {
                playingAudio[i]?.Update();
            }    
        }

        public int GetActiveCount()
        {
            return playingAudio.Count;
        }

        public int GetPooledCount()
        {
            return audioPool.Count;
        }

        //Run when the world or main menu is switched
        private void DisposeOfAllSounds()
        {
            if (playingAudio == null)
                return;

            //Dispose of all sounds
            for (int i = 0; i < playingAudio.Count; i++)
            {
                DisposeOfSource(playingAudio[i]);
                i--;
            }
        }

        //Returns a mixer group via the enum
        public static AudioMixerGroup GetMixerGroup(MixerGroup group)
        {
            foreach (MixerGroupObject g in Instance.mixerObjects)
            {
                if (g.group == group)
                    return g.asset;
            }
            return null;
        }

        //Force stop a specific source
        public static void StopPooledClip(PooledAudioSource source)
        {
            if (source == null)
                return;

            if (source.complete)
                return;

            if (Instance.playingAudio.Contains(source))
            {
                //Stop coroutine
                if (source.coroutine != null)
                    CoroutineManager.StopACoroutine(source.coroutine);
                
                Instance.DisposeOfSource(source);
            }

        }

        //Play 2D audio source by randomly selecting from list
        public static PooledAudioSource PlayAudio(List<ArdenAudioClip> clips, MixerGroup group = MixerGroup.ui,float volume=1, bool looped = false, AudioSourceSettings settings = null)
        {
            if (clips == null) return null;

            if (clips.Count == 0) return null;

            if (clips.Count == 1)
                return PlayPooledClip(clips[0].audioClip, group, clips[0].volume * volume, clips[0].GetPitch(), looped, settings);

            int r = UnityEngine.Random.Range(0, clips.Count);

            return PlayPooledClip(clips[r].audioClip, group, clips[r].volume * volume, clips[r].GetPitch(), looped, settings);
        }

        public static PooledAudioSource PlayAudio(ArdenAudioClip clip, MixerGroup group = MixerGroup.ui, float volume = 1, bool looped=false, AudioSourceSettings settings = null)
        {
            if (clip == null) return null;

            return PlayPooledClip(clip.audioClip, group, clip.volume * volume, clip.GetPitch(), looped, settings);
        }

        //Play a random sound at a position selected from a list
        public static PooledAudioSource PlayAudioAtTransform(List<ArdenAudioClip> clips, Transform transform, MixerGroup group = MixerGroup.sfx, float volume = 1, bool looped = false, AudioSourceSettings settings = null)
        {
            if (clips == null) return null;

            if (clips.Count == 0) return null;

            PooledAudioSource source = null;

            if (clips.Count == 1)
                source = PlayPooledClip(clips[0].audioClip, transform.position, group, clips[0].volume * volume, clips[0].GetPitch(), looped, settings);
            else
            {
                int r = UnityEngine.Random.Range(0, clips.Count);
                source = PlayPooledClip(clips[r].audioClip, transform.position, group, clips[r].volume * volume, clips[r].GetPitch(), looped, settings);
            }

            if(source != null)
            {
                source.attached = transform;
                source.Update();
            }
                

            return source;
        }

        public static PooledAudioSource PlayAudioAtTransform(ArdenAudioClip clip, Transform transform, MixerGroup group = MixerGroup.ui, float volume = 1, AudioSourceSettings settings = null, bool looped = false)
        {
            if (clip == null) return null;

            PooledAudioSource source = PlayPooledClip(clip.audioClip, transform.position, group, clip.volume * volume, clip.GetPitch(), looped, settings);

            if (source != null)
            {
                source.attached = transform;
                source.Update();
            }

            return source;
        }

        //Play a random sound at a position selected from a list
        public static PooledAudioSource PlayAudioAtPoint(List<ArdenAudioClip> clips, Vector3 position, MixerGroup group = MixerGroup.sfx, float volume=1, bool looped=false, AudioSourceSettings settings = null)
        {
            if (clips == null) return null;

            if (clips.Count == 0) return null;

            if (clips.Count == 1)
                PlayPooledClip(clips[0].audioClip, position, group, clips[0].volume * volume, clips[0].GetPitch(), looped, settings);

            int r = UnityEngine.Random.Range(0, clips.Count);

            return PlayPooledClip(clips[r].audioClip, position, group,clips[r].volume * volume, clips[r].GetPitch(), looped, settings);
        }

        public static PooledAudioSource PlayAudioAtPoint(ArdenAudioClip clip, Vector3 position, MixerGroup group = MixerGroup.ui, float volume=1, AudioSourceSettings settings = null, bool looped = false)
        {
            if (clip == null) return null;

            return PlayPooledClip(clip.audioClip, position, group, clip.volume * volume, clip.GetPitch(), looped, settings);
        }

        public static PooledAudioSource PlayPooledClip(AudioClip clip, MixerGroup group = MixerGroup.ui, float volume = 1, float pitch = 1,bool loop = false, AudioSourceSettings settings = null)
        {
            return PlayPooledClip(clip, null, group, volume, pitch, loop, settings);
        }

        //Play a sound at a position
        public static PooledAudioSource PlayPooledClip(AudioClip clip, Vector3? position, MixerGroup group = MixerGroup.sfx, float volume = 1, float pitch = 1,bool loop = false, AudioSourceSettings settings = null)
        {
            if (Instance.audioPool == null)
                Instance.audioPool = new List<AudioSource>();

            if (Instance.audioPoolObject == null)
            {
                Instance.audioPoolObject = new GameObject("__Audio Pool (Singleton)");
                GameObject.DontDestroyOnLoad(Instance.audioPoolObject);
            }

            if (clip == null)
                return null;

            if (settings == null)
                settings = new AudioSourceSettings();

            AudioSource audioSource = null;

            if(position == null)
                audioSource = Instance.GetAudioSource();
            else
                audioSource = Instance.GetAudioSource((Vector3)position);

            audioSource.outputAudioMixerGroup = GetMixerGroup(group);

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.name = clip.name;
            audioSource.time = 0;
            
            if (pitch > 0)
                audioSource.pitch = pitch;
            else
                audioSource.pitch = 0.01f;

            audioSource.Play();

            audioSource.rolloffMode = settings.rolloffMode;
            audioSource.maxDistance = settings.maxDistance;
            audioSource.minDistance = settings.minDistance;

            PooledAudioSource audioSourceData = new PooledAudioSource();
            audioSourceData.audioSource = audioSource;
            audioSourceData.complete = false;

            //By default, anything not music, ambient, or UI will be affected by time
            if (group != MixerGroup.ambient && group != MixerGroup.music && group != MixerGroup.ui)
                audioSourceData.gameTimebased = true;

            if (loop == false)
            {
                IEnumerator WaitTillAudioComplete()
                {
                    yield return new WaitWhile(() =>
                    {
                        if (audioSource == null)
                            return false;
                        return audioSource.isPlaying;
                    });

                    if (audioSourceData.onComplete != null)
                    {
                        audioSourceData.onComplete();
                    }
                    Instance.OnAudioSourceDone(audioSourceData);
                }

                audioSourceData.coroutine = CoroutineManager.RunExternalCoroutine(WaitTillAudioComplete());
            }

            Instance.playingAudio.Add(audioSourceData);

            return audioSourceData;
        }

        private void OnAudioSourceDone(PooledAudioSource source)
        {
            DisposeOfSource(source);
        }

        private void DisposeOfSource(PooledAudioSource data)
        {
            //Search for source in list and remove
            for (int i = 0; i < playingAudio.Count; i++)
            {
                if (playingAudio[i].Equals(data))
                {
                    //Label complete and dispose
                    playingAudio[i].complete = true;
                    playingAudio.RemoveAt(i);
                }
            }

            data.Dispose();

            AudioSource source = (AudioSource)data.audioSource;

            if (source == null)
                return;

            source.Stop();
            source.gameObject.SetActive(false);

            if (audioPool.Count > maxAudioPool)
                Destroy(source.gameObject);
            else
                audioPool.Add(source);
        }

        private AudioSource GetAudioSource()
        {
            AudioSource source = GetAudioSourceFromPool();

            if (source == null)
                source = SpawnAudioSource();

            return source;
        }

        private AudioSource GetAudioSource(Vector3 position)
        {
            AudioSource source = GetAudioSourceFromPool(position);

            if (source == null)
                source = SpawnAudioSource(position);

            return source;
        }

        private AudioSource SpawnAudioSource()
        {
            GameObject audioObject = new GameObject("Audio Clip");
            audioObject.transform.parent = audioPoolObject.transform;
            AudioSource clip = audioObject.AddComponent<AudioSource>();
            clip.spatialBlend = 0;
            return clip;
        }

        private AudioSource SpawnAudioSource(Vector3 position)
        {
            GameObject audioObject = new GameObject("Audio Clip");
            audioObject.transform.parent = audioPoolObject.transform;
            audioObject.transform.position = position;
            AudioSource clip = audioObject.AddComponent<AudioSource>();
            clip.spatialBlend = 1;

            return clip;
        }

        private AudioSource GetAudioSourceFromPool()
        {
            if (audioPool.Count == 0)
                return null;

            AudioSource source = audioPool[0];
            audioPool.RemoveAt(0);

            //Source is null, return next source
            if (source == null)
                return GetAudioSourceFromPool();

            source.gameObject.SetActive(true);
            source.spatialBlend = 0;

            return source;
        }

        private AudioSource GetAudioSourceFromPool(Vector3 position)
        {
            if (audioPool.Count == 0)
                return null;

            AudioSource source = audioPool[0];

            audioPool.RemoveAt(0);


            if (source == null)
                return null;

            source.transform.position = position;
            source.gameObject.SetActive(true);
            source.spatialBlend = 1;

            return source;
        }

    }

}
