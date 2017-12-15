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

    public void Copy(SnakePart p) {
        lastPos = p.lastPos;
        targetPos = p.targetPos;
        go.transform.localPosition = targetPos;

        lastRotateAngle = p.lastRotateAngle;
        targetRotateAngle = p.targetRotateAngle;
        targetAngle = p.targetAngle;
        go.transform.localEulerAngles = targetAngle;
    }
}


public class Snake {
    public Snake(int id, GameObject root, Vector3 pos, int bodyCount) {
        id_ = id;
        dead = false;

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

        currPlane = PlaneType.Down;
        mapPlane = new Dictionary<PlaneType, PlaneMove>();
        mapPlane[PlaneType.Down] = new PlaneDown();
        mapPlane[PlaneType.Left] = new PlaneLeft();
        mapPlane[PlaneType.Right] = new PlaneRight();
        mapPlane[PlaneType.Front] = new PlaneFront();
    }

    int id_;

    bool dead;
    public bool Dead {
        get {
            return dead;
        }
    }

    int speed = 5;
    int bodyCount = 10;

    List<SnakePart> snake;
    Direction currDirect;
    PlaneType currPlane;

    Vector3 camOffset;
    Vector3 camAngle;

    int interval = 0;

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
        float time = Time.fixedDeltaTime * speed;
        for (int i = 0; i < snake.Count; i++) {
            snake[i].go.transform.localPosition = Vector3.Lerp(snake[i].lastPos, snake[i].targetPos, (Time.time - lastFixedTime) / time);
            snake[i].go.transform.Rotate(snake[i].targetRotateAngle * (Time.deltaTime / time), Space.Self);
        }
    }

    void updateCameraParam() {
        camOffset = Config.CameraConfig[currPlane][0];
        camAngle = Config.CameraConfig[currPlane][1];
    }

    void updateCamera() {
        Vector3 offset = camOffset.normalized;
        Game.Instance.Camera.transform.localPosition = snake[0].go.transform.position + camOffset + offset * snake.Count * Game.Instance.CameraHighRatio;
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

    bool crash() {
        Vector3 headPos = snake[0].targetPos;
        for (int i = 1; i < snake.Count; i++) {
            Vector3 offset = snake[i].targetPos - headPos;
            if (Mathf.Approximately(offset.sqrMagnitude, 0)) {
                dead = true;
                return true;
            }
        }

        return false;
    }

    public void Update() {
        updateSnake();

        if (id_ == Game.Instance.playerId) {
            updateCameraParam();
            updateCamera();
        }
    }

    public bool FixedUpdate() {
        interval++;
        if (interval < speed) {
            return false;
        }
        interval = 0;
        return true;
    }

    public void Go(Direction newDirect) {
        interval++;
        if (interval < speed) {
            return;
        }
        interval = 0;

        for (int i = 0; i < snake.Count; i++) {
            snake[i].trans.position = snake[i].targetPos;
            snake[i].lastPos = snake[i].targetPos;
            snake[i].trans.localEulerAngles = snake[i].targetAngle;
            snake[i].lastRotateAngle = snake[i].targetRotateAngle;
        }

        if (crash()) {
            return;
        }

        bool eatFlag = eat();

        Direction direct = currDirect;
        setDirection(newDirect);

        Vector3 dest = Vector3.zero;
        PlaneType newPlane = mapPlane[currPlane].Move(snake[0].lastPos, ref currDirect, ref dest);
        snake[0].SetTargetPos(dest);
        Vector3 angle;
        if (currPlane == newPlane) {
            angle = mapPlane[currPlane].Rotate(direct, currDirect);
        } else {
            angle = mapPlane[newPlane].Enter(currPlane);
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
    }
}
