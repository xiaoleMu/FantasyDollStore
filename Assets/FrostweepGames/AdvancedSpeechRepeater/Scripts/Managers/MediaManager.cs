using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.SpeechRepeater
{
    public class MediaManager : IService, IMediaManager
    {
        public event Action StartedListeningEvent;
        public event Action<AudioClip> FinishedListeningEvent;
        public event Action ListeningFailedEvent;

        public event Action EndAudioPlayingEvent;

        public event Action BeginTalkigEvent;
        public event Action<AudioClip> EndTalkigEvent;

        private AudioClip _microphoneWorkingAudioClip;
        private AudioClip _latestVoiceAudioClip;
        private string _microphoneDevice;

        private int _currentSamplePosition;
        private int _previousSamplePosition;
        private float[] _currentAudioSamples;

        private bool _isTalking,
                     _isPlayingAudio;


        private List<float> _currentRecordingVoice;

        private IVoiceDetectionManager _voiceDetectionManager;
        private AdvancedSpeechRepeater _speechRepeater;

        private SoundContainer _soundContainer;

        private float _delayTimer = 0f;


        public bool IsCanWork { get; set; }
        public bool IsListening { get; set; }

        public void Init()
        {
            _speechRepeater = AdvancedSpeechRepeater.Instance;
            _voiceDetectionManager = _speechRepeater.ServiceLocator.Get<IVoiceDetectionManager>();

            _soundContainer = new SoundContainer();
            _soundContainer.Init(_speechRepeater.audioConfig, _speechRepeater.transform);

            CheckMicrophones();
        }

        public void StartListening()
        {
            if (IsListening || !IsCanWork)
                return;

            _currentRecordingVoice = new List<float>();

            if (_microphoneWorkingAudioClip != null)
                MonoBehaviour.Destroy(_microphoneWorkingAudioClip);

            ClearCache();

            _microphoneWorkingAudioClip = Microphone.Start(_microphoneDevice, true, 1, _speechRepeater.config.sampleRate);

            _currentAudioSamples = new float[_microphoneWorkingAudioClip.samples * _microphoneWorkingAudioClip.channels];

            IsListening = true;

            if (StartedListeningEvent != null)
                StartedListeningEvent();
        }


        public void StopListening()
        {
            if (!IsListening || !IsCanWork)
                return;

            IsListening = false;

            Microphone.End(_microphoneDevice);

            _currentAudioSamples = null;
            _currentRecordingVoice = null;

            if (FinishedListeningEvent != null)
                FinishedListeningEvent(_latestVoiceAudioClip);
        }

        private void CheckMicrophones()
        {
            if (Microphone.devices.Length > 0)
            {
                _microphoneDevice = Microphone.devices[0];
                IsCanWork = true;
            }
            else
            {
                Debug.Log("Microphone devices not found!");
                IsCanWork = false;
            }
        }

        public void Update()
        {
            _soundContainer.Update();

            if (_soundContainer.IsPlaying || !IsListening)
                return;

            if (_isPlayingAudio)
            {
                _delayTimer += Time.deltaTime;

                if(_delayTimer >= _speechRepeater.config.delayBeforeStartListening)
                {
                    _isPlayingAudio = false;

                    if (EndAudioPlayingEvent != null)
                        EndAudioPlayingEvent();

                    _delayTimer = 0f;
                }
            }

            if (IsListening && !_isPlayingAudio)
            {
                _currentSamplePosition = Microphone.GetPosition(_microphoneDevice);
                _microphoneWorkingAudioClip.GetData(_currentAudioSamples, 0);

                bool isTalking = _voiceDetectionManager.CheckVoice(AudioClip2ByteConverter.FloatToByte(_currentAudioSamples));

                if (!_isTalking && isTalking)
                {
                    _isTalking = true;

                    if (BeginTalkigEvent != null)
                        BeginTalkigEvent();
                }
                else if (_isTalking && !isTalking)
                {
                    _isTalking = false;

                    ClearCache();

                     _latestVoiceAudioClip = MakeAudioClipFromSamples(_currentRecordingVoice.ToArray());

                    if (EndTalkigEvent != null)
                        EndTalkigEvent(_latestVoiceAudioClip);

                    _currentRecordingVoice.Clear();

                    _soundContainer.ConnectAudioClip(_latestVoiceAudioClip);

                    _isPlayingAudio = true;
                }
                else if (_isTalking)
                {
                    AddVoiceSamples();
                }

                _previousSamplePosition = _currentSamplePosition;
            }
            else
            {
                _previousSamplePosition = Microphone.GetPosition(_microphoneDevice);
            }
        }

        public void Dispose()
        {
            if (_microphoneWorkingAudioClip != null)
                MonoBehaviour.Destroy(_microphoneWorkingAudioClip);
        }

        private void ClearCache()
        {
            if (_latestVoiceAudioClip != null)
                MonoBehaviour.Destroy(_latestVoiceAudioClip);
        }

        private void AddVoiceSamples()
        {
            if (_previousSamplePosition > _currentSamplePosition)
            {
                for (int i = _previousSamplePosition; i < _currentAudioSamples.Length; i++)
                    _currentRecordingVoice.Add(_currentAudioSamples[i]);

                _previousSamplePosition = 0;
            }

            for (int i = _previousSamplePosition; i < _currentSamplePosition; i++)
                _currentRecordingVoice.Add(_currentAudioSamples[i]);

            _previousSamplePosition = _currentSamplePosition;
        }

        private AudioClip MakeAudioClipFromSamples(float[] samples)
        {
            AudioClip clip;

            clip = AudioClip.Create("RecordedVoice", samples.Length, _microphoneWorkingAudioClip.channels, _speechRepeater.config.sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }
    }
}