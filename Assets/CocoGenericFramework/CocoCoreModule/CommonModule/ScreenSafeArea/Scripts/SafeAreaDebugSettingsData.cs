using UnityEngine;


namespace CocoPlay
{
	public partial class CocoDebugSettingsData
	{
#if UNITY_EDITOR


		#region Safe Area

		private const float SAFE_AREA_ASPECT = 19f / 9f;
		private static readonly Rect _iPhoneXSafeAreaRatioLandscape = new Rect (0.04802955665f, 0.06133333333f, 0.9039408867f, 0.9386666667f);
		private static readonly Rect _iPhoneXSafeAreaRatioPortrait = new Rect (0, 0.04802955665f, 1, 0.9039408867f);


		public enum EditorSafeAreaStyle
		{
			Off,
			Auto,
			IphoneX,
			Custom
		}


		[Header ("Safe Area")]
		[SerializeField]
		private EditorSafeAreaStyle m_SafeAreaStyle = EditorSafeAreaStyle.Auto;

		public EditorSafeAreaStyle SafeAreaStyle {
			get { return m_SafeAreaStyle; }
			set {
				m_SafeAreaStyle = value;
				Save ();
			}
		}

		[SerializeField]
		private Rect m_CustomSafeAreaRatio = new Rect (0.05f, 0.05f, 0.9f, 0.9f);

		public Rect SafeAreaRatio {
			get {
				switch (SafeAreaStyle) {
				case EditorSafeAreaStyle.Auto:
					return AutoSafeAreaRatio;

				case EditorSafeAreaStyle.IphoneX:
					return iPhoneXSafeAreaRatio;

				case EditorSafeAreaStyle.Custom:
					return m_CustomSafeAreaRatio;
				}

				return FullScreenSafeAreaRatio;
			}
			set {
				if (SafeAreaStyle != EditorSafeAreaStyle.Custom) {
					return;
				}

				m_CustomSafeAreaRatio = value;
				Save ();
			}
		}

		private static bool IsLandscape {
			get { return Screen.width > Screen.height; }
		}

		private static Rect AutoSafeAreaRatio {
			get {
				if (IsLandscape) {
					if ((float)Screen.width / Screen.height > SAFE_AREA_ASPECT) {
						return _iPhoneXSafeAreaRatioLandscape;
					}
				} else {
					if ((float)Screen.height / Screen.width > SAFE_AREA_ASPECT) {
						return _iPhoneXSafeAreaRatioPortrait;
					}
				}

				return FullScreenSafeAreaRatio;
			}
		}

		private static Rect iPhoneXSafeAreaRatio {
			get { return IsLandscape ? _iPhoneXSafeAreaRatioLandscape : _iPhoneXSafeAreaRatioPortrait; }
		}

		private static Rect FullScreenSafeAreaRatio {
			get { return new Rect (0, 0, 1, 1); }
		}

		#endregion


#endif
	}
}