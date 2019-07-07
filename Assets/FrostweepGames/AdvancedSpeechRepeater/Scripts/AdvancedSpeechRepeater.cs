using System;
using UnityEngine;


namespace FrostweepGames.Plugins.SpeechRepeater
{
    public class AdvancedSpeechRepeater : MonoBehaviour
    {
        public event Action ListeningFailedEvent;
        public event Action StartedListeningEvent;
        public event Action<AudioClip> FinishedListeningEvent;
        public event Action BeginTalkigEvent;
        public event Action<AudioClip> EndTalkigEvent;
        public event Action EndAudioPlayingEvent;

        private static AdvancedSpeechRepeater _Instance;
        public static AdvancedSpeechRepeater Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = MonoBehaviour.Instantiate(Resources.Load<GameObject>("AdvancedSpeechRepeater")).GetComponent<AdvancedSpeechRepeater>();
                    _Instance.gameObject.name = "[Singleton]Advanced Speech Repeater";
                }

                return _Instance;
            }
        }


        private ServiceLocator _serviceLocator;

        private IMediaManager _mediaManager;


        public ServiceLocator ServiceLocator { get { return _serviceLocator; } }


        [Header("Prefab Object Settings")]
        public bool isDontDestroyOnLoad = false;

        [Header("Voice Detection Settings")]
        public Config config;

        [Header("Audio Playing Settings")]
        public AudioConfig audioConfig;


        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            _Instance = this;

            _serviceLocator = new ServiceLocator();
            _serviceLocator.InitServices();

            _mediaManager = _serviceLocator.Get<IMediaManager>();

            _mediaManager.StartedListeningEvent += StartedRecordEventHandler;
            _mediaManager.FinishedListeningEvent += FinishedRecordEventHandler;
            _mediaManager.ListeningFailedEvent += RecordFailedEventHandler;
            _mediaManager.BeginTalkigEvent += BeginTalkigEventHandler;
            _mediaManager.EndTalkigEvent += EndTalkigEventHandler;
            _mediaManager.EndAudioPlayingEvent += EndAudioPlayingEventHandler;
            
        }

        private void Update()
        {
            if (_Instance == this)
            {
                _serviceLocator.Update();
            }
        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _mediaManager.StartedListeningEvent -= StartedRecordEventHandler;
                _mediaManager.FinishedListeningEvent -= FinishedRecordEventHandler;
                _mediaManager.ListeningFailedEvent -= RecordFailedEventHandler;
                _mediaManager.BeginTalkigEvent -= BeginTalkigEventHandler;
                _mediaManager.EndTalkigEvent -= EndTalkigEventHandler;
                _mediaManager.EndAudioPlayingEvent -= EndAudioPlayingEventHandler;

                _Instance = null;
                _serviceLocator.Dispose();
            }
        }

        public void StartListening()
        {
            _mediaManager.StartListening();
        }

        public void StopListening()
        {
            _mediaManager.StopListening();
        }

        private void RecordFailedEventHandler()
        {
            if (ListeningFailedEvent != null)
                ListeningFailedEvent();
        }

        private void BeginTalkigEventHandler()
        {
            if (BeginTalkigEvent != null)
                BeginTalkigEvent();
        }

        private void EndAudioPlayingEventHandler()
        {
            if (EndAudioPlayingEvent != null)
                EndAudioPlayingEvent();
        }

        private void EndTalkigEventHandler(AudioClip clip)
        {
            if (EndTalkigEvent != null)
                EndTalkigEvent(clip);
        }

        private void StartedRecordEventHandler()
        {
            if (StartedListeningEvent != null)
                StartedListeningEvent();
        }

        private void FinishedRecordEventHandler(AudioClip clip)
        {
            if (FinishedListeningEvent != null)
                FinishedListeningEvent(clip);
        }

    }
}