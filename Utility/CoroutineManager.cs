using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ardenfall.Utility
{

    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _instance;

        private static CoroutineManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "CoroutineManager";
                    _instance = obj.AddComponent<CoroutineManager>();
                }
                return _instance;
            }
        }

        public static void StopACoroutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        public static Coroutine RunExternalCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        public static Coroutine RunDelayingCoroutine(float delay,Action onComplete, bool unscaled=false)
        {
            IEnumerator coroutine = Instance.DelayingCoroutine(delay, unscaled, onComplete);
            return Instance.StartCoroutine(coroutine);
        }

        public static Coroutine RunFrameCoroutine(Action onComplete)
        {
            return Instance.StartCoroutine("FrameCoroutine", onComplete);
        }

        private IEnumerator FrameCoroutine(Action onComplete)
        {
            //Wait for one frame
            yield return null;

            if (onComplete != null)
                onComplete();
        }

        private IEnumerator DelayingCoroutine(float delay, bool unscaled,Action onComplete)
        {
            if (unscaled)
                yield return new WaitForSecondsRealtime(delay);
            else 
                yield return new WaitForSeconds(delay);

            if (onComplete != null)
                onComplete();
        }

    }

}
