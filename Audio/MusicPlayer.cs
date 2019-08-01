using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Ardenfall.Audio
{
    //Manages a single layer of music
    public class MusicPlayer
    {
        private List<ArdenAudioClip> songs = new List<ArdenAudioClip>();

        private ArdenAudioClip currentSong;
        private PooledAudioSource currentSongSource;

        private bool isPlaying = false;

        private float silenceChance;
        private float silenceMinLength;
        private float silenceMaxLength;
        private Coroutine silenceCoroutine;

        public MusicPlayer()
        {

        }

        public MusicPlayer(float silenceChance, float silenceMinLength, float silenceMaxLength)
        {
            this.silenceChance = silenceChance;
            this.silenceMinLength = silenceMinLength;
            this.silenceMaxLength = silenceMaxLength;
        }

        public ArdenAudioClip CurrentAudioClip
        {
            get => currentSong;
        }

        public void Play()
        {
            if(!isPlaying)
            {
                isPlaying = true;
                PlayNextSong();
            }
        }

        public void Stop()
        {
            isPlaying = false;

            if (currentSongSource != null)
            {
                currentSongSource.onComplete -= OnCurrentSongComplete;
                SoundManager.StopPooledClip(currentSongSource);
            }

            currentSong = null;
            currentSongSource = null;
        }

        public void FadeIn(float speed)
        {
            Play();
        }

        public void FadeOut(float speed)
        {
            Stop();
        }

        /// <summary>
        /// Adds songs without interfiering with the current song
        /// </summary>
        /// <param name="songs"></param>
        public void AddSongs(List<ArdenAudioClip> addSongs)
        {
            foreach(ArdenAudioClip clip in addSongs)
            {
                if (!songs.Contains(clip))
                    songs.Add(clip);
            }
        }

        /// <summary>
        /// Removes songs, if current song is removed it will be stopped
        /// </summary>
        /// <param name="songs"></param>
        public void RemoveSongs(List<ArdenAudioClip> removeSongs)
        {
            foreach (ArdenAudioClip clip in removeSongs)
            {
                if (songs.Contains(clip))
                    songs.Remove(clip);
            }

            if (!songs.Contains(currentSong) && isPlaying)
                PlayNextSong();
        }

        /// <summary>
        /// Removes songs and adds new ones, if the current song is to be removed and 
        /// is in the new songs list it will not be stopped
        /// </summary>
        /// <param name="addSongs"></param>
        /// <param name="removeSongs"></param>
        public void RemoveAndAddSongs(List<ArdenAudioClip> addSongs, List<ArdenAudioClip> removeSongs)
        {
            if(removeSongs != null)
            {
                foreach (ArdenAudioClip clip in removeSongs)
                {
                    if (songs.Contains(clip))
                        songs.Remove(clip);
                }
            }
            
            if(addSongs != null)
            {
                foreach (ArdenAudioClip clip in addSongs)
                {
                    if (!songs.Contains(clip))
                        songs.Add(clip);
                }
            }
            

            if(!songs.Contains(currentSong) && isPlaying)
                PlayNextSong();
        }

        private void PlayNextSong()
        {
            if (currentSongSource != null)
            {
                currentSongSource.onComplete -= OnCurrentSongComplete;
                SoundManager.StopPooledClip(currentSongSource);
            }

            currentSong = null;
            currentSongSource = null;

            PlaySong(PickSong());
        }

        private void PlaySong(ArdenAudioClip clip)
        {
            if (clip == null)
            {
                PlaySilence();
            }
            else
            {
                currentSong = clip;

                currentSongSource = SoundManager.PlayAudio(clip, MixerGroup.music, 1, true);
                currentSongSource.onComplete += OnCurrentSongComplete;
            }
            
        }

        private void PlaySilence()
        {
            float silenceLength = Random.Range(silenceMinLength, silenceMaxLength);
            
            IEnumerator PlaySilenceCoroutine()
            {
                yield return new WaitForSecondsRealtime(silenceLength);

                if(isPlaying)
                    OnCurrentSongComplete();
            }

            silenceCoroutine = Utility.CoroutineManager.RunExternalCoroutine(PlaySilenceCoroutine());
        }

        private void OnCurrentSongComplete()
        {
            currentSong = null;
            currentSongSource = null;
            silenceCoroutine = null;

            //Pick next song
            PlaySong(PickSong());
        }

        private ArdenAudioClip PickSong()
        {
            if (songs.Count == 0)
                return null;

            //Silence
            if(silenceChance != 0)
            {
                if (Random.value <= silenceChance)
                    return null;
            }

            int r = Random.Range(0, songs.Count);
            return songs[r];
        }
    }

}