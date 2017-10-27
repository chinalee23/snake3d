using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMove {
    protected PlaneType plane;

    protected virtual PlaneType move(Transform head, ref Direction direction) { return plane; }

    public virtual void Rotate(Transform head, Direction direction) { }

    public virtual PlaneType Move(Transform head, ref Direction direction) {
        return plane;
    }
}

class PlaneDown : PlaneMove {
    public PlaneDown() {
        plane = PlaneType.Down;
    }

    public override void Rotate(Transform head, Direction direction) {
        switch (direction) {
            case Direction.Up:
                head.localRotation = Quaternion.Euler(0, 90, 0);
                break;
            case Direction.Down:
                head.localRotation = Quaternion.Euler(0, 270, 0);
                break;
            case Direction.Left:
                head.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Right:
                head.localRotation = Quaternion.Euler(0, 180, 0);
                break;
        }
    }

    public override PlaneType Move(Transform head, ref Direction direction) {
        Vector3 offset = Vector3.zero;
        switch (direction) {
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
        }

        Vector3 newPos = head.localPosition + offset;
        PlaneType newPlane = plane;
        if (newPos.x < -49.5) {
            newPos.x = -50;
            newPos.y = 0.5f;
            head.localPosition = newPos;
            direction = Direction.Up;
            newPlane = PlaneType.Left;
        } else if (newPos.x > 49.5) {
            newPos.x = 50;
            newPos.y = 0.5f;
            head.localPosition = newPos;
            direction = Direction.Up;
            newPlane = PlaneType.Right;
        } else if (newPos.z < -49.5) {
            newPos.z = -50;
            return PlaneType.Back;
        } else if (newPos.z > 49.5) {
            newPos.z = 50;
            return PlaneType.Front;
        } else {
            Rotate(head, direction);
            head.localPosition = newPos;
        }
        return newPlane;
    }
}

class PlaneLeft : PlaneMove {
    public PlaneLeft() {
        plane = PlaneType.Left;
    }
    public override void Rotate(Transform head, Direction direction) {
        switch (direction) {
            case Direction.Up:
                head.localRotation = Quaternion.Euler(0, 0, -90);
                break;
            case Direction.Down:
                head.localRotation = Quaternion.Euler(180, 0, -90);
                break;
            case Direction.Left:
                head.localRotation = Quaternion.Euler(-90, 0, -90);
                break;
            case Direction.Right:
                head.localRotation = Quaternion.Euler(90, 0, -90);
                break;
        }
    }

    public override PlaneType Move(Transform head, ref Direction direction) {
        Vector3 offset = Vector3.zero;
        switch (direction) {
            case Direction.Up:
                offset = Vector3.up;
                break;
            case Direction.Down:
                offset = Vector3.down;
                break;
            case Direction.Left:
                offset = Vector3.back;
                break;
            case Direction.Right:
                offset = Vector3.forward;
                break;
        }

        Vector3 newPos = head.localPosition + offset;
        PlaneType newPlane = plane;
        if (newPos.y < 0.5) {
            newPos.y = 0;
            newPos.x = -49.5f;
            head.localPosition = newPos;
            return PlaneType.Down;
        } else if (newPos.y > 100) {
            newPos.y = 100;
            return PlaneType.Up;
        } else if (newPos.z < -49.5) {
            newPos.z = -50;
            return PlaneType.Back;
        } else if (newPos.z > 49.5) {
            newPos.z = 50;
            return PlaneType.Front;
        } else {
            Rotate(head, direction);
            head.localPosition = newPos;
        }
        return newPlane;
    }
}

class PlaneRight : PlaneMove {
    public PlaneRight() {
        plane = PlaneType.Right;
    }
    public override void Rotate(Transform head, Direction direction) {
        switch (direction) {
            case Direction.Up:
                head.localRotation = Quaternion.Euler(0, 180, -90);
                break;
            case Direction.Down:
                head.localRotation = Quaternion.Euler(180, 180, -90);
                break;
            case Direction.Left:
                head.localRotation = Quaternion.Euler(-90, 180, -90);
                break;
            case Direction.Right:
                head.localRotation = Quaternion.Euler(90, 180, -90);
                break;
        }
    }

    public override PlaneType Move(Transform head, ref Direction direction) {
        Vector3 offset = Vector3.zero;
        switch (direction) {
            case Direction.Up:
                offset = Vector3.up;
                break;
            case Direction.Down:
                offset = Vector3.down;
                break;
            case Direction.Left:
                offset = Vector3.forward;
                break;
            case Direction.Right:
                offset = Vector3.back;
                break;
        }

        Vector3 newPos = head.localPosition + offset;
        PlaneType newPlane = plane;
        if (newPos.y < 0.5) {
            newPos.y = 0;
            newPos.x = -49.5f;
            head.localRotation = Quaternion.Euler(0, 180, 0);
            head.localPosition = newPos;
        } else if (newPos.y > 100) {
            newPos.y = 100;
            return PlaneType.Up;
        } else if (newPos.z < -49.5) {
            newPos.z = -50;
            return PlaneType.Back;
        } else if (newPos.z > 49.5) {
            newPos.z = 50;
            return PlaneType.Front;
        } else {
            Rotate(head, direction);
            head.localPosition = newPos;
        }
        return newPlane;
    }
}
