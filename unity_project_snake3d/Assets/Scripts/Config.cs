using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Config : MonoBehaviour {
    public static Config Instance;

    public GameObject PrefabHead;
    public GameObject PrefabBody;
    public GameObject PrefabTail;

    public static Dictionary<PlaneType, Vector3[]> CameraConfig;

	void Awake() {
        Instance = this;
        loadCameraConfig();
    }

    static void loadCameraConfig() {
        CameraConfig = new Dictionary<PlaneType, Vector3[]>();
        string path = Application.dataPath + "/../config/camera";
        using (StreamReader sr = new StreamReader(path)) {
            string s;
            while ((s = sr.ReadLine()) != null) {
                string[] seps = s.Split(',');
                int type = int.Parse(seps[0].Trim());
                string[] pos = seps[1].Trim().Split(' ');
                Vector3 cPos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                string[] angle = seps[2].Trim().Split(' ');
                Vector3 cAngle = new Vector3(float.Parse(angle[0]), float.Parse(angle[1]), float.Parse(angle[2]));
                CameraConfig[(PlaneType)type] = new Vector3[] { cPos, cAngle };
            }
        }
    }

    public static void Save(PlaneType pt, Vector3 pos, Vector3 angle) {
        if (CameraConfig == null) {
            loadCameraConfig();
        }

        if (!CameraConfig.ContainsKey(pt)) {
            CameraConfig[pt] = new Vector3[2];
        }
        CameraConfig[pt][0] = pos;
        CameraConfig[pt][1] = angle;
        string path = Application.dataPath + "/../config/camera";
        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8)) {
            foreach (var kv in CameraConfig) {
                string s = string.Format("{0}, {1} {2} {3}, {4} {5} {6}", (int)kv.Key, kv.Value[0].x, kv.Value[0].y, kv.Value[0].z, kv.Value[1].x, kv.Value[1].y, kv.Value[1].z);
                sw.WriteLine(s);
            }
        }

        Debug.Log("save ok...");
    }

    public static void Clear() {
        if (CameraConfig != null) {
            CameraConfig.Clear();
        }
        
        string path = Application.dataPath + "/../config/camera";
        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8)) {
        }
    }
}
