﻿using System.Collections;
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

	public int numBeatsPerSegment = 16;
	private double nextEventTime;
	private int flip = 0;

	[SerializeField]
	private AudioSource[] audioSources = new AudioSource[2];

	[SerializeField]
	public AudioClip[] clips = new AudioClip[2];

	public delegate void BeatHandler ();

	public BeatHandler BeatExact;
	public BeatHandler BeatOnUpdate;

	private bool sendBeatSignal = false, _triggerDrop = false, _initPlay = false;

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

		if(!_running)
			return;

		double time = AudioSettings.dspTime;
		if (time + 1.0F > nextEventTime) {
			if (_triggerDrop) {
				_triggerDrop = false;
				_buildTrack.PlayScheduled (nextEventTime);
				_dropTrack.PlayScheduled (nextEventTime + ((60.0F / bpm * numBeatsPerSegment)*2f));
			}
			if(_initPlay) {
				_initPlay = false;
				PlayBeats(nextEventTime);
			}

			Debug.Log ("Scheduled source to start at time " + nextEventTime);
			nextEventTime += 60.0F / bpm * numBeatsPerSegment;
		}
	}

				void PlayBeats(double eventTime) {
					_bassTrack.PlayScheduled (eventTime);
					_clavTrack.PlayScheduled (eventTime);
					_keyTrack.PlayScheduled (eventTime);
					_pizzTrack.PlayScheduled (eventTime);
					_drumTrack.PlayScheduled (eventTime);
				}

	public void TriggerDrop ()
	{
		_triggerDrop = true;
	}


	void PlayDrop ()
	{
		_bassTrack.volume = 0;
		_clavTrack.volume = 0;
		_keyTrack.volume = 0;
		_pizzTrack.volume = 0;
		_drumTrack.volume = 0;
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
