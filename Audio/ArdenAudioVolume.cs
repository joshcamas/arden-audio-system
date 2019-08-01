using UnityEngine;
using System.Collections;

namespace Ardenfall.Audio
{
    public class ArdenAudioVolume : MonoBehaviour
    {
        public ArdenAudioAsset asset;
        public Collider colliderVolume;

        public MixerGroup group = MixerGroup.ambient;

        public AudioSourceSettings settings;

        private ArdenAudioInstance instance;
        private bool isQuitting = false;

        public void Start()
        {
            if (colliderVolume == null)
                colliderVolume = GetComponent<Collider>();

            instance = asset.PlayAtPoint(settings,new Vector3(), 1, group, true);

        }

        public void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(true);
        }

        private void DrawGizmos(bool selected)
        {
            if (colliderVolume == null)
                colliderVolume = GetComponent<Collider>();

            Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            Gizmos.color = selected? new Color(0.3f, 0.3f, 0.3f) : new Color(0.1f, 0.1f, 0.1f);

            if (colliderVolume != null)
            {
                //Draw sphere collider
                if (colliderVolume as SphereCollider != null)
                {
                    SphereCollider sphere = (SphereCollider)colliderVolume;
                    Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                }
                //Draw box collider
                else if (colliderVolume as BoxCollider != null)
                {
                    BoxCollider box = (BoxCollider)colliderVolume;
                    Gizmos.DrawWireCube(box.center, box.size);
                }
                //Draw bounds
                else
                    Gizmos.DrawWireCube(colliderVolume.bounds.center - transform.position, colliderVolume.bounds.size);

            }

            Gizmos.DrawIcon(transform.position, "AudioVolumeGizmo.png", true);

            Gizmos.color = Color.white;
            Gizmos.matrix = oldGizmosMatrix;
        }

        public void Update()
        {
            //TODO: Using Camera.main is a foolish choice
            Camera camera = Camera.main;

            if (camera == null)
                return;

            if (instance == null)
                return;

            Vector3 location = Physics.ClosestPoint(camera.gameObject.transform.position, colliderVolume, colliderVolume.transform.position, colliderVolume.transform.rotation);

            instance.SetPosition(location);
        }

        public void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public void OnDestroy()
        {
            if (isQuitting)
                return;

            if(instance != null)
                instance.Stop();
        }
    }
}