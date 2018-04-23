using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrigger : MonoBehaviour {


	public bool startLine = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") {
			if (startLine) {
				col.GetComponent<Player> ().StartRace ();
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		
		if (col.tag == "Player") {
			Debug.Log ("Hit a trigger "+startLine);
			if (!startLine) {
				col.GetComponent<Player> ().EndRace ();
			}
		}
	}
}
