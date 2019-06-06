using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace CocoPlay
{
	public class CocoGlobalRecordModel : StateModel<CocoGlobalRecordData>
	{

		#region Game Common

		public int FunnelSceneCount {
			get {
				return _data.mFunnelSceneCount;
			}
			set {
				_data.mFunnelSceneCount = value;
				Save ();
			}
		}

		public int GameDuration {
			get {
				return _data.mGameDuration;
			}
			set {
				_data.mGameDuration = value;
				Save ();
			}
		} 
			
		public int SessionNumber {
			get {
				return _data.mSessionNumber;
			}
			set {
				_data.mSessionNumber = value;
				Save ();
			}
		}


		public bool FirstTimeFlowOver {
			get{
				return _data.mFirstTimeFlowOver;
			}
			set{
				_data.mFirstTimeFlowOver = value;
				Save ();
			}
		}

		#endregion

		#region AB Test

		public GPType CurGPType {
			get {
				return _data.curGpType;
			}
			set {
				_data.curGpType = value;
				Save ();
			}
		}

		public GPType CurLocalGPType {
			get {
				return _data.curLocalGpType;
			}
			set {
				_data.curLocalGpType = value;
				Save ();
			}
		}

		#endregion


		JsonData m_JsData;

		public JsonData JsData
		{
			get
			{
				if(m_JsData == null)
				{
					m_JsData = JsonMapper.ToObject(_data.SaveStr);
				}
				return m_JsData;
			}
		}

		public void SetData(string key, bool value)
		{
			JsData[key] = value;
			StateSave ();
		}
		
		public void SetData(string key, int value)
		{
			JsData[key] = value;
			StateSave ();
		}
		
		public void SetData(string key, float value)
		{
			JsData[key] = value;
			StateSave ();
		}
		
		public void SetData(string key, string value)
		{
			JsData[key] = value;
			StateSave ();
		}
		
		public bool GetBool(string key)
		{
			if(!CCTool.JSContainsKey(JsData, key))
			{
				return false;
			}
			string jsStr = JsData[key].ToJson();
			return bool.Parse(jsStr);
		}
		
		public int GetInt(string key)
		{
			if(!CCTool.JSContainsKey(JsData, key))
			{
				return 0;
			}
			string jsStr = JsData[key].ToJson();
			return int.Parse(jsStr);
		}
		
		public float GetFloat(string key)
		{
			if(!CCTool.JSContainsKey(JsData, key))
			{
				return 0;
			}
			string jsStr = JsData[key].ToJson();
			return float.Parse(jsStr);
		}
		
		public string GetString(string key)
		{
			if(!CCTool.JSContainsKey(JsData, key))
			{
				return "";
			}
			string str = JsData[key].ToString();
			return str;
		}

		public bool HasKey(string key)
		{
			if(!CCTool.JSContainsKey(JsData, key))
			{
				return false;
			}
			return true;
		}

		protected void StateSave()
		{
			_data.SaveStr = JsData.ToJson();
			Save ();
		}
	}
}
