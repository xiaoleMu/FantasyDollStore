using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace Game
{
	public class GameRecordData : IStateData
	{
		LitJson.JsonData pdata;
		public string appVersion = "0.0.0";

		public int totalTime = 0;
		public int totalSessionNum = 0;
		public int totalPurchaseNum = 0;

		public bool firstRateUsByDance = false;

		#region scene data

		public string finishedPhotoGestureTips = "";
		public bool firstMainEndRateUsUsed = false;
		public System.DateTime dailyTime = System.DateTime.Now;

		public bool firstMsgShowed = false;
		public bool firstMsgTwoNextShowed = false;
		public bool dailyInvitationShoed = true;

		public string firstEnterScenes = "";

		#endregion

		#region CharacterRecord

		public string characterRecordData = "";

		#endregion

		#region store

		public int isGodOrReset = 0;

		#endregion

		public string SaveStr = "";

		#region behavior
		public int durationTime = 0;
		public int scene_entry_count = 0;
		public int homeGiftOpened = 0;
		public int homeGiftUsed = 0;
		public int homeGiftRemoved = 0;
		#endregion

		//用于成就系统
		#region Achievements

		public List<string> dressupAchievementsList = new List<string>();
		public List<string> makeupAchievementsList = new List<string>();
		public List<int> danceMovesAchievementsList = new List<int>();
		public List<string> nailAchievementsList = new List<string>();
		public List<string> leagueAchievementsList = new List<string>();

		#endregion

		// permission request flag (android)
		public int permissionRequestFlag = 0;

		/// <summary>
		/// 已经完成的Flow
		/// </summary>
		public Dictionary<string,int> sceneEntryTimes = new Dictionary<string, int>();

		public string GetStateName ()
		{
			return "GameRecordState";
		}

		public override string ToString ()
		{
			return ("GameRecordState: id=" + appVersion).ToString ();
		}

		public string ToLogString ()
		{
			return ("GameRecordState: id=" + appVersion).ToString ();
		}

		public IStateData Clone ()
		{
			GameRecordData data = new GameRecordData ();
			//			System.Reflection.FieldInfo[] fields = typeof(GameRecordData).GetFields();
			//			foreach(var info in fields)
			//			{
			//				object value = info.GetValue(this);
			//				info.SetValue(data, value);
			//			}    

			data.appVersion = appVersion;
			data.totalTime = totalTime;
			data.totalSessionNum = totalSessionNum;
			data.firstRateUsByDance = firstRateUsByDance;

			#region scene data
			data.finishedPhotoGestureTips = finishedPhotoGestureTips;
			data.firstMainEndRateUsUsed = firstMainEndRateUsUsed;
			data.dailyTime = dailyTime;
			data.firstMsgShowed = firstMsgShowed;
			data.firstMsgTwoNextShowed = firstMsgTwoNextShowed;
			data.dailyInvitationShoed = dailyInvitationShoed;
			data.firstEnterScenes = firstEnterScenes;

			#endregion

			#region CharacterRecord
			data.characterRecordData = characterRecordData;
			#endregion

			#region store
			data.isGodOrReset = isGodOrReset;
			#endregion

			data.SaveStr = SaveStr;
			data.durationTime = durationTime;
			data.scene_entry_count = scene_entry_count;
			data.homeGiftUsed = homeGiftOpened;
			data.homeGiftUsed = homeGiftUsed;
			data.homeGiftRemoved = homeGiftRemoved;

			data.permissionRequestFlag = permissionRequestFlag;

			return data;
		}

		#region for FTUE flow

		public bool isIntroFinished = false;
		public bool isFTUEFinished = false;
		public int gameFlowStep = 0;
		public bool isRegisterNotifications = false;
		#endregion

		public bool isGameShowedRateUs = false;


		#region Change Lock

		public bool isRVLock = false;

		#endregion


		#region Doll

		public List<List<string>> recordDolls = new List<List<string>> ();

		#endregion
	}
}
