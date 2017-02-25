using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player_Collider : MonoBehaviour
{

	[SerializeField]
	private CircleCollider2D _col;

	[SerializeField] private ParticleSystem collectParticles;

	[SerializeField] private AudioClip[] hits;

	[SerializeField] private SpriteRenderer _sprite;

	private float beatPunchIntensity = 0.5f;

	private Enemy _currentEnemy;

	private void Start ()
	{
		AudioManager.instance.BeatOnUpdate += OnBeat;
	}

	private void OnBeat ()
	{
		float dur = 60f / (float)AudioManager.instance.bpm - 0.1f;
		transform.DOPunchScale (new Vector3 (beatPunchIntensity, beatPunchIntensity, beatPunchIntensity), dur);
	}

	public void OnEnemyCollected (Enemy enemy)
	{
		var mainModule = collectParticles.main;
		Color c = ColorManager.GetColorForTrackType (enemy.trail.trackType);
		c.a = 0.5f;
		mainModule.startColor = c;
		collectParticles.Play ();
		AudioClip hit = hits [(int)enemy.trail.trackType];
		AudioManager.instance.PlaySound (hit, 0.5f);
	}

	void Update ()
	{
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2.5f, Vector2.zero, Mathf.Infinity);
		if (hits.Length > 0) {
			_sprite.color = new Color(1,1,1,.85f);
			if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)) {
				for(int i = 0; i < hits.Length; i++) {
					var enemy = hits[i].transform.GetComponent<Enemy>();
					if (!enemy.collected) {
						enemy.AttemptCollect ();
					}
				}
			}
		}
		else {
			_sprite.color = new Color(1,1,1,.5f);
		}
	}
}