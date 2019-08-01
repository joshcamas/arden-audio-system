using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall.Audio
{
    [System.Serializable,CreateAssetMenu(menuName ="Ardenfall/Audio/Playlist")]
    public class ArdenAudioPlaylist : ArdenAudioAsset
    {
        public List<ArdenAudioAsset> audioAssets;
        public float crossFadeSpeed = 2;

        public bool shuffle = true;

        public override ArdenAudioInstance Play(AudioSourceSettings settings, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return Play_Internal(settings, volume, group, looped,false,new Vector3());
        }

        public override ArdenAudioInstance PlayAtPoint(AudioSourceSettings settings,Vector3 location, float volume = 1, MixerGroup group = MixerGroup.sfx, bool looped = false)
        {
            return Play_Internal(settings,volume, group, looped, true, location);
        }

        //A single function to unify play and play at point, under the hood
        private ArdenAudioInstance Play_Internal(AudioSourceSettings settings, float volume, MixerGroup group, bool looped, bool playAtPoint, Vector3 location)
        {
            if (audioAssets == null)
                return null;

            if (audioAssets.Count == 0)
                return null;

            ArdenAudioPlayListInstance instance = new ArdenAudioPlayListInstance(this, settings);
            instance.Init(volume, group, looped, playAtPoint, location);

            return instance;
        }

    }

    public class ArdenAudioPlayListInstance : ArdenAudioInstance
    {
        
        private MixerGroup group;
        private bool looped;
        private bool playAtPoint;

        private float volume;

        private ArdenAudioInstance currentChild;
        private ArdenAudioInstance fadingOutChild;

        private List<int> playedChildren;

        private ArdenAudioPlaylist Playlist { get { return (ArdenAudioPlaylist)asset; } }

        public override void SetVolume(float volume)
        {
            this._volume = volume;
            if (currentChild != null)
                currentChild.SetVolume(volume);
        }

        public override void SetPosition(Vector3 position)
        {
            this.position = position;
            currentChild.SetPosition(position);
        }


        public ArdenAudioPlayListInstance(ArdenAudioAsset asset,AudioSourceSettings settings) : base(asset,settings) { }

        public void Init(float volume, MixerGroup group, bool looped, bool playAtPoint, Vector3 position)
        {
            
            this.volume = volume;
            this.group = group;
            this.looped = looped;
            this.playAtPoint = playAtPoint;
            this.position = position;

            playedChildren = new List<int>();

            //Start playing
            NextChild();
        }

        private void FadeOutCurrentChild(float time)
        {
            if (currentChild == null)
                return;

            //Swap children
            ArdenAudioInstance oChild = fadingOutChild;
            fadingOutChild = currentChild;
            currentChild = oChild;

            //If we've already been fading something out, then just force stop it
            if (currentChild != null)
            {
                currentChild.Stop();
                currentChild = null;
            }

           fadingOutChild.FadeOut(time);

        }

        private int GetNextChildIndex()
        {
            //Detect single song in loop mode
            if (Playlist.audioAssets.Count == 1 && looped)
                return 0;

            //Simple next selection
            if(Playlist.shuffle)
            {
                //Detect if end of playlist has been found
                if (playedChildren.Count >= Playlist.audioAssets.Count)
                {
                    //No looping, return -1
                    if (!looped)
                        return -1;

                    //Clear, and add the current child to the stack (if it's playing)
                    playedChildren = new List<int>();

                    if (currentChild != null)
                        playedChildren.Add(Playlist.audioAssets.IndexOf(currentChild.asset));
                }

                int index = -1;
                bool foundNewSong = false;

                //Max Catch
                int maxRan = Playlist.audioAssets.Count * 2;

                while (!foundNewSong && maxRan > 0)
                {
                    index = UnityEngine.Random.Range(0, Playlist.audioAssets.Count);

                    if (!playedChildren.Contains(index))
                        foundNewSong = true;

                    maxRan--;
                }

                if (index == -1)
                    index = 0;

                return index;
            }
            else
            {
                int index = 0;
                if (currentChild != null)
                    index = Playlist.audioAssets.IndexOf(currentChild.asset) + 1;

                //Hit max
                if (index >= Playlist.audioAssets.Count)
                {
                    //If looped, reset index and start again
                    if (looped)
                        index = 0;
                    else
                    {
                        //Otherwise, we're done here
                        return -1;
                    }
                }
                return index;
            }

        }

        private void PlayChild(int index)
        {
            //Fade current song out 
            if (currentChild != null)
                FadeOutCurrentChild(Playlist.crossFadeSpeed);

            if (playAtPoint)
                currentChild = Playlist.audioAssets[index].PlayAtPoint(sourceSettings,position, volume, group, false);
            else
                currentChild = Playlist.audioAssets[index].Play(sourceSettings, volume, group, false);

            if (!playedChildren.Contains(index))
                playedChildren.Add(index);

           // currentChild.FadeIn(Playlist.crossFadeSpeed);

            //Hook children's completion with playlist
            currentChild.OnComplete += NextChild;

        }

        private void NextChild()
        {

            //Remove hook
            if (currentChild != null)
                currentChild.OnComplete -= NextChild;

            int childIndex = GetNextChildIndex();

            //Out of songs
            if(childIndex == -1)
            {
                Stop();
                return;
            }

            PlayChild(childIndex);
            
        }

        public override void Stop()
        {
            if (completed)
                return;

            base.Stop();

            if (currentChild != null)
                currentChild.Stop();

            if (fadingOutChild != null)
                fadingOutChild.Stop();

            currentChild = null;
            fadingOutChild = null;

            //Stop any coroutines as well
        }

    }
}