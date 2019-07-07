using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FrostweepGames.Plugins.SpeechRepeater
{
    public class SoundContainer : IDisposable
    {
        private GameObject _selfObject;

        private AudioSource _audioSource;
        private AudioListener _audioListener;

        private AudioConfig _audioConfig;

        private bool _isInited = false;

        public bool IsPlaying { get; private set; }

        public void Init(AudioConfig config, Transform parent)
        {
            _audioConfig = config;

            _selfObject = new GameObject("[Container] Audio");
            _selfObject.transform.SetParent(parent, false);

            _audioSource = _selfObject.AddComponent<AudioSource>();


            if (MonoBehaviour.FindObjectOfType<AudioListener>() == null)
                ConnectAudioListener();

            _isInited = true;
        }

        public void Update()
        {
            if (!_isInited)
                return;

            if (IsPlaying && !_audioSource.isPlaying)
                IsPlaying = false;
        }

        public void Dispose()
        {
            _isInited = false;

            MonoBehaviour.Destroy(_selfObject);

            _audioListener = null;
            _audioSource = null;
        }

        public void ConnectAudioClip(AudioClip clip)
        {
            if (!_isInited)
                return;

            ApplyEffects();

            _audioSource.clip = clip;
            _audioSource.Play();

            IsPlaying = true;
        }

        private void ConnectAudioListener()
        {
            _audioListener = _selfObject.AddComponent<AudioListener>();
        }

        private void ApplyEffects()
        {
            _audioSource.outputAudioMixerGroup = _audioConfig.outputs.Find(x => x.audioMixerOutputType == _audioConfig.currentAudioMixerOutputType).mixer;
            _audioSource.volume = _audioConfig.volume;
            _audioSource.spatialBlend = _audioConfig.spatialBlend;
            _audioSource.priority = _audioConfig.priority;
            _audioSource.pitch = _audioConfig.pitch;
            _audioSource.panStereo = _audioConfig.stereoPan;
            _audioSource.mute = _audioConfig.mute;
            _audioSource.bypassEffects = _audioConfig.bypassEffects;
            _audioSource.bypassListenerEffects = _audioConfig.bypassListenerEffects;
            _audioSource.bypassReverbZones = _audioConfig.bypassReverbZones;
            _audioSource.playOnAwake = _audioConfig.playOnAwake;
        }

    }

    [Serializable]
    public class AudioConfig
    {
        [Range(0f, 1f)]
        public float volume = 1f,
                     spatialBlend = 0f;

        [Range(0, 256)]
        public byte priority = 128;

        [Range(-3f, 3f)]
        public float pitch = 1f;

        [Range(-1f, 1f)]
        public float stereoPan = 0f;

        public bool mute = false,
                    bypassEffects = false,
                    bypassListenerEffects = false,
                    bypassReverbZones = false,
                    playOnAwake = true;


        public Enumerators.AudioMixerOutputType currentAudioMixerOutputType = Enumerators.AudioMixerOutputType.DEFAULT;
        public List<AudioMixerOutput> outputs = new List<AudioMixerOutput>();
    }

    [Serializable]
    public class Config
    {
        public int sampleRate = 44100;
        public double voiceDetectionThreshold = 0.2f;

        public bool useVolumeMultiplier = false;
        public float audioVolumeMultiplier = 1f;

        [Range(0f, 60f)]
        public float delayBeforeStartListening = 1f;
    }

    [Serializable]
    public class AudioMixerOutput
    {
        public Enumerators.AudioMixerOutputType audioMixerOutputType;
        public AudioMixerGroup mixer;
    }
}