using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfig : MonoBehaviour {
    public PlaneType pt;

    public void Save() {
        Config.Save(pt, transform.position);
    }
}
