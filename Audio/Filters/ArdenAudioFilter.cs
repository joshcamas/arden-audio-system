using System.Collections.Generic;
using UnityEngine;

namespace Ardenfall
{
    public class ArdenAudioFilterInstance
    {
        private GameObject currentGameObject;
        private List<Component> currentComponents;

        public bool IsAttached(GameObject gameObject)
        {
            return (currentGameObject == gameObject);
        }

        public void Attach(GameObject gameObject, List<Component> components)
        {
            //Detach first
            if(currentGameObject != null)
            {
                Detach();
            }

            currentGameObject = gameObject;
            currentComponents = components;
        }

        public void Detach()
        {
            if(currentComponents != null)
            {
                foreach (Component c in currentComponents)
                    GameObject.Destroy(c);
            }

            currentGameObject = null;
            currentComponents = null;
        }
    }

    public class ArdenAudioFilter : ScriptableObject
    {
        public bool stack = true;

        private Dictionary<GameObject, List<ArdenAudioFilterInstance>> instances;

        //Usually this is the only function we really need to use
        //Just run "AddComponentToObject" for whatever components we add during this time
        public virtual bool AttachToObject(GameObject gameObject, ArdenAudioFilterInstance instance)
        {
            return true;
        }

        protected void RegisterInstance(GameObject gameObject,ArdenAudioFilterInstance instance)
        {
            if (instances == null)
                instances = new Dictionary<GameObject, List<ArdenAudioFilterInstance>>();

            if (!instances.ContainsKey(gameObject))
                instances.Add(gameObject, new List<ArdenAudioFilterInstance>());

            instances[gameObject].Add(instance);
        }

        protected void UnregisterInstance(GameObject gameObject, ArdenAudioFilterInstance instance)
        {
            instance.Detach();
            instances[gameObject].Remove(instance);
        }
    }

}