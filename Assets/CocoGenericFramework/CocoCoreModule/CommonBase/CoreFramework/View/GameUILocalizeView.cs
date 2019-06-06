﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CocoPlay
{
	[AddComponentMenu ("Localize/LocalizeView")]
	public class GameUILocalizeView : MonoBehaviour
	{

		/// <summary>
		/// Localization key.
		/// </summary>

		public string key;

		/// <summary>
		/// reset image size base on sprite
		/// </summary>
		public bool resetImageSize = true;

		/// <summary>
		/// Manually change the value of whatever the localization component is attached to.
		/// </summary>

		public string value {
			set {
				if (!string.IsNullOrEmpty (value)) {
//					Debug.LogWarning ("location : " + value);
					if (GetComponent<Text> () != null) {
						Text txt = GetComponent<Text> ();
						if (txt != null)
							txt.text = value;
					}
					if (GetComponent<Image> () != null) {
						Image img = GetComponent<Image> ();
						if (img != null) {
							var sprite = Resources.Load<Sprite> (value);
							if (sprite != null) {
								img.sprite = sprite;

								if (resetImageSize) {
									img.SetNativeSize ();
								}
							}
						}
						
					}
				}
			}
		}

		bool mStarted = false;

		/// <summary>
		/// Localize the widget on enable, but only if it has been started already.
		/// </summary>

		void OnEnable ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			if (mStarted)
				OnLocalize ();
		}

		/// <summary>
		/// Localize the widget on start.
		/// </summary>

		void Start ()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			#endif
			mStarted = true;
			OnLocalize ();
		}

		/// <summary>
		/// This function is called by the Localization manager via a broadcast SendMessage.
		/// </summary>

        protected virtual void OnLocalize ()
		{
			// If no localization key has been specified, use the label's text as the key
			if (string.IsNullOrEmpty (key)) {
				Text lbl = GetComponent<Text> ();
				if (lbl != null)
					key = lbl.text;
			}

			// If we still don't have a key, leave the value as blank
			if (!string.IsNullOrEmpty (key))
				value = Localization.CocoLocalization.Get (key);
		}
	}
}
