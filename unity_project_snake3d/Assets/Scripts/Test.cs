using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    None,
    Up,
    Down,
    Left,
    Right,
}

public enum PlaneType {
    Up,
    Down,
    Left,
    Right,
    Front,
    Back,
}

public class Test : MonoBehaviour {

    public GameObject Cam;
    public float speed = 20;

    GameObject head;
    List<GameObject> bodys;
    GameObject tail;
    Direction currDirect;
    Direction newDirect;
    PlaneType currPlane;

    int interval = 0;
    Vector3 dest = Vector3.zero;

    PlaneMove pMove;
    Dictionary<PlaneType, PlaneMove> mapPlane;

    Vector3 camOffset;

    GameObject loadPrefab(string path, GameObject parent) {
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

    GameObject clone(GameObject prefab, GameObject parent) {
        GameObject go = MonoBehaviour.Instantiate<GameObject>(prefab);
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.SetActive(true);
        return go;
    }

    void init() {
        initSnake();

        currDirect = Direction.Left;
        newDirect = currDirect;
        currPlane = PlaneType.Down;

        mapPlane = new Dictionary<PlaneType, PlaneMove>();
        mapPlane[PlaneType.Down] = new PlaneDown();
        mapPlane[PlaneType.Left] = new PlaneLeft();
        mapPlane[PlaneType.Right] = new PlaneRight();
        mapPlane[PlaneType.Front] = new PlaneFront();
    }

    void initSnake() {
        bodys = new List<GameObject>();
        
        head = clone(Config.Instance.PrefabHead, gameObject);
        tail = clone(Config.Instance.PrefabTail, gameObject);
        for (int i = 0; i < 5; i++) {
            GameObject body = clone(Config.Instance.PrefabBody, gameObject);
            bodys.Add(body);
        }
    }

    void initPosition() {
        float x = 0.5f;
        float z = 0.5f;
        head.transform.localPosition = new Vector3(x, 0, z);
        for (int i = 0; i < bodys.Count; i++) {
            x += 1f;
            bodys[i].transform.localPosition = new Vector3(x, 0, z);
        }
        x += 1f;
        tail.transform.localPosition = new Vector3(x, 0, z);
    }

    void checkBound() {
        Vector3 v = head.transform.localPosition;
        if (v.x < -49.5 || v.x > 49.5) {
            if (v.x < -49.5) {
                
            }
        } else if (v.y < -49.5 || v.y > 49.5) {

        } else if (v.z < -49.5 || v.z > 49.5) {

        }
    }

    void updateInput() {
        if (Input.GetKey(KeyCode.W)) {
            if (currDirect != Direction.Up && currDirect != Direction.Down) {
                newDirect = Direction.Up;
            }
        } else if (Input.GetKey(KeyCode.S)) {
            if (currDirect != Direction.Up && currDirect != Direction.Down) {
                newDirect = Direction.Down;
            }
        } else if (Input.GetKey(KeyCode.A)) {
            if (currDirect != Direction.Left && currDirect != Direction.Right) {
                newDirect = Direction.Left;
            }
        } else if (Input.GetKey(KeyCode.D)) {
            if (currDirect != Direction.Left && currDirect != Direction.Right) {
                newDirect = Direction.Right;
            }
        }
    }

    void updateSnake() {
        
    }

    void updateCamera() {
        Cam.transform.localPosition = head.transform.localPosition + camOffset;
    }

    void go() {
        interval++;
        if (interval < speed) {
            return;
        }
        interval = 0;

        currDirect = newDirect;

        GameObject body = bodys[bodys.Count - 1];
        bodys.RemoveAt(bodys.Count - 1);
        tail.transform.localPosition = body.transform.localPosition;
        tail.transform.localRotation = body.transform.localRotation;

        body.transform.localPosition = head.transform.localPosition;
        body.transform.localRotation = head.transform.localRotation;
        bodys.Insert(0, body);

        currPlane = mapPlane[currPlane].Move(head.transform.localPosition, speed, ref currDirect, ref dest);
        Vector3 angle = mapPlane[currPlane].Rotate(currDirect);
        head.transform.localRotation = Quaternion.Euler(angle);
        head.transform.localPosition = dest;

        newDirect = currDirect;
    }

    void updateHead() {
        switch (currPlane) {
            case PlaneType.Up:
            case PlaneType.Down:
                break;
            case PlaneType.Left:
            case PlaneType.Right:
                break;
            case PlaneType.Front:
            case PlaneType.Back:
                break;
        }
    }

	// Use this for initialization
	void Start () {
        init();
        initPosition();

        camOffset = Cam.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        updateInput();
        updateSnake();
        updateCamera();
	}

    void FixedUpdate() {
        go();
    }
}
