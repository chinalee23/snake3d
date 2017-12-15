using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSystem {
    public static NetSystem _instance = new NetSystem();
    public static NetSystem Instance {
        get {
            return _instance;
        }
    }

    Net.Transfer transfer;

    public void Init(string protocol) {
        if (protocol == "tcp") {
            transfer = new Net.TransferTcp();
        } else {
            transfer = new Net.TransferUdp();
        }
    }

    public void Connect(string ip, int port, System.Action<bool> cb) {
        System.Net.IPEndPoint ep = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port);
        transfer.Connect(ep, delegate () {
            cb(transfer.Connected);
        });
    }

    public void Send(int msgType, byte[] data) {
        transfer.Send(msgType, data);
    }

    public void Update() {
        if (transfer == null || !transfer.Connected) {
            return;
        }

        transfer.Update();
        while (true) {
            Net.Message msg = transfer.Recv();
            if (msg == null) {
                break;
            }
            Game.Instance.ProcessMsg(msg.msgType, msg.msg);
        }
    }

    public void Dispose() {
        if (transfer != null) {
            transfer.Disconnect();
        }
    }
}
