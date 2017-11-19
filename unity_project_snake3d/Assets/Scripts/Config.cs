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

    public static Dictionary<PlaneType, Vector3> CameraConfig;

	void Awake() {
        Instance = this;
        loadCameraConfig();
    }

    static void loadCameraConfig() {
        CameraConfig = new Dictionary<PlaneType, Vector3>();
        string path = Application.dataPath + "/../config/camera";
        using (StreamReader sr = new StreamReader(path)) {
            string s;
            while ((s = sr.ReadLine()) != null) {
                string[] seps = s.Split(',');
                int type = int.Parse(seps[0].Trim());
                string[] pos = seps[1].Trim().Split(' ');
                Vector3 cPos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                CameraConfig[(PlaneType)type] = cPos;
            }
        }
    }

    public static void Save(PlaneType pt, Vector3 v) {
        if (CameraConfig == null) {
            loadCameraConfig();
        }

        CameraConfig[pt] = v;
        string path = Application.dataPath + "/../config/camera";
        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8)) {
            foreach (var kv in CameraConfig) {
                string s = string.Format("{0}, {1} {2} {3}", (int)kv.Key, kv.Value.x, kv.Value.y, kv.Value.z);
                sw.WriteLine(s);
            }
        }

        Debug.Log("save ok...");
    }
}
