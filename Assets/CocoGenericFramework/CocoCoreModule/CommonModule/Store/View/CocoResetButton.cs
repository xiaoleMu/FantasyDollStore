using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CocoPlay
{
	public class CocoResetButton : CocoUINormalButton
	{
		private Text nameText;

		protected override void OnRegister ()
		{
			base.OnRegister ();

			nameText = GetComponentInChildren<Text> ();

			if (!CocoDebugSettingsData.Instance.IsGodModeEnabled) {
				gameObject.SetActive (false);
			}
		}

		protected override void OnClick ()
		{
			base.OnClick ();

			if (!CocoDebugSettingsData.Instance.IsGodModeEnabled) {
				return;
			}

			PlayerPrefs.DeleteAll ();
			PlayerPrefs.Save ();
#if UNITY_2017_1_OR_NEWER
			Caching.ClearCache ();
#else
			Caching.CleanCache ();
#endif
			string pPath = Application.persistentDataPath + "/DB/";
			if (System.IO.Directory.Exists (pPath))
				System.IO.Directory.Delete (pPath, true);

			if (nameText != null)
				nameText.text = "Restart";

			UnityEngine.EventSystems.EventSystem.current.enabled = false;
		}
	}
}