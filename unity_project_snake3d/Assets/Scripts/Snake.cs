using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    None = -1,
    Up,
    Left,
    Down,
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


public class SnakePart {
    public GameObject go;
    public Transform trans;

    public Vector3 lastPos;
    public Vector3 targetPos;

    public Vector3 lastRotateAngle;
    public Vector3 targetRotateAngle;
    public Vector3 targetAngle;

    public SnakePart(GameObject g) {
        go = g;
        trans = g.transform;

        lastPos = go.transform.position;
        targetPos = go.transform.position;

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

    public void Copy(SnakePart p) {
        lastPos = p.lastPos;
        targetPos = p.targetPos;
        go.transform.position = targetPos;

        lastRotateAngle = p.lastRotateAngle;
        targetRotateAngle = p.targetRotateAngle;
        targetAngle = p.targetAngle;
        go.transform.localEulerAngles = targetAngle;
    }
}


public class Snake {
    public Snake(int id_, GameObject root, Vector3 pos, int bodyCount, int speed) {
        id = id_;
        Dead = false;

        m_speed = speed;

        snake = new List<SnakePart>();

        GameObject head = Game.Instance.Clone(Config.Instance.PrefabHead, root);
        head.transform.position = pos;
        snake.Add(new SnakePart(head));

        for (int i = 0; i < bodyCount; i++) {
            GameObject body = Game.Instance.Clone(Config.Instance.PrefabBody, root);
            pos.x += 1f;
            body.transform.position = pos;
            snake.Add(new SnakePart(body));
        }

        GameObject tail = Game.Instance.Clone(Config.Instance.PrefabTail, root);
        pos.x += 1f;
        tail.transform.position = pos;
        snake.Add(new SnakePart(tail));

        currDirect = Direction.Left;
        newDirect = Direction.None;

        currPlane = PlaneType.Down;
        mapPlane = new Dictionary<PlaneType, PlaneMove>();
        mapPlane[PlaneType.Down] = new PlaneDown();
        mapPlane[PlaneType.Left] = new PlaneLeft();
        mapPlane[PlaneType.Right] = new PlaneRight();
        mapPlane[PlaneType.Front] = new PlaneFront();

        camOffset = Config.CameraConfig[currPlane][0];
        camAngle = Config.CameraConfig[currPlane][1];
    }

    public int id;

    public bool Dead;

    int m_speed = 5;
    int bodyCount = 10;

    public List<SnakePart> snake {
        get;
        private set;
    }
    Direction currDirect;
    Direction newDirect;
    PlaneType currPlane;

    Vector3 camOffset;
    Vector3 camAngle;

    int fixedUpdateInterval = 0;
    int frameInterval = 0;

    PlaneMove pMove;
    Dictionary<PlaneType, PlaneMove> mapPlane;

    float lastFixedTime = -1f;


    void setDirection(Direction direct) {
        switch (direct) {
            case Direction.Up:
            case Direction.Down:
                if (currDirect != Direction.Up && currDirect != Direction.Down) {
                    currDirect = direct;
                }
                break;
            case Direction.Left:
            case Direction.Right:
                if (currDirect != Direction.Left && currDirect != Direction.Right) {
                    currDirect = direct;
                }
                break;
        }
    }

    void updateSnake() {
        if (lastFixedTime < 0) {
            return;
        }
        float time = Time.fixedDeltaTime * m_speed;
        for (int i = 0; i < snake.Count; i++) {
            snake[i].go.transform.position = Vector3.Lerp(snake[i].lastPos, snake[i].targetPos, (Time.time - lastFixedTime) / time);
            snake[i].go.transform.Rotate(snake[i].targetRotateAngle * (Time.deltaTime / time), Space.Self);
        }
    }

    void updateCameraParam() {
        camOffset = Config.CameraConfig[currPlane][0];
        camAngle = Config.CameraConfig[currPlane][1];
    }

    void updateCamera_() {
        Vector3 offset = camOffset.normalized;
        Game.Instance.Camera.transform.position = snake[0].go.transform.position + camOffset + offset * snake.Count * Game.Instance.CameraHighRatio;
        Game.Instance.Camera.transform.localEulerAngles = camAngle;
    }

