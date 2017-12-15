using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foods : MonoBehaviour {
    class Grid {
        public int x;
        public int y;
        public Grid() {
            x = y = 0;
        }
        public Grid(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public static Foods Instance;

    List<Grid> foods = new List<Grid>();
    List<GameObject> foods_go = new List<GameObject>();

    void initFoods() {
        for (int i = 0; i < transform.childCount; i++) {
            Transform trans = transform.GetChild(i);
            Vector3 pos = trans.position;
            int x = (int)(pos.x + 49.5f);
            int y = (int)(pos.z + 49.5f);
            foods.Add(new Grid(x, y));
            foods_go.Add(trans.gameObject);
        }
    }

    void Awake() {
        Instance = this;
    }

	void Start () {
        initFoods();
	}
	
    public bool Eat(Vector3 v) {
        int x = (int)(v.x + 49.5f);
        int y = (int)(v.z + 49.5f);
        for (int i = 0; i < foods.Count; i++) {
            if (x == foods[i].x && y == foods[i].y) {
                foods.RemoveAt(i);
                GameObject go = foods_go[i];
                foods_go.RemoveAt(i);
                GameObject.Destroy(go);
                return true;
            }
        }

        return false;
    }
}
