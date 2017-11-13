using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMove {
    protected PlaneType plane;

    protected virtual PlaneType move(Transform head, ref Direction direction) { return plane; }

    public Vector3 Rotate(Direction oldDirection, Direction newDirection) {
        if ((int)newDirection == ((int)oldDirection + 1) % 4) {
            return new Vector3(0, -90, 0);
        } else if ((int)newDirection == ((int)oldDirection + 3) % 4) {
            return new Vector3(0, 90, 0);
        } else {
            return Vector3.zero;
        }
    }

    public virtual Vector3 Enter(PlaneType currPlane) { return new Vector3(0, 0, -90); }

    public virtual PlaneType Move(Vector3 currPos, ref Direction direction, ref Vector3 dest) {
        return plane;
    }
}

class PlaneDown : PlaneMove {
    public PlaneDown() {
        plane = PlaneType.Down;
    }

    public override PlaneType Move(Vector3 currPos, ref Direction direction, ref Vector3 dest) {
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

        Vector3 newPos = currPos + offset;
        PlaneType newPlane = plane;
        if (newPos.x < -49.5f) {
            newPos.x = -50;
            newPos.y = 0.5f;
            direction = Direction.Up;
            newPlane = PlaneType.Left;
        } else if (newPos.x > 49.5f) {
            newPos.x = 50;
            newPos.y = 0.5f;
            direction = Direction.Up;
            newPlane = PlaneType.Right;
        } else if (newPos.z < -49.5f) {
            newPos.z = -50;
            newPlane = PlaneType.Back;
        } else if (newPos.z > 49.5f) {
            newPos.z = 50;
            newPos.y = 0.5f;
            newPlane = PlaneType.Front;
        }
        dest = newPos;
        return newPlane;
    }
}

class PlaneLeft : PlaneMove {
    public PlaneLeft() {
        plane = PlaneType.Left;
    }    

    public override PlaneType Move(Vector3 currPos, ref Direction direction, ref Vector3 dest) {
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

        Vector3 newPos = currPos + offset;
        PlaneType newPlane = plane;
        if (newPos.y < 0.5) {
            newPos.y = 0;
            newPos.x = -49.5f;
            direction = Direction.Right;
            newPlane = PlaneType.Down;
        } else if (newPos.y > 100) {
            newPos.y = 100;
            newPlane = PlaneType.Up;
        } else if (newPos.z < -49.5f) {
            newPos.z = -50;
            newPlane = PlaneType.Back;
        } else if (newPos.z > 49.5f) {
            newPos.z = 50;
            newPos.x = -49.5f;
            direction = Direction.Right;
            newPlane = PlaneType.Front;
        }
        dest = newPos;
        return newPlane;
    }
}

class PlaneRight : PlaneMove {
    public PlaneRight() {
        plane = PlaneType.Right;
    }

    public override PlaneType Move(Vector3 currPos, ref Direction direction, ref Vector3 dest) {
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

        Vector3 newPos = currPos + offset;
        PlaneType newPlane = plane;
        if (newPos.y < 0.5f) {
            newPos.y = 0;
            newPos.x = 49.5f;
            direction = Direction.Left;
            newPlane = PlaneType.Down;
        } else if (newPos.y > 100) {
            newPos.y = 100;
            newPlane = PlaneType.Up;
        } else if (newPos.z < -49.5f) {
            newPos.z = -50;
            newPlane = PlaneType.Back;
        } else if (newPos.z > 49.5f) {
            newPos.z = 50;
            newPos.x = 49.5f;
            direction = Direction.Left;
            newPlane = PlaneType.Front;
        }
        dest = newPos;
        return newPlane;
    }
}

class PlaneFront : PlaneMove {
    public PlaneFront() {
        plane = PlaneType.Front;
    }

    public override PlaneType Move(Vector3 currPos, ref Direction direction, ref Vector3 dest) {
        Vector3 offset = Vector3.zero;
        switch (direction) {
            case Direction.Up:
                offset = Vector3.up;
                break;
            case Direction.Down:
                offset = Vector3.down;
                break;
            case Direction.Left:
                offset = Vector3.left;
                break;
            case Direction.Right:
                offset = Vector3.right;
                break;
        }

        Vector3 newPos = currPos + offset;
        PlaneType newPlane = plane;
        if (newPos.y < 0.5f) {
            newPos.y = 0;
            newPos.z = 49.5f;
            newPlane = PlaneType.Down;
        } else if (newPos.y > 100) {
            newPos.y = 100;
            newPlane = PlaneType.Up;
        } else if (newPos.x < -49.5f) {
            newPos.x = -50f;
            newPos.z = 49.5f;
            newPlane = PlaneType.Left;
        } else if (newPos.x > 49.5f) {
            newPos.x = 50f;
            newPos.z = 49.5f;
            newPlane = PlaneType.Right;
        }
        dest = newPos;
        return newPlane;
    }
}
