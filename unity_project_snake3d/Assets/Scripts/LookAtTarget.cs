using UnityEngine;

/// <summary>
/// Attaching this script to an object will make that object face the specified target.
/// The most ideal use for this script is to attach it to the camera and make the camera look at its target.
/// </summary>

[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour
{
	public int level = 0;
	public Transform target;
	public float speed = 8f;

	Transform mTrans;

	void Start ()
	{
		mTrans = transform;
        if (target == null)
        {
            target = Camera.main.gameObject.transform;
        }
    }

	void LateUpdate ()
	{
		if (enabled && target != null)
		{
            Vector3 rot = mTrans.eulerAngles;
			Vector3 dir = target.position - mTrans.position;
            //dir.y = 0f;
            float mag = dir.magnitude;

			if (mag > 0.001f)
			{
				Quaternion lookRot = Quaternion.LookRotation(dir);

                rot.y = lookRot.eulerAngles.y + 90f;
                rot.z = (lookRot.eulerAngles.x - 360f) * 0.2f + 90f;
                lookRot = Quaternion.Euler(rot);
				mTrans.rotation = Quaternion.Slerp(mTrans.rotation, lookRot, Mathf.Clamp01(speed * Time.deltaTime));
			}
		}
	}
}