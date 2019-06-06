#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCPackDressData
{
	List<CCPackItem> m_List;

	public void Init()
	{
		m_List = new List<CCPackItem> ();

		CCPackItem item_body = new CCPackItem();
		item_body.category = "Basic_Body";
		m_List.Add(item_body);
	}
}

#endif






