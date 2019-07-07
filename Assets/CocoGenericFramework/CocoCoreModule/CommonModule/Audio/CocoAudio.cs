using UnityEngine;
using System.Collections.Generic;
using TabTale;
using TabTale.Plugins.PSDK;

#if !COCO_FAKE
using CocoAudioID = Game.CocoAudioID;

#else
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
#endif


namespace CocoPlay
{
	public class CocoAudio
	{
		#region Instance

		static CocoAudio m_Instance = null;

		static public CocoAudio Instance {
			get {
				if (m_Instance == null) {
					m_Instance = new CocoAudio ();
				}
				return m_Instance;
			}
		}

		private CocoAudio ()
		{
		    m_LayerSwitchDict = new Dictionary<SoundLayer, bool>()
		    {
		        {SoundLayer.Main, true},
		        {SoundLayer.Music, true}
		    };
		}

		#endregion


		#region Global

		SoundManager m_Manager = null;

		public SoundManager Manager {
			get {
				if (m_Manager == null) {
					m_Manager = CocoRoot.GetInstance<SoundManager> ();
				}
				return m_Manager;
			}
		}

		ICocoAudioData m_Data = null;

		public ICocoAudioData Data {
			get {
				if (m_Data == null) {
					m_Data = CocoRoot.GetInstance<ICocoAudioData> ();
				}
				return m_Data;
			}
		}



	    #region status

	    bool m_IsOn = false;
	    private Dictionary<SoundLayer, bool> m_LayerSwitchDict;
	    
	    public static readonly string MUSIC_PAUSE_CALLER_IN_GAMEPLAY = "inGameplay";

	    public static bool IsOn {
	        get {
	            return Instance.m_IsOn;
	        }
	        set {
	            Instance.SetSoundSwitch (value);
	        }
	    }

	    void SetSoundSwitch (bool isOn)
	    {
	        if (m_IsOn == isOn) {
	            return;
	        }

	        m_IsOn = isOn;

	        PsdkEventSystem.Instance.PauseGameMusicEventNotification (LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC, !m_IsOn, MUSIC_PAUSE_CALLER_IN_GAMEPLAY);
	        ResumeLayerSound();
	    }

	    public void SetLayerEnable(SoundLayer layer, bool isOn)
	    {
	        if(!m_LayerSwitchDict.ContainsKey(layer))
	            return;
	        m_LayerSwitchDict[layer] = isOn;
	        
	        if(isOn && m_IsOn)
	            Manager.UnMuteSoundLayer(layer);
	        else
	            Manager.MuteSoundLayer(layer);
	    }
	    
	    public bool GetLayerEnable(SoundLayer layer)
	    {
	        if(!m_LayerSwitchDict.ContainsKey(layer))
	            return true;
	        return m_LayerSwitchDict[layer];
	    }

	    void ResumeLayerSound()
	    {
	        foreach (var layer in m_LayerSwitchDict.Keys)
	        {
	            bool layerEnable = m_LayerSwitchDict[layer];
	            
	            if (layerEnable && m_IsOn)
	            {
	                if(Manager.IsLayerMuted(layer))
	                    Manager.UnMuteSoundLayer(layer);

	            }
	            else
	            {
	                if(!Manager.IsLayerMuted(layer))
	                    Manager.MuteSoundLayer(layer);
	            }
	        }
	    }
	    
	    #endregion status

		public static void StopAll ()
		{
			StopBgMusic ();
			StopSound ();
			StopOnAllTargets ();
		}

		public static void PauseAll (bool pause)
		{
			PauseBgMusic (pause);
			PauseSound (pause);
			PauseOnAllTargets (pause);
		}

		public static void MuteAll (bool mute)
		{
			if (mute) {
				Instance.Manager.MuteAllLayers ();
			} else {
				Instance.Manager.UnMuteAllLayers ();
			}
			MuteOnAllTargets (mute);
		}

		#endregion


		#region Bg Music

		public static void PlayBgMusic (string musicName)
		{
			StopBgMusic ();
			List<string> soundList = new List<string> (1) { musicName };
			Instance.Manager.PlaySound (soundList, SoundLayer.Music, true);
		}

		public static void PlayBgMusic (CocoAudioID audioId)
		{
			string audioName = Instance.Data.GetAudioName (audioId);
			PlayBgMusic (audioName);
		}

		public static void StopBgMusic ()
		{
			Instance.Manager.StopSoundLayer (SoundLayer.Music);
		}

		public static void PauseBgMusic (bool pause)
		{
			if (pause) {
				Instance.Manager.PauseSoundLayer (SoundLayer.Music);
			} else {
				Instance.Manager.UnPauseSoundLayer (SoundLayer.Music);
			}
		}

		#endregion


		#region Sound

		public static void PlaySound (string soundName, bool loop = false)
		{
			List<string> soundList = new List<string> (1) { soundName };
			Instance.Manager.PlaySound (soundList, SoundLayer.Main, loop);
		}

		public static void PlaySound (CocoAudioID audioId, bool loop = false)
		{
			string audioName = Instance.Data.GetAudioName (audioId);
			PlaySound (audioName, loop);
		}

		public static void StopSound ()
		{
			Instance.Manager.StopSoundLayer (SoundLayer.Main);
		}

		public static void StopSound (string soundName)
		{
			List<string> soundList = new List<string> (1) { soundName };
			Instance.Manager.StopSound (soundList, SoundLayer.Main);
		}

