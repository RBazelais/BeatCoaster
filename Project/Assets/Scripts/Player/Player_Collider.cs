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
		if (_currentEnemy != null) {
			if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift)) {
				if (!_currentEnemy.collected) {
					_currentEnemy.AttemptCollect ();
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		
		_currentEnemy = col.GetComponent<Enemy> ();
		_sprite.color = ColorManager.GetColorForTrackType (_currentEnemy.trackType);
		_sprite.color = new Color (_sprite.color.r, _sprite.color.g, _sprite.color.b, .5f);
	}

	void OnTriggerExit2D (Collider2D col)
	{
		_sprite.color = new Color (1, 1, 1, .5f);
		_currentEnemy = null;
	}


}