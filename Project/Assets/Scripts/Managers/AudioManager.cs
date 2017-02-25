using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public enum TrackTypes
	{
		Bass = 0,
		Clav = 1,
		Keys = 2,
		Pizz = 3,
		Drums = 4
	}

	[SerializeField]
	private AudioSource _bassTrack, _clavTrack, _keyTrack, _pizzTrack, _drumTrack, _buildTrack, _dropTrack;

	public AudioSource buildTrack {
		get{ return _buildTrack; }
	}

	[SerializeField]
	private double _bpm = 160.0F;

	[SerializeField] private AudioSource sfxSource;

	[SerializeField] private AudioSource _sfxWhiteNoise;

	public AudioSource sfxWhiteNoise {
		get{ return _sfxWhiteNoise; }
	}

	[SerializeField]
	private AudioClip _whiteNoiseClip;

	public AudioClip whiteNoiseClip {
		get{ return _whiteNoiseClip; }
	}

	public void PlaySound (AudioClip clip, float volume = 1)
	{
		sfxSource.PlayOneShot (clip, volume);
	}

	public double bpm {
		get { return _bpm; }
	}

	[SerializeField]
	private float _gain = 0.5F;
	[SerializeField]
	private int _signatureHi = 4;
	[SerializeField]
	private int _signatureLo = 4;
	private double _nextTick = 0.0F;
	private float _amp = 0.0F;
	private float _phase = 0.0F;
	private double _sampleRate = 0.0F;
	private int _accent;
	private bool _running = false;

	public int numBeatsPerSegment = 16;
	private double nextEventTime;
	private int flip = 0;

	public delegate void BeatHandler ();

	public BeatHandler BeatExact;
	public BeatHandler BeatOnUpdate;

	private bool sendBeatSignal = false, _triggerDrop = false, _initPlay = false, _waitForBuild = false;

	private double _dropTime;

	void Awake ()
	{
		if (!instance)
			instance = this;
	}

	void Update ()
	{
		if (sendBeatSignal) {
			BeatOnUpdate ();
			sendBeatSignal = false;
		}
			

		if (!_running)
			return;

		double time = AudioSettings.dspTime;
		if (time + 1.0F > nextEventTime) {
			if (_triggerDrop) {
				_triggerDrop = false;
				_waitForBuild = true;
				_buildTrack.PlayScheduled (nextEventTime);
				_dropTime = nextEventTime + ((60.0F / bpm * numBeatsPerSegment) * 2f);
				_dropTrack.PlayScheduled (_dropTime);
				StartCoroutine (Player_Controller.instance.YieldToDropEnable ());
				Debug.Log ("Scheduled source to start at time " + nextEventTime);
			}
			if (_initPlay) {
				_initPlay = false;
				PlayBeats (nextEventTime);
			}
				
			nextEventTime += 60.0F / bpm * numBeatsPerSegment;
		}

		if (_waitForBuild) {
			if (AudioSettings.dspTime >= _dropTime) {
				_waitForBuild = false;

				if (Player_Controller.instance.dropHit)
					PlayDrop ();
				else {
					_dropTrack.volume = 0;
					Player_Controller.instance.ResetToBass ();
				}
			}
		}
	}

	void PlayBeats (double eventTime)
	{
		_dropTrack.Stop ();
		_buildTrack.Stop ();

		_bassTrack.PlayScheduled (eventTime);
		_clavTrack.PlayScheduled (eventTime);
		_keyTrack.PlayScheduled (eventTime);
		_pizzTrack.PlayScheduled (eventTime);
		_drumTrack.PlayScheduled (eventTime);

		_bassTrack.volume = 1f;
		_clavTrack.volume = 0;
		_keyTrack.volume = 0;
		_pizzTrack.volume = 0;
		_drumTrack.volume = 0;
	}

	public void TriggerDrop ()
	{
		_triggerDrop = true;
	}


	void PlayDrop ()
	{
		_dropTrack.volume = 1;
		_bassTrack.volume = 0;
		_clavTrack.volume = 0;
		_keyTrack.volume = 0;
		_pizzTrack.volume = 0;
		_drumTrack.volume = 0;

		StartCoroutine (Player_Controller.instance.PlayDrop ());
	}

	void OnAudioFilterRead (float[] data, int channels)
	{
		if (!_running)
			return;

		double samplesPerTick = _sampleRate * 60.0F / _bpm * 4.0F / _signatureLo;
		double sample = AudioSettings.dspTime * _sampleRate;
		int dataLen = data.Length / channels;
		int n = 0;
		while (n < dataLen) {
			float x = _gain * _amp * Mathf.Sin (_phase);
			int i = 0;
			while (i < channels) {
				data [n * channels + i] += x;
				i++;
			}
			while (sample + n >= _nextTick) {
				_nextTick += samplesPerTick;
				_amp = 1.0F;
				if (++_accent > _signatureHi) {
					_accent = 1;
					_amp *= 2.0F;
				}
//				Debug.Log ("Tick: " + _accent + "/" + _signatureHi);
				OnBeat ();
			}
			_phase += _amp * 0.3F;
			_amp *= 0.993F;
			n++;
		}
	}

	private void OnBeat ()
	{
		if (BeatExact != null)
			BeatExact ();
		sendBeatSignal = true;
	}

	public void PlayTracks ()
	{
		_accent = _signatureHi;
		double startTick = AudioSettings.dspTime;
		_sampleRate = AudioSettings.outputSampleRate;
		_nextTick = startTick * _sampleRate;
		nextEventTime = AudioSettings.dspTime + 2.0F;
		_running = true;
		_initPlay = true;
	}

	public void RestartBeats ()
	{
		if (_dropTrack.isPlaying) {
			_dropTrack.Stop ();
			VolumeUpAllTracks ();
		}
	}

	void VolumeUpAllTracks ()
	{
		_bassTrack.volume = 1f;
		if (Player_Controller.instance.GetClavTrail ().active && _clavTrack.volume == 0)
			_clavTrack.volume = 1f;
		if (Player_Controller.instance.GetKeysTrail ().active && _keyTrack.volume == 0)
			_keyTrack.volume = 1f;
		if (Player_Controller.instance.GetPizzTrail ().active && _pizzTrack.volume == 0)
			_pizzTrack.volume = 1f;
		if (Player_Controller.instance.GetDrumTrail ().active && _drumTrack.volume == 0)
			_drumTrack.volume = 1f;
	}

	public AudioSource GetTrack (TrackTypes type)
	{
		AudioSource track = null;
		switch (type) { 
		case TrackTypes.Bass:
			track = _bassTrack;
			break;
		case TrackTypes.Clav:
			track = _clavTrack;
			break;
		case TrackTypes.Keys:
			track = _keyTrack;
			break;
		case TrackTypes.Pizz:
			track = _pizzTrack;
			break;
		case TrackTypes.Drums:
			track = _drumTrack;
			break;
		}
		return track;
	}
}
