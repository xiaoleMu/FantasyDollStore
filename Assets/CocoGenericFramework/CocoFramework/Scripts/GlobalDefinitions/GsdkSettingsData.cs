using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TabTale
{
	[System.Serializable]
	public class GsdkSettingsData : ScriptableObject
	{
		public static string GAME_SPECIFIC_PATH 
		{ get { return "Assets/_Game/TabTale"; } }

		public static string GAME_SPECIFIC_RES 
		{ get { return GAME_SPECIFIC_PATH + "/Resources"; } }
		
		private static GsdkSettingsData _instance=null;
		public static GsdkSettingsData Instance 
		{
			get 
			{
				if (_instance == null)
				{
					_instance = Resources.Load ("GsdkSettingsData") as GsdkSettingsData;
				}
				
				if (_instance == null) 
				{
					_instance = ScriptableObject.CreateInstance<GsdkSettingsData>();
				}
				
				return _instance;
			}
		}

		public override string ToString() 
		{
			return "isConnectionEnabled:" + IsConnectionEnabled;
		}

		[SerializeField]
		private bool _isConnectionEnabled = true;
		public bool IsConnectionEnabled
		{
			get { return _isConnectionEnabled; }
			set { _isConnectionEnabled = value; Save();}
		}

		[SerializeField]
		private bool _socialScoreRefreshCoolDownEnabled = true;
		public bool SocialScoreRefreshCoolDownEnabled
		{
			get { return _socialScoreRefreshCoolDownEnabled; }
			set { _socialScoreRefreshCoolDownEnabled = value; Save();}
		}

		[SerializeField]
		private bool _isDebugEnabled = true;
		public bool IsDebugEnabled
		{
			get { return _isDebugEnabled; }
			set { _isDebugEnabled = value; Save();}
		}

		[SerializeField]
		private bool _isRewardSystemEnabled = true;
		public bool IsRewardSystemEnabled
		{
			get { return _isRewardSystemEnabled; }
			set { _isRewardSystemEnabled = value; Save();}
		}

		[SerializeField]
		private bool _isEventSystemEnabled = true;
		public bool isEventSystemEnabled
		{
			get { return _isEventSystemEnabled; }
			set { _isEventSystemEnabled = value; Save(); }
		}

		[SerializeField]
		private bool _isCoreLoggerEnabled = false;
		public bool IsCoreLoggerEnabled
		{
			get { return _isCoreLoggerEnabled; }
			set 
			{ 
				_isCoreLoggerEnabled = value; 
				CoreLogger.Enabled = value; 
				Save(); 
			}
		}

		[SerializeField]
		private bool _isShowingSessionStartInEditor = false;
		public bool IsShowingSessionStartInEditor
		{
			get { return _isShowingSessionStartInEditor; }
			set { _isShowingSessionStartInEditor = value; Save(); }
		}

		[SerializeField]
		private string _iapGooglePublicKey;
		public string IapGooglePublicKey
		{
			get { return _iapGooglePublicKey; }
			set { _iapGooglePublicKey = value; Save(); }
		}


		private void Save()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			AssetDatabase.Refresh();
			Debug.Log("Saved GSDK Settings...");
#endif
		}

	}
}
