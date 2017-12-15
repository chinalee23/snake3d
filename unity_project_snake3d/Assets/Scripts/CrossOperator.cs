using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossOperator : MonoBehaviour {
    public Button BtnUp;
    public Button BtnDown;
    public Button BtnLeft;
    public Button BtnRight;

    public static CrossOperator Instance;
    public Direction direct = Direction.None;

	void Awake() {
        Instance = this;
    }

	void Start () {
        BtnUp.onClick.AddListener(delegate () {
            direct = Direction.Up;
        });

        BtnDown.onClick.AddListener(delegate () {
            direct = Direction.Down;
        });

        BtnLeft.onClick.AddListener(delegate () {
            direct = Direction.Left;
        });

        BtnRight.onClick.AddListener(delegate () {
            direct = Direction.Right;
        });
    }
}
