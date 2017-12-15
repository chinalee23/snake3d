using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Direction direct = Direction.Left;

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
            double len = (double)hsSnake["len"];

            Vector3 pos = new Vector3((float)x, (float)y);
            Snake snake = new Snake((int)playerId, root, pos, (int)len);
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
        
    }
    
    public override void Start() {
        NetSystem.Instance.Init("tcp");
        NetSystem.Instance.Connect("127.0.0.1", 12345, onConnect);
    }

    public override void Update() {
        NetSystem.Instance.Update();

        if (!started) {
            return;
        }

        Snake mine = snakes[Game.Instance.playerId];
        if (mine.Dead) {
            return;
        }

        mine.Update();

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
        if (!started) {
            return;
        }
        Snake mine = snakes[Game.Instance.playerId];
        if (!mine.FixedUpdate()) {
            return;
        }

        string js = "{\"id:\":" + Game.Instance.playerId + ", \"direct\":" + (int)direct + "}";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(js);
        NetSystem.Instance.Send((int)MsgType.Frame, data);

        Debug.Log("send frame: " + js);
    }

    public override void ProcessMsg(int msgType, byte[] msg) {
        string js = System.Text.Encoding.UTF8.GetString(msg);
        Debug.Log(string.Format("receive {0}: {1}", msgType, js));
        object jd = MiniJSON.jsonDecode(js);
        if (mapFuncs.ContainsKey(msgType)) {
            mapFuncs[msgType](jd);
        }
    }

    public void StartRoom() {
        NetSystem.Instance.Send((int)MsgType.RoomStartReq, null);
    }
}
