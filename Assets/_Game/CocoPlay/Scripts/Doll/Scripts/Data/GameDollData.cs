using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using CocoPlay;

namespace Game
{
	public class GameDollData
	{

		public CCAnimationData CA_Dressup_Standby = new CCAnimationData("standby01");
		public CCAnimationData CA_Dressup_Pose1 = new CCAnimationData("standby02");
		public CCAnimationData CA_Dressup_Pose2 = new CCAnimationData("standby03");
		public CCAnimationData CA_Dressup_ear = new CCAnimationData("du_ear");
		public CCAnimationData CA_Dressup_body = new CCAnimationData("du_body");
		public CCAnimationData CA_Dressup_sit = new CCAnimationData("sit");
		public CCAnimationData CA_Dressup_win01 = new CCAnimationData("win01");
		public CCAnimationData CA_Dressup_win02 = new CCAnimationData("win02");


		List <CocoDressupCategoryData> m_CategoryList;

		public int curSelectRole = -1;

		public GameDollData (){
			InitDollData ();
		}

		private void InitDollData (){
			m_CategoryList = new List<CocoDressupCategoryData> ();

			CocoDressupCategoryData categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "body";
			categoryData.m_IconNormalPath = "Sprite/Category/pattern";
			categoryData.m_IconSelectedPath = "Sprite/Category/pattern_xz";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
			categoryData.m_Animation = CA_Dressup_body;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "ear";
			categoryData.m_IconNormalPath = "Sprite/Category/Ears";
			categoryData.m_IconSelectedPath = "Sprite/Category/Ears_xz";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
			categoryData.m_Animation = CA_Dressup_ear;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "eye";
			categoryData.m_IconNormalPath = "Sprite/Category/eyes";
			categoryData.m_IconSelectedPath = "Sprite/Category/eyes_xz";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
			categoryData.m_Animation = CA_Dressup_ear;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "nose";
			categoryData.m_IconNormalPath = "Sprite/Category/mouth";
			categoryData.m_IconSelectedPath = "Sprite/Category/mouth_xz";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
			categoryData.m_Animation = CA_Dressup_ear;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "tail";
			categoryData.m_IconNormalPath = "Sprite/Category/tails";
			categoryData.m_IconSelectedPath = "Sprite/Category/tails_xz";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
			categoryData.m_Animation = CA_Dressup_ear;
			m_CategoryList.Add (categoryData);


		}

		public CocoDressupCategoryData GetCategoryData(string pCategoryID){
			foreach (CocoDressupCategoryData dressupCategoryData in m_CategoryList){
				if (dressupCategoryData.CategoryID == pCategoryID){
					return dressupCategoryData;
				}
			}

			return null;
		}
			
		protected virtual GridInfo GetGridInfo(string categoryID)
		{
			GridInfo info = new GridInfo();
//			switch(categoryID)
//			{
//			case "dress":
//			case "bottom":
//				info.padding = new RectOffset(0,0,25,25);
//				info.cellSize = new Vector2(150, 150);
//				info.spacing = new Vector2(0, 15);
//				break;
//
//			default:
//				info.padding = new RectOffset(0,0,25,25);
//				info.cellSize = new Vector2(150, 150);
//				info.spacing = new Vector2(0, 15);
//				break;
//			}

			info.padding = new RectOffset(10,0,20,0);
			info.cellSize = new Vector2(320, 160);
			info.spacing = new Vector2(0, 20);

			return info;
		}
	}


	public class CocoDressupCategoryData
	{
		public string CategoryID;
		// icon
		public string m_IconNormalPath;  //通用icon路径
		public string m_IconSelectedPath;//选择icon路径

		// bg
		public string m_BGNormalPath; //通用icon背景路径
		public string m_BGSelectPath; //通用icon背景路径

		// item
		public string m_ItemPrefabsPath; //预设路径
		public GridInfo m_ItemGridInfo; //item 大小间距信息

		// Animation
		public CCAnimationData m_Animation = null;

		public CocoLockType m_LockType = CocoLockType.Non;

	}

}
