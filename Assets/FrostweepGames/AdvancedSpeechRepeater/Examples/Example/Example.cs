using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.SpeechRepeater
{
    public class Example : MonoBehaviour
    {
        private Button _startListeningButton,
                       _stopListeningButton;

        private Image _statusImage;

        private AdvancedSpeechRepeater _speechRepeater;

        public GameObject rootObject;

        // Should be used Start function because AdvancedSpeechRepeater.Instance inited in Awake function!
        private void Start()
        {
            _speechRepeater = AdvancedSpeechRepeater.Instance;

            _statusImage = rootObject.transform.Find("Image_Status").GetComponent<Image>();

            _startListeningButton = rootObject.transform.Find("Button_StartListening").GetComponent<Button>();
            _stopListeningButton = rootObject.transform.Find("Button_StopListening").GetComponent<Button>();

            _startListeningButton.onClick.AddListener(StartListeningButtonOnClickHandler);
            _stopListeningButton.onClick.AddListener(StopListeningButtonOnClickHandler);

            _speechRepeater.BeginTalkigEvent += BeginTalkingEventHandler;
            _speechRepeater.EndTalkigEvent += EndTalkingEventHandler;
            _speechRepeater.EndAudioPlayingEvent += EndAudioPlayingEventHandler;
        }

        private void Update()
        {

        }

        private void OnDestroy()
        {
            _speechRepeater.BeginTalkigEvent -= BeginTalkingEventHandler;
            _speechRepeater.EndTalkigEvent -= EndTalkingEventHandler;
            _speechRepeater.EndAudioPlayingEvent -= EndAudioPlayingEventHandler;
        }

        private void StartListeningButtonOnClickHandler()
        {
            _speechRepeater.StartListening();

            _statusImage.color = Color.green;
        }

        private void StopListeningButtonOnClickHandler()
        {
            _speechRepeater.StopListening();

            _statusImage.color = Color.white;
        }

        private void BeginTalkingEventHandler()
        {
            _statusImage.color = Color.red;
        }

        private void EndTalkingEventHandler(AudioClip clip)
        {
            _statusImage.color = Color.yellow;
        }

        private void EndAudioPlayingEventHandler()
        {
            _statusImage.color = Color.green;
        }
    }
}