using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Collider : MonoBehaviour {

	[SerializeField]
	private BoxCollider2D _col;

	void OnTriggerEnter2D(Collider2D col)
	{
		var enemy = col.GetComponent<Enemy>();
		if(!Player_Controller.instance.GetTrack(enemy.trackType).active){
		enemy.Collect();
		}
	}
}