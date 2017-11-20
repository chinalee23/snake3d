using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfig : MonoBehaviour {
    public PlaneType pt;

    public void Save() {
        Config.Save(pt, transform.position, transform.localEulerAngles);
    }

    public void Clear() {
        Config.Clear();
    }

    public void Load() {
        Config.Load();
    }
}
