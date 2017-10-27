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

    GameObject head;
    List<GameObject> bodys;
    GameObject tail;
    Direction currDirect;
    Direction newDirect;
    PlaneType currPlane;

    float fixedInterval = 0.01f;
    int speed = 20;
    int interval = 0;
    Vector3 dest;

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

    void init() {
        bodys = new List<GameObject>();

        head = loadPrefab("prefab/snake_head", gameObject);
        tail = loadPrefab("prefab/snake_tail", gameObject);
        for (int i = 0; i < 5; i++) {
            GameObject body = loadPrefab("prefab/snake_body", gameObject);
            bodys.Add(body);
        }

        currDirect = Direction.Left;
        newDirect = currDirect;
        currPlane = PlaneType.Down;

        mapPlane = new Dictionary<PlaneType, PlaneMove>();
        mapPlane[PlaneType.Down] = new PlaneDown();
        mapPlane[PlaneType.Left] = new PlaneLeft();
        mapPlane[PlaneType.Right] = new PlaneRight();
        mapPlane[PlaneType.Front] = new PlaneFront();
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

        PlaneType newPlane = mapPlane[currPlane].Move(head.transform, ref currDirect);
        if (newPlane != currPlane) {
            mapPlane[newPlane].Rotate(head.transform, currDirect);
            currPlane = newPlane;
            newDirect = currDirect;
        }
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
