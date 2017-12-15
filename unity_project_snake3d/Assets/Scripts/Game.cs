using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public GameObject Camera;
    public float CameraHighRatio = 0.1f;

    public static Game Instance;

    public int playerId = 0;

    Battle battle;


    void Awake() {
        Instance = this;
        Application.runInBackground = true;
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (battle != null) {
            battle.Update();
        }
    }

    private void FixedUpdate() {
        if (battle != null) {
            battle.FixedUpdate();
        }
    }

    private void OnApplicationQuit() {
        NetSystem.Instance.Dispose();
    }

    public Battle GetBattle() {
        return battle;
    }

    public void StartBattle(bool flag) {
        if (flag) {
            battle = new BattleOnline();
            battle.Start();
        } else {
            battle = new BattleOffline();
            battle.Start();
        }
    }

    public void ProcessMsg(int msgType, byte[] msg) {
        battle.ProcessMsg(msgType, msg);
    }

    public GameObject LoadPrefab(string path, GameObject parent) {
        Object o = Resources.Load(path);
        if (o == null) {
            return null;
        }

        GameObject go = MonoBehaviour.Instantiate(o) as GameObject;
        if (go == null) {
            return null;
        }
        go.SetActive(true);
        go.transform.parent = parent.transform;
        return go;
    }

    public GameObject Clone(GameObject prefab, GameObject parent) {
        GameObject go = MonoBehaviour.Instantiate<GameObject>(prefab);
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        go.SetActive(true);
        return go;
    }
}
