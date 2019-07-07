using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

namespace TabTale
{
	[RequireComponent(typeof(AudioListener))]

	public class SoundManagerPlayer : MonoBehaviour
	{
		private Dictionary<SoundLayer, List<SoundManagerClip>> _layers;
		private Dictionary<string, SoundManagerClip> _continuosLooping;
		int i=0;
		public AudioMixer masterMixer;

		public class SoundManagerClip
		{
			public string _audioClipPath;
			public AudioSource _audiSource;
			public bool _isPaused;
			public float _playUntil = 0;

			public void LoadSoundClip()
			{
				_audiSource.clip = Resources.Load<AudioClip>(_audioClipPath);
			}

			public void PauseClip()
			{
				_isPaused = true;
				_audiSource.Pause();
			}

			public void UnPauseClip()
			{
				_isPaused = false;
				_audiSource.Play();
			}

			public void MuteClip()
			{
				_audiSource.mute = true;
			}

			public void UnmuteClip()
			{
				_audiSource.mute = false;
			}
		}

		void Awake()
		{
			_layers = new Dictionary<SoundLayer, List<SoundManagerClip>>();
			_continuosLooping = new Dictionary<string, SoundManagerClip>();

			foreach (SoundLayer layer in Enum.GetValues(typeof(SoundLayer)))
				_layers.Add(layer, new List<SoundManagerClip>());

			//masterMixer = Resources.Load<AudioMixer>("TTMasterMixer") as AudioMixer;
			masterMixer = GameApplication.LoadFrameworkResource<AudioMixer> ("TTMasterMixer");
			if (masterMixer == null) {
				masterMixer = Resources.Load<AudioMixer>("TTMasterMixer");
			}
		}

		void Update()
		{
			if (_continuosLooping != null && _continuosLooping.Count > 0)
				RemoveExpiredContinuosClips();
		}

		private void RemoveExpiredContinuosClips()
		{
			List<string> clipsToRemove = null;

			foreach (var kvp in _continuosLooping.Where((cc) => cc.Value._playUntil < Time.time || !cc.Value._audiSource.isPlaying))
			{
				if (clipsToRemove == null)
					clipsToRemove = new List<string>();

				clipsToRemove.Add(kvp.Key);
				kvp.Value._audiSource.Stop();
			}

			if (clipsToRemove != null)
				foreach (var clip in clipsToRemove)
					_continuosLooping.Remove(clip);
		}

		public SoundManagerClip PlaySound(string clipName, SoundLayer layer, bool isLooping = false)
		{
			SoundManagerClip smc = FindFreeAudioSource(layer, clipName);
			smc._audioClipPath = clipName;
			smc.LoadSoundClip();
			smc._audiSource.loop = isLooping;
			smc._audiSource.Play();
			return smc;
		}

//		public SoundManagerClip PlaySound(AudioClip clip, SoundLayer layer, bool isLooping = false)
//		{
//			SoundManagerClip smc = FindFreeAudioSource(layer, clipName);
//			smc._audioClipPath = clipName;
//			smc.LoadSoundClip();
//			smc._audiSource.clip = clip；
//			smc._audiSource.loop = isLooping;
//			smc._audiSource.Play();
//			return smc;
//		}

		public bool IsPlaying(string clipName, SoundLayer layer)
		{
			bool isPlaying = false;

			foreach (SoundManagerClip clip in _layers[layer])
			{
				if (clip._audiSource != null && clip._audiSource.clip != null) 
				{
					if (clip._audioClipPath == clipName && clip._audiSource.isPlaying)
						isPlaying = true;
				}
			}

			return isPlaying;
		}

		public SoundManagerClip PlayTimedSound(string clipName, SoundLayer layer, float timeToPlay)
		{
			SoundManagerClip cc = null;

			if (_continuosLooping.ContainsKey(clipName))
			{
				cc = _continuosLooping[clipName];
				cc._playUntil = Time.time + timeToPlay;
			}
			else
			{
				cc = FindFreeAudioSource(layer, clipName);
				cc._audioClipPath = clipName;
				cc._playUntil = Time.time + timeToPlay;
				cc._audiSource.loop = true;
				cc.LoadSoundClip();
				cc._audiSource.Play();

				_continuosLooping.Add(cc._audioClipPath, cc);
			}

			return cc;
		}

		public void StopAllSoundsOnLayer(SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer])
			{
				if (clip._audiSource != null)
					clip._audiSource.Stop();
			}
		}

		public void StopSound(string clipName, SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer])
			{
				if (clip._audiSource != null && clip._audioClipPath == clipName)
					clip._audiSource.Stop ();
			}
		}
	
		public void MuteLayer(SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer])
				clip.MuteClip();
		}

	

		public void UnmuteLayer(SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer])
				clip.UnmuteClip();
		}

	

		public void StartAllSoundsOnLayer(SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer])
				clip._audiSource.Play();
		}

		public void PauseSoundsOnLayer(SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer].Where(c => c._audiSource.isPlaying))
				clip.PauseClip();
		}

		public void UnPauseSoundsOnLayer(SoundLayer layer)
		{
			foreach (SoundManagerClip clip in _layers[layer].Where(c => c._isPaused))
				clip.UnPauseClip();
		}

		public AudioMixerGroup SetAudioGroup (SoundLayer layer)
		{
			AudioMixerGroup temp = null;

			if (layer == SoundLayer.Main) 
			{
				temp = masterMixer.FindMatchingGroups("Main")[0];
				Debug.Log("SetAudioGroup to: " + temp.name);
			}
			else 
			{	
				temp = masterMixer.FindMatchingGroups("Music")[0];
				Debug.Log("SetAudioGroup to: " + temp.name);
			}

			return temp;
		}

		SoundManagerClip FindFreeAudioSource(SoundLayer layer, string clipName)
		{
			AudioSource tempAudio = null;

			foreach (SoundManagerClip clip in _layers[layer]) 
			{

				if(clip._audioClipPath == clipName)
				{
					tempAudio = clip._audiSource;
				}
				if (!clip._audiSource.isPlaying && !clip._isPaused)
					return clip;
			}

			SoundManagerClip smc = new SoundManagerClip();

			if (tempAudio == null) 
			{
				smc._audiSource = gameObject.AddComponent<AudioSource> ();
			} 
			else 
			{
				smc._audiSource = tempAudio;
			}

			smc._audiSource.outputAudioMixerGroup = SetAudioGroup (layer);
			smc._audiSource.playOnAwake = false;
			smc._audioClipPath = "";
			_layers[layer].Add(smc);
			return smc;
		}
	}
}
