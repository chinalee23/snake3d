using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    enum Direction {
        None,
        Up,
        Down,
        Left,
        Right,
    }
    GameObject head;
    List<GameObject> bodys;
    GameObject tail;
    Direction currDirect;
    Direction newDirect;

    float fixedInterval = 0.01f;
    int speed = 20;
    int interval = 0;
    Vector3 dest;

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

    void go() {
        interval++;
        if (interval < speed) {
            return;
        }
        interval = 0;

        if (currDirect != newDirect) {
            switch (newDirect) {
                case Direction.Up:
                    head.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case Direction.Down:
                    head.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    break;
                case Direction.Left:
                    head.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Right:
                    head.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
            if (newDirect != Direction.None) {
                currDirect = newDirect;
            }
        }

        Vector3 offset;
        switch (currDirect) {
            case Direction.Up:
                offset = Vector3.forward;
                break;
            case Direction.Down:
                offset = Vector3.back;
                break;
            case Direction.Left:
                offset = Vector3.left;
                break;
            case Direction.Right:
                offset = Vector3.right;
                break;
            default:
                return;
        }
        GameObject body = bodys[bodys.Count - 1];
        bodys.RemoveAt(bodys.Count - 1);
        tail.transform.localPosition = body.transform.localPosition;
        body.transform.localPosition = head.transform.localPosition;
        bodys.Insert(0, body);
        head.transform.localPosition += offset;

        offset = bodys[bodys.Count - 1].transform.localPosition - tail.transform.localPosition;
        if (offset == Vector3.left) {
            tail.transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else if (offset == Vector3.right) {
            tail.transform.localRotation = Quaternion.Euler(0, 180, 0);
        } else if (offset == Vector3.forward) {
            tail.transform.localRotation = Quaternion.Euler(0, 90, 0);
        } else if (offset == Vector3.back) {
            tail.transform.localRotation = Quaternion.Euler(0, 270, 0);
        }
    }

	// Use this for initialization
	void Start () {
        init();
        initPosition();
	}
	
	// Update is called once per frame
	void Update () {
        updateInput();
        updateSnake();
	}

    void FixedUpdate() {
        go();
    }
}
