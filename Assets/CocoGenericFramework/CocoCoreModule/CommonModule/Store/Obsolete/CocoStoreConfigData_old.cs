using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if COCO_FAKE
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#else
using CocoStoreID = Game.CocoStoreID;
using CocoSceneID = Game.CocoSceneID;
#endif

namespace CocoPlay
{
	public class CocoStoreConfigData_old : ICocoStoreConfigData
	{
		#if ABTEST

		protected Dictionary<CocoStoreID, string> m_ConfigData = new Dictionary<CocoStoreID, string> ();
		protected Dictionary<CocoStoreID, string> m_ConfigData_TypeA = new Dictionary<CocoStoreID, string> ();
		protected Dictionary<CocoStoreID, string> m_ConfigData_TypeB = new Dictionary<CocoStoreID, string> ();

		protected void AddItem (CocoStoreID ID, string key)
		{
			m_ConfigData.Add (ID, key);
		}

		protected void AddItem_TypeA (CocoStoreID ID, string key)
		{
			m_ConfigData_TypeA.Add (ID, key);
		}

		protected void AddItem_TypeB (CocoStoreID ID, string key)
		{
			m_ConfigData_TypeB.Add (ID, key);
		}

		public string GetStoreKey (CocoStoreID ID)
		{
			string key = string.Empty;
			if (m_ConfigData.ContainsKey (ID))
				m_ConfigData.TryGetValue (ID, out key);
			return key;
		}

		public string GetStoreKey_TypeA (CocoStoreID ID)
		{
			string key = string.Empty;
			if (m_ConfigData_TypeA.ContainsKey (ID))
				m_ConfigData_TypeA.TryGetValue (ID, out key);
			return key;
		}

		public string GetStoreKey_TypeB (CocoStoreID ID)
		{
			string key = string.Empty;
			if (m_ConfigData_TypeB.ContainsKey (ID))
				m_ConfigData_TypeB.TryGetValue (ID, out key);
			return key;
		}

		public List<string> GetStoreKeyList ()
		{
			List<string> list = new List<string> ();
			foreach (var key in m_ConfigData.Values) {
				list.Add (key);
			}
			return list;
		}

		public List<string> GetStoreKeyList_TypeA ()
		{
			List<string> list = new List<string> ();
			foreach (var key in m_ConfigData_TypeA.Values) {
				list.Add (key);
			}
			return list;
		}

		public List<string> GetStoreKeyList_TypeB ()
		{
			List<string> list = new List<string> ();
			foreach (var key in m_ConfigData_TypeB.Values) {
				list.Add (key);
			}
			return list;
		}

		public virtual Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot ()
		{
			return new Dictionary<CocoStoreID, List<CocoStoreID>> ();
		}

		public virtual Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot_TypeA ()
		{
			return new Dictionary<CocoStoreID, List<CocoStoreID>> ();
		}

		public virtual Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot_TypeB ()
		{
			return new Dictionary<CocoStoreID, List<CocoStoreID>> ();
		}

		public virtual Dictionary<CocoStoreID, List<CocoStoreID>> GetABVersionSlot (){
			return new Dictionary<CocoStoreID, List<CocoStoreID>> ();
		}

		#else

		protected Dictionary<CocoStoreID, string> m_ConfigData = new Dictionary<CocoStoreID, string> ();

		protected void AddItem (CocoStoreID ID, string key)
		{
			m_ConfigData.Add (ID, key);
		}

		public string GetStoreKey (CocoStoreID ID)
		{
			string key;
			m_ConfigData.TryGetValue (ID, out key);
			return key;
		}

		public List<string> GetStoreKeyList ()
		{
			List<string> list = new List<string> ();
			foreach (var key in m_ConfigData.Values) {
				list.Add (key);
			}
			return list;
		}

		public virtual Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot ()
		{
			return new Dictionary<CocoStoreID, List<CocoStoreID>> ();
		}

		public List<CocoStoreItemData> ItemDatas {
			get { return null; }
		}

		public CocoStoreItemData GetItemData (CocoStoreID itemId)
		{
			throw new System.NotImplementedException ();
		}

		public CocoStoreItemData GetItemData (string itemKey)
		{
			throw new System.NotImplementedException ();
		}

		public IEnumerable<CocoStoreID> ItemIds {
			get { return m_ConfigData.Keys; }
		}

#endif
	}
}
