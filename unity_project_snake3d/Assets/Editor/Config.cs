using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraConfig))]
public class CameraConfigEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CameraConfig cc = (CameraConfig)target;
        if (GUILayout.Button("save")) {
            cc.Save();
        }
        if (GUILayout.Button("clear")) {
            cc.Clear();
        }
        if (GUILayout.Button("reload")) {
            cc.Load();
        }
    }
}