		public static void StopSound (CocoAudioID audioId)
		{
			string audioName = Instance.Data.GetAudioName (audioId);
			StopSound (audioName);
		}

		public static void PauseSound (bool pause)
		{
			if (pause) {
				Instance.Manager.PauseSoundLayer (SoundLayer.Main);
			} else {
				Instance.Manager.UnPauseSoundLayer (SoundLayer.Main);
			}
		}

		#endregion


		#region Sound On Target

		HashSet<GameObject> m_TargetSet = new HashSet<GameObject> ();

		public static void PlayOnTarget (GameObject targetGo, CocoAudioID audioID, bool loop = false)
		{
			PlayOnTarget (targetGo, Instance.Data.GetAudioName (audioID), loop);
		}

		public static void PlayOnTarget (GameObject targetGo, string soundName, bool loop = false)
		{
			if (Instance.Manager.IsLayerMuted (SoundLayer.Main)) {
				return;
			}
			if (targetGo == null || string.IsNullOrEmpty (soundName)) {
				return;
			}

			AudioClip clip = Resources.Load<AudioClip> ("Sounds/" + soundName);
			if (clip == null) {
				return;
			}

			AudioSource audio = CocoLoad.GetOrAddComponent<AudioSource> (targetGo);
			audio.playOnAwake = false;
			AudioClip oldClip = audio.clip;

			if (oldClip != clip) {
				if (oldClip != null) {
					audio.clip = null;
					Resources.UnloadAsset (oldClip);
				}
				audio.clip = clip;
			}
			audio.loop = loop;
			audio.Play ();

			if (!Instance.m_TargetSet.Contains (targetGo)) {
				Instance.m_TargetSet.Add (targetGo);
			}
		}

		public static void PlayOnTarget (GameObject targetGo, AudioClip clip, bool loop = false)
		{
			if (Instance.Manager.IsLayerMuted (SoundLayer.Main)) {
				return;
			}
			if (targetGo == null || clip == null) {
				return;
			}

			AudioSource audio = CocoLoad.GetOrAddComponent<AudioSource> (targetGo);
			audio.playOnAwake = false;
			AudioClip oldClip = audio.clip;

			if (oldClip != clip) {
				if (oldClip != null) {
					audio.clip = null;
					Resources.UnloadAsset (oldClip);
				}
				audio.clip = clip;
			}
			audio.loop = loop;
			audio.Play ();

			if (!Instance.m_TargetSet.Contains (targetGo)) {
				Instance.m_TargetSet.Add (targetGo);
			}
		}

		public static void StopOnTarget (GameObject targetGo)
		{
			if (Instance.Manager.IsLayerMuted (SoundLayer.Main)) {
				return;
			}
			if (targetGo == null) {
				return;
			}

			AudioSource audio = targetGo.GetComponent<AudioSource> ();
			if (audio == null) {
				return;
			}

			AudioClip clip = audio.clip;
			if (clip != null) {
				audio.Stop ();
				audio.clip = null;
			}

			Instance.m_TargetSet.Remove (targetGo);
		}

		public static void PauseOnTarget (GameObject targetGo, bool pause)
		{
//			if (Instance.Manager.IsLayerMuted (SoundLayer.Main)) {
//				return;
//			}
			if (targetGo == null) {
				return;
			}

			AudioSource audio = targetGo.GetComponent<AudioSource> ();
			if (audio == null) {
				return;
			}

			if (pause) {
				audio.Pause ();
			} else {
				audio.UnPause ();
			}
		}

		static void MuteOnTarget (GameObject targetGo, bool mute)
		{
//			if (!IsOn) {
//				return;
//			}
			if (targetGo == null) {
				return;
			}

			AudioSource audio = targetGo.GetComponent<AudioSource> ();
			if (audio == null) {
				return;
			}

			audio.mute = mute;
		}

		public static void StopOnAllTargets ()
		{
			HashSet<GameObject> tempTargetSet = new HashSet<GameObject> (Instance.m_TargetSet);
			foreach (GameObject targetGo in tempTargetSet) {
				StopOnTarget (targetGo);
			}
		}

		public static void PauseOnAllTargets (bool pause)
		{
			foreach (GameObject targetGo in Instance.m_TargetSet) {
				PauseOnTarget (targetGo, pause);
			}
		}

		public static void MuteOnAllTargets (bool mute)
		{
			foreach (GameObject targetGo in Instance.m_TargetSet) {
				MuteOnTarget (targetGo, mute);
			}
		}

		public static void PlayTargetSound (GameObject pGameObject, bool pIsLoop = false)
		{


			if (Instance.Manager.IsLayerMuted (SoundLayer.Main)) {
				return;
			}

			AudioSource pAudio = pGameObject.GetComponent <AudioSource> ();

			if (pAudio == null)
				return;

			pAudio.loop = pIsLoop;
			pAudio.Play ();
			if (!Instance.m_TargetSet.Contains (pGameObject)) {
				Instance.m_TargetSet.Add (pGameObject);
			}
		}


		public static void StopTargetSound (GameObject targetGo)
		{
			if (Instance.Manager.IsLayerMuted (SoundLayer.Main)) {
				return;
			}
			AudioSource pAudio = targetGo.GetComponent <AudioSource> ();
			if (pAudio != null) {
                pAudio.Stop();
				Instance.m_TargetSet.Remove (targetGo);
			}
		}

		#endregion
	}
}
