using UnityEngine;
using UnityEditor;
using System.Collections;
using Ardenfall.Audio;

namespace ArdenfallEditor.Audio
{
    [CustomEditor(typeof(MaterialSoundObject)),CanEditMultipleObjects]
    public class MaterialSoundObjectInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

        }

    }

    [CustomEditor(typeof(ArdenAudioVolume)), CanEditMultipleObjects]
    public class ArdenAudioVolumeEdtor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

        }

    }
}
