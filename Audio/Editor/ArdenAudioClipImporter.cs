
using UnityEngine;
using UnityEditor;
using Ardenfall.Audio;

namespace ArdenfallEditor.Audio
{
    public class ArdenAudioClipGenerator
    {
        [MenuItem("Ardenfall/Tools/SwitchArdenToWav", priority = 1001)]
        public static void SwitchToWav()
        {
            int count = 0;

            foreach (string s in AssetDatabase.GetAllAssetPaths())
            {
                Ardenfall.ArdenAudioClip asset = AssetDatabase.LoadMainAssetAtPath(s) as Ardenfall.ArdenAudioClip;
                if (asset == null)
                    continue;

                string audiofilepath = AssetDatabase.GetAssetPath(asset.audioClip.GetInstanceID());

                if (audiofilepath.Contains(".wav"))
                    continue;

                string goalpath = audiofilepath.Replace(".mp3", ".wav").Replace(".ogg", ".wav");

                AudioClip goalClip = AssetDatabase.LoadMainAssetAtPath(goalpath) as AudioClip;

                if (goalClip == null)
                    continue;

                asset.audioClip = goalClip;
                EditorUtility.SetDirty(asset);

                AssetDatabase.DeleteAsset(audiofilepath);

                count++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Switched " + count + " assets");
        }

        [MenuItem("Assets/Create Arden AudioClip", priority = 1001)]
        public static void ConvertToArdenAudio()
        {
            foreach (Object sel in Selection.objects)
            {
                if (sel as AudioClip != null)
                    Convert((AudioClip)sel);
            }
        }

        [MenuItem("Assets/Create Arden AudioClip", validate = true)]
        public static bool ConvertToArdenAudioValidate()
        {
            foreach (Object sel in Selection.objects)
            {
                if (sel as AudioClip == null)
                    return false;
            }
            return true;
        }
        /*
        [MenuItem("Assets/Utility/Convert To Unity AudioClip",priority = 1002)]
        public static void ConvertToUnityAudio()
        {
            foreach (Object sel in Selection.objects)
            {
                if (sel as Ardenfall.ArdenAudioClip != null)
                    Convert((Ardenfall.ArdenAudioClip)sel);
            }
        }

        [MenuItem("Assets/Utility/Convert To Unity AudioClip", validate = true)]
        public static bool ConvertToUnityAudioValidate()
        {
            foreach (Object sel in Selection.objects)
            {
                if (sel as Ardenfall.ArdenAudioClip == null)
                    return false;
            }
            return true;
        }*/

        private static void Convert(Ardenfall.ArdenAudioClip clip)
        {
            string assetPath = AssetDatabase.GetAssetPath(clip.GetInstanceID());

            if (clip.audioClip != null)
            {
                clip.audioClip.hideFlags = HideFlags.None;
                EditorUtility.SetDirty(clip.audioClip);
            }

            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.Refresh();
        }

        private static void Convert(AudioClip clip)
        {
            string assetPath = AssetDatabase.GetAssetPath(clip.GetInstanceID());
            string newPath = System.IO.Path.ChangeExtension(assetPath, ".asset");

            //Check if we have a asset here already. If so, do nothing
            if (AssetDatabase.LoadAssetAtPath(newPath, typeof(Ardenfall.ArdenAudioClip)) != null)
                return;

            Ardenfall.ArdenAudioClip clipWrapper = ScriptableObject.CreateInstance<Ardenfall.ArdenAudioClip>();
            clipWrapper.audioClip = clip;

            AssetDatabase.CreateAsset(clipWrapper, newPath);

            clip.hideFlags = HideFlags.HideInHierarchy;
            EditorUtility.SetDirty(clip);

            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }
    }
}
