using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour {
    static UIStart _instance;
    public static UIStart Instance {
        get {
            return _instance;
        }
    }

    public GameObject RootStart;
    public Button BtnOnline;
    public Button BtnOffline;

    public GameObject RootOnline;
    public Button BtnStart;
    public Text TextRoom;
    public Text TextPlayerCount;

    public GameObject GoCross;

    void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        GoCross.SetActive(false);

        BtnOnline.onClick.AddListener(delegate () {
            Game.Instance.StartBattle(true);
            RootStart.SetActive(false);
            RootOnline.SetActive(true);
        });

        BtnOffline.onClick.AddListener(delegate () {
            Game.Instance.StartBattle(false);
            Disvisiable();
        });

        BtnStart.onClick.AddListener(delegate () {
            BattleOnline battle = Game.Instance.GetBattle() as BattleOnline;
            battle.StartRoom();
        });

        RootStart.SetActive(true);
        RootOnline.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetRoomInfo(int roomId, int playerCount) {
        TextRoom.text = "房间号: " + roomId;
        TextPlayerCount.text = "人数: " + playerCount;
    }

    public void Disvisiable() {
        gameObject.SetActive(false);
        GoCross.SetActive(true);
    }
}
