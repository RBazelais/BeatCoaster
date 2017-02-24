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
	private AudioSource _bassTrack, _clavTrack, _keyTrack, _pizzTrack, _drumTrack;

	[SerializeField]
	private double _bpm = 160.0F;
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

	public delegate void BeatHandler ();
	public BeatHandler BeatExact;
	public BeatHandler BeatOnUpdate;

	private bool sendBeatSignal = false;

	void Awake() {
		if(!instance)
			instance = this;
	}

	void Update()
	{
		if (sendBeatSignal) {
			BeatOnUpdate();
			sendBeatSignal = false;
		}
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
				OnBeat();
			}
			_phase += _amp * 0.3F;
			_amp *= 0.993F;
			n++;
		}
	}

	private void OnBeat() {
		if (BeatExact != null) BeatExact();
		sendBeatSignal = true;
	}

	public void PlayTracks ()
	{
		_bassTrack.Play ();
		_clavTrack.Play ();
		_keyTrack.Play ();
		_pizzTrack.Play ();
		_drumTrack.Play ();

		_accent = _signatureHi;
		double startTick = AudioSettings.dspTime;
		_sampleRate = AudioSettings.outputSampleRate;
		_nextTick = startTick * _sampleRate;
		_running = true;
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
