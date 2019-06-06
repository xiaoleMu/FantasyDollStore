using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TabTale
{
	public class VibrationToggleButtonView : MainView
    {
        [Inject]
        public SoundManager soundManager { get; set; }

		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }

        public Text _label;
        public Image _OnIcon;
        public Image _OffIcon;

        [PostConstruct]
        public void Init()
        {
            UpdateLabel();
        }

        public void OnClick()
        {
            soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);
			settingsStateModel.ToogleVibration();

            UpdateLabel();
        }

        public void UpdateLabel()
        {
			bool isVibrationOn = settingsStateModel.VibrationOn();

			_label.text = isVibrationOn ? "VIBRATE ON" : "VIBRATE OFF";

            if (_OnIcon != null)
				_OnIcon.enabled = isVibrationOn;

            if (_OffIcon != null)
				_OffIcon.enabled = !isVibrationOn;
        }
    }
}

