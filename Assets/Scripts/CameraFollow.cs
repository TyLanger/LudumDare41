using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform player;
	public Vector3 offset;

	// Screen shake from https://www.youtube.com/watch?v=tu-Qe66AvtY
	public float trauma = 0;
	public float traumaLoss = 0.1f;
	public float maxShake = 0.4f;
	public float maxShakeAngle = 5;

	Vector3 shakeOffset;
	float shakeAngle;

	// Use this for initialization
	void Start () {
		shakeOffset = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		shakeOffset = new Vector3 (maxShake * trauma * trauma * trauma * Random.Range (-1.0f, 1.0f), 0, maxShake * trauma * trauma * trauma * Random.Range (-1.0f, 1.0f));
		shakeAngle = maxShakeAngle * trauma * trauma * trauma * Random.Range (-1.0f, 1.0f);

		transform.position = player.position + offset + shakeOffset;
		transform.rotation = Quaternion.Euler (45, 90, 0 + shakeAngle);

		if (trauma > 0) {
			trauma -= traumaLoss;
			trauma = Mathf.Clamp01 (trauma);
		}


	}

	public void AddScreenShake(float amount)
	{
		trauma += amount;
		if (trauma > 1) {
			trauma = 1;
		}
	}
}
