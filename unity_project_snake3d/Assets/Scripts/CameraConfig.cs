using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfig : MonoBehaviour {
    public GameObject snake;
    public PlaneType pt;

    public void Save() {
        if (snake != null) {
            Config.Save(pt, transform.position - snake.transform.position, transform.localEulerAngles);
        } else {
            Config.Save(pt, transform.position, transform.localEulerAngles);
        }
    }

    public void Clear() {
        Config.Clear();
    }

    public void Load() {
        Config.Load();
    }
}
