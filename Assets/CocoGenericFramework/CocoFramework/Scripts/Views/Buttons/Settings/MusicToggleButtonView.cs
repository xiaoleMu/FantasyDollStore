using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TabTale
{
	public class MusicToggleButtonView: MainView
	{
		[Inject]
		public SoundManager soundManager{ get; set; }

		public Text _label;
		public Image _OnIcon;
		public Image _OffIcon;

		[PostConstruct]
		public void Init ()
		{
			UpdateLabel ();
		}

		
		public void OnClick ()
		{
			if (soundManager.IsLayerMuted (SoundLayer.Music)) {
				soundManager.UnMuteSoundLayer (SoundLayer.Music);
				soundManager.PlaySound (SoundMapping.Map.GeneralButtonClick, SoundLayer.Music);
			} else
				soundManager.MuteSoundLayer (SoundLayer.Music);
			
			UpdateLabel ();
		}

		public void UpdateLabel ()
		{
			bool isOff = soundManager.IsLayerMuted (SoundLayer.Music);
			if (_label != null)
				_label.text = isOff ? "MUSIC OFF" : "MUSIC ON";
			
			if (_OnIcon != null)
				_OnIcon.enabled = !isOff;
			
			if (_OffIcon != null)
				_OffIcon.enabled = isOff;
		}
	}
}