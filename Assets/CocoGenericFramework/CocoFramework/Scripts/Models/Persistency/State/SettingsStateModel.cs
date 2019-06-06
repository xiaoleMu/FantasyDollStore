using UnityEngine;
using System.Collections;

namespace TabTale
{
	public partial class SettingsStateModel : StateModel<SettingsStateData>
	{
		public bool VibrationOn ()
		{
			return _data.vibration;
		}

		public bool ToogleVibration ()
		{
			_data.vibration = !_data.vibration;
			return Save ();
		}

		public bool SoundOn ()
		{
			return _data.sound;
		}

		public bool ToggleSound ()
		{
			_data.sound = !_data.sound;
			return Save ();
		}

		public bool MusicOn ()
		{
			return _data.music;
		}

		public bool ToggleMusic ()
		{
			_data.music = !_data.music;
			return Save ();
		}

		public bool ShowingAds()
		{
			return !_data.noAds;
		}

		public bool SetNoAds(bool flag = true)
		{
			_data.noAds = flag;

			logger.Log("SettingsStataModel.SetNoAds : settings noAds to:" + flag);

			return Save ();
		}

		public string Language ()
		{
			return _data.language;
		}

		public bool SetLanguage (string newLang)
		{
			_data.language = newLang;
			return Save ();
		}

		#region Promotion
		private const string EligibleProperty = "eligibleForPromotion";

		public bool EligibleForPromotion()
		{
			return _data.properties.GetBoolProperty(EligibleProperty);
		}
		public bool SetPromotionEligibility (bool isEligible)
		{
			_data.properties.SetProperty(EligibleProperty, isEligible);
			return Save ();
		}

		#endregion
	}
}
