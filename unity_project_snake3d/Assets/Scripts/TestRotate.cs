using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour {
    public Vector3 startAngle;
    public Vector3 tarAngle;
    float lastTime = 0;
    bool rotating = false;

	IEnumerator rotate(float offset) {
        Debug.Log("start...");
        rotating = true;
        Vector3 start = transform.localRotation.eulerAngles;
        Vector3 tar = new Vector3(0, start.y + offset, 0);
        Debug.Log(start + ", " + tar);
        float startTime = Time.time;
        while (transform.localRotation.eulerAngles.y != tar.y) {
            yield return new WaitForEndOfFrame();
        }
        rotating = false;
        Debug.Log("over...");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.R)) {
            transform.Rotate(new Vector3(0, 10, 0), Space.Self);
        }
	}
}
