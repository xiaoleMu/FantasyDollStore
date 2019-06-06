using System;
using UnityEngine;
using System.Collections.Generic;



namespace TabTale
{
    public class SoundManager
    {
		[Inject]
		public SoundManagerReadySignal soundManagerReadySignal { get; set;}

        private static SoundManagerPlayer _player;
        public static Dictionary<SoundLayer, bool> channelMute;
        public static Dictionary<SoundLayer, bool> channelPaused;

        [PostConstruct]
        public void Init()
        {
            channelMute = new Dictionary<SoundLayer, bool>();
            channelPaused = new Dictionary<SoundLayer, bool>();
            foreach (SoundLayer layer in Enum.GetValues(typeof(SoundLayer)))
            {
                channelMute.Add(layer, PlayerPrefs.GetInt("soundMute/" + layer, 0) == 1);
				channelPaused.Add (layer, false);
            }

            GameObject soundMng = GameObject.Find("SoundManagerPlayer");
            if (soundMng == null)
            {
                soundMng = new GameObject("SoundManagerPlayer");
                _player = soundMng.AddComponent<SoundManagerPlayer>();
                GameObject.DontDestroyOnLoad(soundMng);
            }
            else
                _player = soundMng.GetComponent<SoundManagerPlayer>();


			soundManagerReadySignal.Dispatch ();

        }

		public bool IsClipPlaying (string clipName, SoundLayer layer)
		{
			return _player.IsPlaying (clipName, layer);
		}
        
        public void StopSoundLayer(SoundLayer layer)
        {
            _player.StopAllSoundsOnLayer(layer);
        }

		public void StartSoundLayer(SoundLayer layer)
		{
			_player.StartAllSoundsOnLayer(layer);
		}

        public void MuteAllLayers()
        {
			foreach (SoundLayer layer in Enum.GetValues(typeof(SoundLayer)))
                MuteSoundLayer(layer);
        }

		public void UnMuteAllLayers(bool alsoPlayTheSoundsOnLayer = true)
        {
            foreach (SoundLayer layer in Enum.GetValues(typeof(SoundLayer)))
				UnMuteSoundLayer(layer);
        }

        public void MuteSoundLayer(SoundLayer layer)
        {
			//StopSoundLayer(layer);
			_player.MuteLayer (layer);
            channelMute[layer] = true;
            PlayerPrefs.SetInt("soundMute/" + layer, 1);
            PlayerPrefs.Save();
        }

		public void UnMuteSoundLayer(SoundLayer layer)
        {
//			if (alsoPlayTheSoundsOnLayer)
//				StartSoundLayer(layer);
			_player.UnmuteLayer (layer);
            channelMute[layer] = false;
            PlayerPrefs.SetInt("soundMute/" + layer, 0);
            PlayerPrefs.Save();
        }

        public void PauseAllLayers()
        {
            foreach (SoundLayer layer in Enum.GetValues(typeof(SoundLayer)))
                PauseSoundLayer(layer);
        }

        public void UnPauseAllLayers()
        {
            foreach (SoundLayer layer in Enum.GetValues(typeof(SoundLayer)))
                UnPauseSoundLayer(layer);
        }


        public void PauseSoundLayer(SoundLayer layer)
        {
            channelPaused[layer] = true;
            _player.PauseSoundsOnLayer(layer);
        }

        public void UnPauseSoundLayer(SoundLayer layer)
        {
            channelPaused[layer] = false;
            _player.UnPauseSoundsOnLayer(layer);
        }



        public bool IsLayerMuted(SoundLayer layer)
        {
            return channelMute[layer];
        }


        public void PlaySound(List<string> soundList, SoundLayer layer, bool isLooping = false)
        {
//            if (channelMute[layer] || channelPaused[layer])
//                return;

            string clipPath = SoundMapping.GetSoundClipPathForEventType(soundList);
			SoundManagerPlayer.SoundManagerClip smc = _player.PlaySound(clipPath, layer, isLooping);

			if (channelMute[layer])
				smc.MuteClip();
			
			if (channelPaused[layer])
				smc.PauseClip();
        }

		public void StopSound(List<string> soundList, SoundLayer layer)
		{
//			if (channelMute[layer] || channelPaused[layer])
//				return;

			// try to stop all sounds in list because can't know which one be played
			for (int i = 0; i < soundList.Count; i++) {
				string clipPath = SoundMapping.GetSoundClipPathForEventType(soundList [i]);
				_player.StopSound (clipPath,layer);
			}
		}

        public void PlayTimedSound(List<string> type, SoundLayer layer, float timeToPlay)
        {
//            if (channelMute[layer] || channelPaused[layer])
//                return;

            string clipPath = SoundMapping.GetSoundClipPathForEventType(type);
			SoundManagerPlayer.SoundManagerClip smc = _player.PlayTimedSound(clipPath, layer, timeToPlay);

			if (channelMute[layer])
				smc.MuteClip();

			if (channelPaused[layer])
				smc.PauseClip();
        }
    }
}