    float cameraFloatTime = -1f;
    Vector3 cameraStartOffset;
    Vector3 cameraStartAngle;
    void updateCamera() {
        Vector3 newOffset = Config.CameraConfig[currPlane][0];
        Vector3 newAngle = Config.CameraConfig[currPlane][1];

        Vector3 distOffset = newOffset - camOffset;
        Vector3 distAngle = newAngle - camAngle;
        if (!Mathf.Approximately(distOffset.sqrMagnitude, 0) || !Mathf.Approximately(distAngle.sqrMagnitude, 0)) {
            if (cameraFloatTime < 0) {
                cameraFloatTime = Time.time;
                cameraStartOffset = camOffset;
                cameraStartAngle = camAngle;
            }
            float ratio = (Time.time - cameraFloatTime) / 2f;
            camOffset = Vector3.Lerp(cameraStartOffset, newOffset, ratio);
            camAngle = Vector3.Lerp(cameraStartAngle, newAngle, ratio);
        } else {
            if (cameraFloatTime > 0) {
                cameraFloatTime = -1f;
                camOffset = newOffset;
                camAngle = newAngle;
            }
        }

        Game.Instance.Camera.transform.position = snake[0].go.transform.position + camOffset + camOffset.normalized * snake.Count * Game.Instance.CameraHighRatio;
        Game.Instance.Camera.transform.localEulerAngles = camAngle;
    }

    bool eat() {
        bool flag = Foods.Instance.Eat(snake[0].targetPos);
        if (flag) {
            GameObject body = Game.Instance.Clone(Config.Instance.PrefabBody, snake[0].trans.parent.gameObject);
            SnakePart part = new SnakePart(body);
            part.Copy(snake[snake.Count - 2]);
            snake.Insert(snake.Count - 1, part);
        }

        return flag;
    }

    public void Update() {
        updateSnake();

        if (id == Game.Instance.playerId) {
            //updateCameraParam();
            updateCamera();
        }
    }

    public bool FixedUpdate() {
        fixedUpdateInterval++;
        if (fixedUpdateInterval < m_speed) {
            return false;
        }
        fixedUpdateInterval = 0;
        return true;
    }

    public void FixedData() {
        frameInterval--;
        if (frameInterval > 0) {
            return;
        }

        for (int i = 0; i < snake.Count; i++) {
            snake[i].trans.position = snake[i].targetPos;
            snake[i].lastPos = snake[i].targetPos;
            snake[i].trans.localEulerAngles = snake[i].targetAngle;
            snake[i].lastRotateAngle = snake[i].targetRotateAngle;
        }
    }

    public bool CrashSelf() {
        Vector3 headPos = snake[0].targetPos;
        for (int i = 1; i < snake.Count; i++) {
            Vector3 offset = snake[i].targetPos - headPos;
            if (Mathf.Approximately(offset.sqrMagnitude, 0)) {
                return true;
            }
        }

        return false;
    }

    public void Go(ref Direction direct) {
        if (direct != Direction.None) {
            newDirect = direct;
        }

        if (frameInterval > 0) {
            return;
        }

        direct = Direction.None;

        bool eatFlag = eat();

        Direction oldirect = currDirect;
        setDirection(newDirect);

        Vector3 dest = Vector3.zero;
        PlaneType newPlane = mapPlane[currPlane].Move(snake[0].lastPos, ref currDirect, ref dest);
        snake[0].SetTargetPos(dest);
        Vector3 angle;
        if (currPlane == newPlane) {
            angle = mapPlane[currPlane].Rotate(oldirect, currDirect);
        } else {
            angle = mapPlane[newPlane].Enter(currPlane);
            newDirect = currDirect;
        }
        snake[0].SetRotateAngle(angle);
        currPlane = newPlane;

        int count = eatFlag ? snake.Count - 2 : snake.Count;
        for (int i = 1; i < count; i++) {
            snake[i].SetTargetPos(snake[i - 1].lastPos);
            snake[i].targetRotateAngle = snake[i - 1].lastRotateAngle;
            snake[i].targetAngle = snake[i - 1].go.transform.localEulerAngles;
        }

        lastFixedTime = Time.time;

        frameInterval = m_speed;
    }
}
