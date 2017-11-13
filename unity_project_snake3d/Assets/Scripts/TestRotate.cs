using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour {
    public Vector3 tarAngle;
    public Vector3 rotateAngle;

    bool rotating = false;

    IEnumerator rotate(bool flag) {
        rotating = true;

        Vector3 angle;
        if (flag) {
            angle = tarAngle - transform.localRotation.eulerAngles;
        } else {
            angle = rotateAngle;
        }
        Debug.Log(angle);
        
        float startTime = Time.time;
        while (true) {
            transform.Rotate(angle * Time.deltaTime, Space.Self);
            if ((Time.time - startTime) >= 1f) {
                break;
            } else {
                yield return new WaitForEndOfFrame();
            }
        }
        rotating = false;
        Debug.Log("over");
    }
	
	// Update is called once per frame
	void Update () {
        if (rotating) {
            return;
        }
        if (Input.GetKey(KeyCode.R)) {
            StartCoroutine(rotate(true));
        }
        if (Input.GetKey(KeyCode.T)) {
            StartCoroutine(rotate(false));
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.localEulerAngles = tarAngle;
        }
	}
}
