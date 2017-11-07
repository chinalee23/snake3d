using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakePart {
    public GameObject go;
    public Vector3 targetPos;
    public Vector3 targetRot;

    public SnakePart(GameObject g) {
        go = g;
        targetPos = go.transform.localPosition;
        targetRot = Vector3.zero;
    }
}

public class TestNew : MonoBehaviour {

    public GameObject Cam;
    public float speed = 20;
    
    List<SnakePart> snake;
    Direction currDirect;
    Direction newDirect;
    PlaneType currPlane;

    int interval = 0;

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
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
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
        snake = new List<SnakePart>();
    }

    void initPosition() {
        Vector3 pos = new Vector3(0.5f, 0, 0.5f);

        GameObject head = clone(Config.Instance.PrefabHead, gameObject);
        head.transform.localPosition = pos;
        snake.Add(new SnakePart(head));

        for (int i = 0; i < 5; i++) {
            GameObject body = clone(Config.Instance.PrefabBody, gameObject);
            pos.x += 1f;
            body.transform.localPosition = pos;
            snake.Add(new SnakePart(body));
        }
        
        GameObject tail = clone(Config.Instance.PrefabTail, gameObject);
        pos.x += 1f;
        tail.transform.localPosition = pos;
        snake.Add(new SnakePart(tail));
    }

    void checkBound() {
        Vector3 v = snake[0].go.transform.localPosition;
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
        float time = Time.fixedDeltaTime * speed;
        for (int i = 0; i < snake.Count; i++) {
            snake[i].go.transform.localPosition = Vector3.Lerp(snake[i].go.transform.localPosition, snake[i].targetPos, Time.deltaTime / time);
            snake[i].go.transform.localRotation = Quaternion.Euler(Vector3.Lerp(snake[i].go.transform.localRotation.eulerAngles, snake[i].targetRot, Time.deltaTime / time));
        }
    }

    void updateCamera() {
        Cam.transform.localPosition = snake[0].go.transform.localPosition + camOffset;
    }
    
    void go() {
        interval++;
        if (interval < speed) {
            return;
        }
        interval = 0;

        currDirect = newDirect;
        
        Vector3 dest = Vector3.zero;
        currPlane = mapPlane[currPlane].Move(snake[0].go.transform.localPosition, speed, ref currDirect, ref dest);
        Vector3 angle = mapPlane[currPlane].Rotate(currDirect);

        snake[0].targetPos = dest;
        snake[0].targetRot = angle;
        for (int i = 1; i < snake.Count; i++) {
            snake[i].targetPos = snake[i - 1].go.transform.localPosition;
            snake[i].targetRot = snake[i - 1].go.transform.localRotation.eulerAngles;
        }

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
    void Start() {
        init();
        initPosition();

        camOffset = Cam.transform.localPosition;
    }

    // Update is called once per frame
    void Update() {
        updateInput();
        updateSnake();
        updateCamera();
    }

    void FixedUpdate() {
        go();
    }
}
