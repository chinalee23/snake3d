using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class BattleOnline: Battle {
    enum MsgType {
        EnterReq = 1,
        EnterRsp = 2,
        RoomInfo = 3,
        RoomStartReq = 4,
        BattleStart = 5,
        Ready = 6,
        Fight = 7,
        Frame = 8,
    }
    
    Dictionary<int, Snake> snakes;
    Dictionary<int, System.Action<object>> mapFuncs;
    Direction direct = Direction.None;

    bool started;

    public BattleOnline() : base() {
        mapFuncs = new Dictionary<int, System.Action<object>>();
        mapFuncs.Add(2, onEnterRsp);
        mapFuncs.Add(3, onRoomInfo);
        mapFuncs.Add(5, onBattleStart);
        mapFuncs.Add(7, onFight);
        mapFuncs.Add(8, onFrame);

        started = false;
    }

    void onConnect(bool status) {
        if (!status) {
            Debug.LogError("connect fail");
            return;
        }
        Debug.Log("connect success");

        sendEnter();
    }

    void sendEnter() {
        NetSystem.Instance.Send((int)MsgType.EnterReq, null);
    }

    void onEnterRsp(object jd) {
        Hashtable hs = jd as Hashtable;
        double id = (double)hs["id"];
        Game.Instance.playerId = (int)id;
    }

    void onRoomInfo(object jd) {
        Hashtable hs = jd as Hashtable;
        double roomId = (double)hs["id"];
        double playerCount = (double)hs["playerCount"];
        UIStart.Instance.SetRoomInfo((int)roomId, (int)playerCount);
    }

    void onBattleStart(object jd) {
        GameObject root = new GameObject("BattleRoot");
        snakes = new Dictionary<int, Snake>();

        Hashtable hs = jd as Hashtable;
        ArrayList al = hs["players"] as ArrayList;
        for (int i = 0; i < al.Count; i++) {
            Hashtable hsPlayer = al[i] as Hashtable;
            double playerId = (double)hsPlayer["id"];
            Hashtable hsSnake = hsPlayer["snake"] as Hashtable;
            double x = (double)hsSnake["x"];
            double y = (double)hsSnake["y"];
            double z = (double)hsSnake["z"];
            double len = (double)hsSnake["len"];
            double speed = (double)hsSnake["speed"];

            Vector3 pos = new Vector3((float)x, (float)y, (float)z);
            Snake snake = new Snake((int)playerId, root, pos, (int)len, (int)speed);
            snakes.Add((int)playerId, snake);
        }

        UIStart.Instance.Disvisiable();

        NetSystem.Instance.Send((int)MsgType.Ready, null);
    }

    void onFight(object jd) {
        Debug.Log("fight...");

        started = true;
    }

    void onFrame(object jd) {
        foreach (var kvp in snakes) {
            kvp.Value.FixedData();
            if (kvp.Value.CrashSelf()) {
                kvp.Value.Dead = true;
            }
        }

        calcCrash();

        Hashtable hs = jd as Hashtable;
        ArrayList al = hs["frames"] as ArrayList;

        Dictionary<int, Direction> mapOp = new Dictionary<int, Direction>();
        for (int i = 0; i < al.Count; i++) {
            Hashtable hsPlayer = al[i] as Hashtable;
            int id = (int)((double)hsPlayer["id"]);
            int dir = (int)((double)hsPlayer["direct"]);
            mapOp[id] = (Direction)dir;
        }

        foreach (var kvp in snakes) {
            if (kvp.Value.Dead) {
                continue;
            }
            Direction direct = Direction.None;
            if (mapOp.ContainsKey(kvp.Value.id)) {
                direct = mapOp[kvp.Value.id];
            }
            kvp.Value.Go(ref direct);
        }
    }

    bool checkCrash(SnakePart p1, SnakePart p2) {
        Vector3 offset = p1.trans.position - p2.trans.position;
        return Mathf.Approximately(offset.sqrMagnitude, 0);
    }

    void calcCrash() {
        foreach (Snake si in snakes.Values) {
            if (si.Dead) {
                continue;
            }
            SnakePart headi = si.snake[0];
            foreach (Snake sj in snakes.Values) {
                if (si.id == sj.id) {
                    continue;
                }
                if (checkCrash(headi, sj.snake[0])) {
                    // 头和头相撞
                    si.Dead = true;
                    sj.Dead = true;
                    Debug.Log(string.Format("snake[{0}] crash snake[{1}] head to head", si.id, sj.id));
                    break;
                } else {
                    for (int j = 1; j < sj.snake.Count; j++) {
                        // 头和身子相撞
                        if (checkCrash(headi, sj.snake[j])) {
                            Debug.Log(string.Format("snake[{0}] head crash snake[{1}] body", si.id, sj.id));
                            si.Dead = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    public override void Start() {
        NetSystem.Instance.Init("tcp");

        if (Application.isEditor) {
            string ip = "127.0.0.1";
            int port = 12345;
            NetSystem.Instance.Connect(ip, port, onConnect);
        } else {
            NetSystem.Instance.Connect(UIStart.Instance.Ip, UIStart.Instance.Port, onConnect);
        }
    }

    public override void Update() {
        NetSystem.Instance.Update();

        if (!started) {
            return;
        }

        //if (Input.GetKey(KeyCode.W)) {
        //    direct = Direction.Up;
        //} else if (Input.GetKey(KeyCode.S)) {
        //    direct = Direction.Down;
        //} else if (Input.GetKey(KeyCode.A)) {
        //    direct = Direction.Left;
        //} else if (Input.GetKey(KeyCode.D)) {
        //    direct = Direction.Right;
        //}
        direct = CrossOperator.Instance.direct;

        foreach (var kvp in snakes) {
            if (!kvp.Value.Dead) {
                kvp.Value.Update();
            }
        }
    }

    public override void FixedUpdate() {
        if (!started) {
            return;
        }
        Snake mine = snakes[Game.Instance.playerId];
        if (!mine.FixedUpdate()) {
            return;
        }

        if (direct != Direction.None) {
            string js = "{\"id\":" + Game.Instance.playerId + ", \"direct\":" + (int)direct + "}";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(js);
            NetSystem.Instance.Send((int)MsgType.Frame, data);

            direct = Direction.None;
        }
    }

    public override void ProcessMsg(int msgType, byte[] msg) {
        string js = System.Text.Encoding.UTF8.GetString(msg);
        object jd = MiniJSON.jsonDecode(js);
        if (mapFuncs.ContainsKey(msgType)) {
            mapFuncs[msgType](jd);
        }
    }

    public void StartRoom() {
        NetSystem.Instance.Send((int)MsgType.RoomStartReq, null);
    }
}
