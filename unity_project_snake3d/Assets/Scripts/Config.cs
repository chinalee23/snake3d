using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {
    public GameObject PrefabHead;
    public GameObject PrefabBody;
    public GameObject PrefabTail;

    public static Config Instance;

	void Awake() {
        Instance = this;
    }
}
