using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakePart {
    public GameObject go;

    public Vector3 lastPos;
    public Vector3 targetPos;

    public Vector3 lastRotateAngle;
    public Vector3 targetRotateAngle;
    public Vector3 targetAngle;

    public SnakePart(GameObject g) {
        go = g;

        lastPos = go.transform.localPosition;
        targetPos = go.transform.localPosition;
        
        lastRotateAngle = Vector3.zero;
        targetRotateAngle = Vector3.zero;
        targetAngle = Vector3.zero;
    }

    public void SetTargetPos(Vector3 v) {
        targetPos = v;
    }

    public void SetRotateAngle(Vector3 v) {
        targetRotateAngle = v;
        Quaternion old = go.transform.localRotation;
        go.transform.Rotate(v);
        targetAngle = go.transform.localEulerAngles;
        go.transform.localRotation = old;
    }

    public void Clone(SnakePart p) {
        lastPos = p.lastPos;
        targetPos = p.targetPos;
        go.transform.localPosition = targetPos;

        lastRotateAngle = p.lastRotateAngle;
        targetRotateAngle = p.targetRotateAngle;
        targetAngle = p.targetAngle;
        go.transform.localEulerAngles = targetAngle;
    }
}

public class TestNew : MonoBehaviour {

    public GameObject Cam;
    public int speed = 20;
    public int bodyCount = 10;
    public float highRatio = 0.1f;
    
    List<SnakePart> snake;
    Direction currDirect;
    Direction newDirect;
    PlaneType currPlane;

    int interval = 0;

    PlaneMove pMove;
    Dictionary<PlaneType, PlaneMove> mapPlane;

    Vector3 camOffset;
    Vector3 camAngle;

    float lastFixedTime = -1f;

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

        Vector3 pos = new Vector3(-16.5f, 0, -2.5f);

        GameObject head = clone(Config.Instance.PrefabHead, gameObject);
        head.transform.localPosition = pos;
        snake.Add(new SnakePart(head));

        for (int i = 0; i < bodyCount - 1; i++) {
            GameObject body = clone(Config.Instance.PrefabBody, gameObject);
            pos.x += 1f;
            body.transform.localPosition = pos;
            snake.Add(new SnakePart(body));
        }
        if (bodyCount > 0) {
            GameObject tail = clone(Config.Instance.PrefabTail, gameObject);
            pos.x += 1f;
            tail.transform.localPosition = pos;
            snake.Add(new SnakePart(tail));
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
        if (lastFixedTime < 0) {
            return;
        }
        float time = Time.fixedDeltaTime * speed;
        for (int i = 0; i < snake.Count; i++) {
            snake[i].go.transform.localPosition = Vector3.Lerp(snake[i].lastPos, snake[i].targetPos, (Time.time - lastFixedTime)/time);
            snake[i].go.transform.Rotate(snake[i].targetRotateAngle * (Time.deltaTime / time), Space.Self);
        }
    }

    void updateCamera() {
        Vector3 offset = camOffset.normalized;
        Cam.transform.localPosition = snake[0].go.transform.position + camOffset + offset * snake.Count * highRatio;
        Cam.transform.localEulerAngles = camAngle;
    }
    
    void go() {
        interval++;
        if (interval < speed) {
            return;
        }
        interval = 0;

        for (int i = 0; i < snake.Count; i++) {
            snake[i].go.transform.localPosition = snake[i].targetPos;
            snake[i].lastPos = snake[i].targetPos;
            snake[i].go.transform.localEulerAngles = snake[i].targetAngle;
            snake[i].lastRotateAngle = snake[i].targetRotateAngle;
        }
        updateCameraParam();

        bool eat = Foods.Instance.Eat(snake[0].targetPos);
        if (eat) {
            GameObject body = clone(Config.Instance.PrefabBody, gameObject);
            SnakePart part = new SnakePart(body);
            part.Clone(snake[snake.Count - 2]);
            snake.Insert(snake.Count - 1, part);
        }

        Vector3 dest = Vector3.zero;
        PlaneType newPlane = mapPlane[currPlane].Move(snake[0].lastPos, ref newDirect, ref dest);
        snake[0].SetTargetPos(dest);
        Vector3 angle;
        if (currPlane == newPlane) {
            angle = mapPlane[currPlane].Rotate(currDirect, newDirect);
        } else {
            angle = mapPlane[newPlane].Enter(currPlane);
        }
        snake[0].SetRotateAngle(angle);
        currPlane = newPlane;
        
        int count = eat ? snake.Count - 2 : snake.Count;
        for (int i = 1; i < count; i++) {
            snake[i].SetTargetPos(snake[i - 1].lastPos);
            snake[i].targetRotateAngle = snake[i - 1].lastRotateAngle;
            snake[i].targetAngle = snake[i - 1].go.transform.localEulerAngles;
        }

        lastFixedTime = Time.time;
        currDirect = newDirect;
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

    void updateCameraParam() {
        camOffset = Config.CameraConfig[currPlane][0];
        camAngle = Config.CameraConfig[currPlane][1];
    }
    
    void Start() {
        init();
        updateCameraParam();
    }
    
    void Update() {
        updateInput();
        updateSnake();
        updateCamera();
    }

    void FixedUpdate() {
        go();
    }
}
