using UnityEngine;
using System.Collections;
using CocoPlay;
using System.Collections.Generic;
using TabTale;
using UnityEngine.UI;
using CocoPlay.Native;
using FrostweepGames.Plugins.SpeechRepeater;

namespace Game
{
	public class GameRecordSceneManage : GameGenericSceneBase
	{
		[Inject]
		public GameRecordVolueData recordData {get; set;}
	
		private AdvancedSpeechRepeater _speechRepeater;

		#region Init Clean

		protected override void initCharacter ()
		{
			base.initCharacter ();

			m_CurRole.gameObject.SetActive (true);

			m_CurRole.Animation.SetAnimationData (new GameRecordAnimationData ());
			m_CurRole.Animation.SetAnimatorController ("recordvolue_animator");
			m_CurRole.Animation.SetAutoSwithEnable (true);
			m_CurRole.Animation.Play (recordData.CA_RecordVolue_Standby);

			m_CurRole.transform.localPosition = new Vector3 (-0.207f, 0.26f, -0.72f);
			m_CurRole.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			m_CurRole.transform.localScale = Vector3.one * 300f;

		}

		protected override void Start ()
		{
			_speechRepeater = AdvancedSpeechRepeater.Instance;
			base.Start ();
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			if (_speechRepeater == null){
				_speechRepeater = AdvancedSpeechRepeater.Instance;
			}

			_speechRepeater.BeginTalkigEvent += BeginTalkingEventHandler;
			_speechRepeater.EndTalkigEvent += EndTalkingEventHandler;
			_speechRepeater.EndAudioPlayingEvent += EndAudioPlayingEventHandler;
		}

		protected override void RemoveListeners ()
		{
			_speechRepeater.BeginTalkigEvent -= BeginTalkingEventHandler;
			_speechRepeater.EndTalkigEvent -= EndTalkingEventHandler;
			_speechRepeater.EndAudioPlayingEvent -= EndAudioPlayingEventHandler;

			base.RemoveListeners ();
		}

		#endregion

		protected override void OnButtonClickWithButtonName (CocoUINormalButton button, string pButtonName)
		{
			base.OnButtonClickWithButtonName (button, pButtonName);
			switch (pButtonName){
			case "Speack":
				CocoAudio.StopBgMusic();
				StartListeningButtonOnClickHandler ();
				break;

			case "Stop":
				StopListeningButtonOnClickHandler ();
				CocoAudio.PlayBgMusic(gameGlobalData.GetSceneBg(gameGlobalData.CurSceneID));
				break;

//			case "speed":
//				_speechRepeater.audioConfig.volume += 1f;
//				break;
//
//			case "Tone":
//				_speechRepeater.audioConfig.pitch += 1f;
//				break;
			}
		}

		[SerializeField]
		Slider m_ToneSlider;
		public void OnToneChange (float value){
			_speechRepeater.audioConfig.pitch = m_ToneSlider.value * 2f + 1f;
			Debug.LogError ("aaaaaaa is : " + _speechRepeater.audioConfig.pitch);
		}

//		public void OnSpeedChange (float value){
//			_speechRepeater.audioConfig.volume = value * 2f + 1f;
//		}

		private void StartListeningButtonOnClickHandler()
		{
			_speechRepeater.StartListening();

			Debug.LogError ("StartListeningButtonOnClickHandler");
//			_statusImage.color = Color.green;
		}

		private void StopListeningButtonOnClickHandler()
		{
			_speechRepeater.StopListening();

			Debug.LogError ("StopListeningButtonOnClickHandler");
//			_statusImage.color = Color.white;
		}

		private void BeginTalkingEventHandler()
		{
			Debug.LogError ("BeginTalkingEventHandler");
//			_statusImage.color = Color.red;
		}

		private void EndTalkingEventHandler(AudioClip clip)
		{
//			CocoAudio.PlayOnTarget (gameObject, clip);
			Debug.LogError ("EndTalkingEventHandler");
//			_statusImage.color = Color.yellow;
//			StopListeningButtonOnClickHandler ();
//			CocoAudio.PlayBgMusic(gameGlobalData.GetSceneBg(gameGlobalData.CurSceneID));
		}

		private void EndAudioPlayingEventHandler()
		{
			Debug.LogError ("EndAudioPlayingEventHandler");
//			_statusImage.color = Color.green;
		}

	}
}
