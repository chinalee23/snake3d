using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOffline: Battle {
    Snake snake;
    Direction direct = Direction.Left;

    public override void Start() {
        GameObject root = new GameObject("BattleRoot");
        snake = new Snake(Game.Instance.playerId, root, new Vector3(0.5f, 0, 0.5f), 10, 3);
    }

    public override void Update() {
        if (snake.Dead) {
            return;
        }

        snake.Update();

        //direct = CrossOperator.Instance.direct;
        if (Input.GetKey(KeyCode.W)) {
            direct = Direction.Up;
        } else if (Input.GetKey(KeyCode.S)) {
            direct = Direction.Down;
        } else if (Input.GetKey(KeyCode.A)) {
            direct = Direction.Left;
        } else if (Input.GetKey(KeyCode.D)) {
            direct = Direction.Right;
        }
    }

    public override void FixedUpdate() {
        if (snake.Dead) {
            return;
        }

        snake.FixedData();
        if (snake.CrashSelf()) {
            snake.Dead = true;
        } else {
            snake.Go(ref direct);
        }
    }
}
