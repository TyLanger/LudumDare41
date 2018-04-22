using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageWakeUp : MonoBehaviour {

	public Transform player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.position;
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Enemy")
		{
			col.GetComponent<Mage> ().WakeUp (true);
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.tag == "Enemy")
		{
			col.GetComponent<Mage> ().WakeUp (false);
		}
	}
}
