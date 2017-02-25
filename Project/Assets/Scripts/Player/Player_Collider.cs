using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player_Collider : MonoBehaviour {

	[SerializeField]
	private BoxCollider2D _col;

	[SerializeField] private ParticleSystem collectParticles;

	[SerializeField] private AudioClip[] hits;

	private float beatPunchIntensity = 0.5f;

	private void Start() {
		AudioManager.instance.BeatOnUpdate += OnBeat;
	}

	private void OnBeat() {
		float dur = 60f / (float)AudioManager.instance.bpm - 0.1f;
		transform.DOPunchScale(new Vector3(beatPunchIntensity, beatPunchIntensity, beatPunchIntensity), dur);
	}

	public void OnEnemyCollected(Enemy enemy) {
		var mainModule = collectParticles.main;
		Color c = ColorManager.GetColorForTrackType(enemy.trail.trackType);
		c.a = 0.5f;
		mainModule.startColor = c;
		collectParticles.Play();
		AudioClip hit = hits[(int)enemy.trail.trackType];
		AudioManager.instance.PlaySound(hit, 0.5f);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		var enemy = col.GetComponent<Enemy>();
		if(!enemy.collected){
			enemy.AttemptCollect();
		}
	}
}