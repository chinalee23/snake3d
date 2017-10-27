using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMove {
    protected PlaneType plane;

    protected virtual PlaneType move(Transform head, ref Direction direction) { return plane; }

    public virtual Vector3 Rotate(Direction direction) { return Vector3.zero; }

    public virtual PlaneType Move(Vector3 currPos, float speed, ref Direction direction, ref Vector3 dest) {
        return plane;
    }
}

class PlaneDown : PlaneMove {
    public PlaneDown() {
        plane = PlaneType.Down;
    }

    public override Vector3 Rotate(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return new Vector3(0, 90, 0);
            case Direction.Down:
                return new Vector3(0, 270, 0);
            case Direction.Left:
                return new Vector3(0, 0, 0);
            case Direction.Right:
                return new Vector3(0, 180, 0);
            default:
                return Vector3.zero;
        }
    }

    public override PlaneType Move(Vector3 currPos, float speed, ref Direction direction, ref Vector3 dest) {
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
            return PlaneType.Back;
        } else if (newPos.z > 49.5f) {
            newPos.z = 50;
            newPos.y = 0.5f;
            newPlane = PlaneType.Front;
            return PlaneType.Front;
        }
        dest = newPos;
        return newPlane;
    }
}

class PlaneLeft : PlaneMove {
    public PlaneLeft() {
        plane = PlaneType.Left;
    }
    public override Vector3 Rotate(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return new Vector3(0, 0, -90);
            case Direction.Down:
                return new Vector3(180, 0, -90);
            case Direction.Left:
                return new Vector3(-90, 0, -90);
            case Direction.Right:
                return new Vector3(90, 0, -90);
            default:
                return Vector3.zero;
        }
    }

    public override PlaneType Move(Vector3 currPos, float speed, ref Direction direction, ref Vector3 dest) {
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
            return PlaneType.Up;
        } else if (newPos.z < -49.5f) {
            newPos.z = -50;
            return PlaneType.Back;
        } else if (newPos.z > 49.5f) {
            newPos.z = 50;
            newPos.x = -49.5f;
            direction = Direction.Right;
            newPlane = PlaneType.Front;
            return PlaneType.Front;
        }
        dest = newPos;
        return newPlane;
    }
}

class PlaneRight : PlaneMove {
    public PlaneRight() {
        plane = PlaneType.Right;
    }

    public override Vector3 Rotate(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return new Vector3(0, 180, -90);
            case Direction.Down:
                return new Vector3(180, 180, -90);
            case Direction.Left:
                return new Vector3(-90, 180, -90);
            case Direction.Right:
                return new Vector3(90, 180, -90);
            default:
                return Vector3.zero;
        }
    }

    public override PlaneType Move(Vector3 currPos, float speed, ref Direction direction, ref Vector3 dest) {
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
            return PlaneType.Up;
        } else if (newPos.z < -49.5f) {
            newPos.z = -50;
            return PlaneType.Back;
        } else if (newPos.z > 49.5f) {
            newPos.z = 50;
            newPos.x = 49.5f;
            direction = Direction.Left;
            newPlane = PlaneType.Front;
            return PlaneType.Front;
        }
        dest = newPos;
        return newPlane;
    }
}

class PlaneFront : PlaneMove {
    public PlaneFront() {
        plane = PlaneType.Front;
    }

    public override Vector3 Rotate(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return new Vector3(0, 90, -90);
            case Direction.Down:
                return new Vector3(180, 90, -90);
            case Direction.Left:
                return new Vector3(-90, 90, -90);
            case Direction.Right:
                return new Vector3(90, 90, -90);
            default:
                return Vector3.zero;
        }
    }

    public override PlaneType Move(Vector3 currPos, float speed, ref Direction direction, ref Vector3 dest) {
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
            return PlaneType.Up;
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
