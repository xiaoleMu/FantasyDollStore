using UnityEngine;
using UnityEditor;
using System.Collections;
using Tabtale.Editor;
using TabTale;

[CustomEditor(typeof(GsdkSettingsData))]
public class GsdkSettingsEditor : Editor
{

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Global settings - All Platforms", EditorStyles.boldLabel);

		EditorGUIUtility.labelWidth = 190f;

		bool isDebugEnabled = EditorGUILayout.Toggle("Is Debug Enabled",GsdkSettingsData.Instance.IsDebugEnabled);
		
		if(isDebugEnabled != GsdkSettingsData.Instance.IsDebugEnabled)
		{
			GsdkSettingsData.Instance.IsDebugEnabled = isDebugEnabled;
		}
			
		bool isCoreLoggerEnabled = EditorGUILayout.Toggle("Is CoreLogger Enabled",GsdkSettingsData.Instance.IsCoreLoggerEnabled);

		if(isCoreLoggerEnabled != GsdkSettingsData.Instance.IsCoreLoggerEnabled)
		{
			GsdkSettingsData.Instance.IsCoreLoggerEnabled = isCoreLoggerEnabled;
		}

		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("Editor settings", EditorStyles.boldLabel); 

		bool isConnectionEnabled = EditorGUILayout.Toggle("Is Connection Enabled",GsdkSettingsData.Instance.IsConnectionEnabled);

		if(isConnectionEnabled != GsdkSettingsData.Instance.IsConnectionEnabled)
		{
			GsdkSettingsData.Instance.IsConnectionEnabled = isConnectionEnabled;
		}

		bool socialScoreRefreshCoolDownEnabled = EditorGUILayout.Toggle("Social Score Cool Down Enabled",GsdkSettingsData.Instance.SocialScoreRefreshCoolDownEnabled);
		
		if(socialScoreRefreshCoolDownEnabled != GsdkSettingsData.Instance.SocialScoreRefreshCoolDownEnabled)
		{
			GsdkSettingsData.Instance.SocialScoreRefreshCoolDownEnabled = socialScoreRefreshCoolDownEnabled;
		}

		bool isRewardSystemEnabled = EditorGUILayout.Toggle("Reward System Enabled",GsdkSettingsData.Instance.IsRewardSystemEnabled);
		if(isRewardSystemEnabled != GsdkSettingsData.Instance.IsRewardSystemEnabled)
		{
			GsdkSettingsData.Instance.IsRewardSystemEnabled = isRewardSystemEnabled;
		}

		bool isEventSystemEnabled = EditorGUILayout.Toggle("Event System Enabled",GsdkSettingsData.Instance.isEventSystemEnabled);
		if(isEventSystemEnabled != GsdkSettingsData.Instance.isEventSystemEnabled)
		{
			GsdkSettingsData.Instance.isEventSystemEnabled = isEventSystemEnabled;
		}

		bool isShowingSessionStartInEditor = EditorGUILayout.Toggle("Is Showing Session Start In Editor",GsdkSettingsData.Instance.IsShowingSessionStartInEditor);
		if(isShowingSessionStartInEditor != GsdkSettingsData.Instance.IsShowingSessionStartInEditor)
		{
			GsdkSettingsData.Instance.IsShowingSessionStartInEditor = isShowingSessionStartInEditor;
		}

		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("Google Play settings", EditorStyles.boldLabel);

		EditorGUILayout.LabelField("Public key:");
		string googlePublicKey = EditorGUILayout.TextField(GsdkSettingsData.Instance.IapGooglePublicKey);
		if(googlePublicKey != GsdkSettingsData.Instance.IapGooglePublicKey)
		{
			GsdkSettingsData.Instance.IapGooglePublicKey = googlePublicKey;
		}
	}
}

