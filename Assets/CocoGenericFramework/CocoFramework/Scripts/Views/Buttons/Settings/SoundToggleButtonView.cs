using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace TabTale
{
	public class SoundToggleButtonView: MainView
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
			if (soundManager.IsLayerMuted (SoundLayer.Main)) {
				soundManager.UnMuteSoundLayer (SoundLayer.Main);
				soundManager.PlaySound (SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);
			} else
				soundManager.MuteSoundLayer (SoundLayer.Main);

			UpdateLabel ();
		}

		public void UpdateLabel ()
		{
			bool isOff = soundManager.IsLayerMuted (SoundLayer.Main);
			if (_label != null)
				_label.text = isOff ? "SOUND OFF" : "SOUND ON";

			if (_OnIcon != null)
				_OnIcon.enabled = !isOff;

			if (_OffIcon != null)
				_OffIcon.enabled = isOff;
		}
	}
}

