using UnityEngine;
using System.Collections.Generic;

namespace Ardenfall
{
    public class AudioEffectVolume : MonoBehaviour
    {
        public BoxCollider boxCollider;
        public List<ArdenAudioFilter> filters;

        private List<AudioListener> listenersInside = new List<AudioListener>();
        private List<ArdenAudioFilterInstance> filterInstances = new List<ArdenAudioFilterInstance>();

        private Camera cachedCamera;
        private AudioListener cachedListener;

        private AudioListener CurrentListener
        {
            get
            {
                if(cachedCamera != Camera.main)
                {
                    cachedCamera = Camera.main;

                    if (cachedCamera != null)
                        cachedListener = cachedCamera.GetComponent<AudioListener>();
                    else
                        cachedListener = null;

                }

                return cachedListener;
            }
        }

        public void Start()
        {
            filterInstances = new List<ArdenAudioFilterInstance>();

            foreach(ArdenAudioFilter f in filters)
            {
                filterInstances.Add(new ArdenAudioFilterInstance());
            }
        }
        public void Update()
        {
            if(CurrentListener != null)
            {
                if (boxCollider.bounds.Contains(CurrentListener.transform.position))
                {
                    OnEnter(CurrentListener);
                } else
                {
                    OnExit(CurrentListener);
                }
            }
        }

        private void OnEnter(AudioListener listener) 
        {
            if (listenersInside.Contains(CurrentListener))
                return;

/*            ArdenAudioListener ardenListener = CameraManager.CurrentListener.GetComponent<ArdenAudioListener>();

            if (ardenListener == null)
                return;
                */

            bool success = true;

            //Add all filters - if at least one fails, do not add to list
            int i = 0;
            foreach (ArdenAudioFilter f in filters)
            {
                //             ardenListener.AttachFilter(f);
                if (f.AttachToObject(listener.gameObject, filterInstances[i]) == false)
                {
                    success = false;
                }
            }

            if (success)
                listenersInside.Add(listener);
        }

        private void OnExit(AudioListener listener)
        {
            if (!listenersInside.Contains(CurrentListener))
                return;

            listenersInside.Remove(listener);

            int i = 0;
            foreach (ArdenAudioFilter f in filters)
            {
                filterInstances[i].Detach();
            }
        }
    }

}