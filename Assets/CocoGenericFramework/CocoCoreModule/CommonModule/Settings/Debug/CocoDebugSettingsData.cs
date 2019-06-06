using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CocoLanguage = Game.CocoLanguage;


namespace CocoPlay
{
	[System.Serializable]
	public partial class CocoDebugSettingsData : ScriptableObject
	{
		public const string GLOBAL_SCRIPTING_SYMBOL = "COCO_DEBUG";

		private static CocoDebugSettingsData m_Instance;

		public static CocoDebugSettingsData Instance {
			get {
				if (m_Instance == null) {
					m_Instance = Resources.Load<CocoDebugSettingsData> ("CocoDebugSettingsData");
				}
				if (m_Instance == null) {
					// Didn't found asset, create one
					m_Instance = CreateInstance<CocoDebugSettingsData> ();
				}
				return m_Instance;
			}
		}

		private void Save ()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty (this);
			AssetDatabase.Refresh ();
			Debug.Log ("Saved Coco Debug Settings ...");
#endif
		}


		#region Options

		[Header ("Global")]
		[SerializeField]
		private bool m_IsGlobalEnabled = true;

		public bool IsGlobalEnabled {
			get {
#if COCO_DEBUG || UNITY_EDITOR
				return m_IsGlobalEnabled;
#else
				return false;
#endif
			}
			set {
				m_IsGlobalEnabled = value;
				Save ();
			}
		}

		[Header ("Common")]
		[SerializeField]
		private bool m_IsGodModeEnabled;

		public bool IsGodModeEnabled {
			get { return IsGlobalEnabled && m_IsGodModeEnabled; }
			set {
				m_IsGodModeEnabled = value;
				Save ();
			}
		}

		[SerializeField]
		private bool m_IsSkipRVPopupEnabled;

		public bool IsSkipRVPopupEnabled {
			get { return IsGlobalEnabled && m_IsSkipRVPopupEnabled; }
			set {
				m_IsSkipRVPopupEnabled = value;
				Save ();
			}
		}

		[SerializeField]
		private CocoOptionalVector2Property m_IsFPSHudEnabled = new CocoOptionalVector2Property ();

		public bool IsFPSHudEnabled {
			get { return IsGlobalEnabled && m_IsFPSHudEnabled.Used; }
			set {
				m_IsFPSHudEnabled.Used = value;
				Save ();
			}
		}

		public Vector2 FPSHudStartPos {
			get { return m_IsFPSHudEnabled.Value; }
			set { m_IsFPSHudEnabled.Value = value; }
		}

		[SerializeField]
		private bool m_IsNotificationTestModeEnabled;

		public bool IsNotificationTestModeEnabled {
			get { return IsGlobalEnabled && m_IsNotificationTestModeEnabled; }
			set { m_IsNotificationTestModeEnabled = value; }
		}

		[SerializeField]
		private bool m_IsSkipInterstitialEnabled;

		public bool IsSkipInterstitialEnabled {
			get { return IsGlobalEnabled && m_IsSkipInterstitialEnabled; }
			set {
				m_IsSkipInterstitialEnabled = value;
				Save ();
			}
		}

		//商店逻辑购买模式
		[SerializeField]
		private bool m_IsDebugStoreMode;

		public bool IsDebugStoreMode {
			get { return IsGlobalEnabled && m_IsDebugStoreMode; }
			set {
				m_IsDebugStoreMode = value;
				Save ();
			}
		}


		[SerializeField]
		private bool m_IsFlurryLogEnabled;

		public bool IsFlurryLogEnabled {
			get { return IsGlobalEnabled && m_IsFlurryLogEnabled; }
			set {
				m_IsFlurryLogEnabled = value;
				Save ();
			}
		}

		[SerializeField]
		private bool m_IsDebugLogEnabled;

		public bool IsDebugLogEnabled {
			get { return IsGlobalEnabled && m_IsDebugLogEnabled; }
			set {
				m_IsDebugLogEnabled = value;
				Save ();
			}
		}

		#endregion


#if UNITY_EDITOR


		#region Editor Options

		[Header ("Editor Only")]
		[SerializeField]
		private CocoOptionalLanguage m_IsEditorLanguageEnabled = new CocoOptionalLanguage ();

		public bool IsEditorLanguageEnabled {
			get { return m_IsEditorLanguageEnabled.Used; }
			set {
				m_IsEditorLanguageEnabled.Used = value;
				Save ();
			}
		}

		public CocoLanguage EditorLanguage {
			get { return m_IsEditorLanguageEnabled.Value; }
			set {
				m_IsEditorLanguageEnabled.Value = value;
				Save ();
			}
		}

		[SerializeField]
		private string[] m_NoMipmapExcludedAssetPaths = { };

		public string[] NoMipmapExcludedAssetPaths {
			get { return m_NoMipmapExcludedAssetPaths; }
			set {
				m_NoMipmapExcludedAssetPaths = value;
				Save ();
			}
		}

		#endregion


#endif
	}
}