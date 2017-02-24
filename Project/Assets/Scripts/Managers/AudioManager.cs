using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public enum TrackTypes
	{
		Bass,
		Clav,
		Keys,
		Pizz,
		Drums
	}

	[SerializeField]
	private AudioSource _bassTrack, _clavTrack, _keyTrack, _pizzTrack, _drumTrack;

	public double bpm = 160.0F;
	public float gain = 0.5F;
	public int signatureHi = 4;
	public int signatureLo = 4;
	private double nextTick = 0.0F;
	private float amp = 0.0F;
	private float phase = 0.0F;
	private double sampleRate = 0.0F;
	private int accent;
	private bool running = false;

	void OnAudioFilterRead (float[] data, int channels)
	{
		if (!running)
			return;

		double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
		double sample = AudioSettings.dspTime * sampleRate;
		int dataLen = data.Length / channels;
		int n = 0;
		while (n < dataLen) {
			float x = gain * amp * Mathf.Sin (phase);
			int i = 0;
			while (i < channels) {
				data [n * channels + i] += x;
				i++;
			}
			while (sample + n >= nextTick) {
				nextTick += samplesPerTick;
				amp = 1.0F;
				if (++accent > signatureHi) {
					accent = 1;
					amp *= 2.0F;
				}
				Debug.Log ("Tick: " + accent + "/" + signatureHi);
			}
			phase += amp * 0.3F;
			amp *= 0.993F;
			n++;
		}
	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.Space) && !_bassTrack.isPlaying)
		{
			PlayTracks();
		}
	}

	void PlayTracks ()
	{
		_bassTrack.Play();
		_clavTrack.Play();
		_keyTrack.Play();
		_pizzTrack.Play();
		_drumTrack.Play();

		accent = signatureHi;
		double startTick = AudioSettings.dspTime;
		sampleRate = AudioSettings.outputSampleRate;
		nextTick = startTick * sampleRate;
		running = true;
	}

	AudioSource GetTrack (TrackTypes type)
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
