using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player_Collider : MonoBehaviour {

	[SerializeField]
	private BoxCollider2D _col;

	void OnTriggerEnter2D(Collider2D col)
	{
		var enemy = col.GetComponent<Enemy>();
		if(!enemy.collected){
			enemy.Collect();
		}
	}
}