using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace CocoPlay
{
	public class ButtonStateData
	{
		static ButtonStateData _this;
		static public ButtonStateData Instance{
			get{
				if(_this == null)
					_this = new ButtonStateData();
				return _this;
			}
		}

		List<string> m_ReleaseItemList = new List<string>();

		public void Clear()
		{
			m_ReleaseItemList.Clear();
		}

		public void AddRelease(string pStr)
		{
			if(!m_ReleaseItemList.Contains(pStr))
				m_ReleaseItemList.Add(pStr);
		}

		public bool IsReleased(string pStr)
		{
			return m_ReleaseItemList.Contains(pStr);
		}

		public void RemoveRelease(string key)
		{
			m_ReleaseItemList.Remove(key);
		}
	}
}