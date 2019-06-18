using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using CocoPlay;

namespace Game
{
	public class GameDollData
	{

        public CCAnimationData CA_Dressup_Standby = new CCAnimationData("com_standby");
		public CCAnimationData CA_Dressup_Pose1 = new CCAnimationData("du_pose01","voice/player@du_pose01");
		public CCAnimationData CA_Dressup_Pose2 = new CCAnimationData("du_pose02","voice/player@du_pose02");
		public CCAnimationData CA_Dressup_Pose3 = new CCAnimationData("du_pose03","voice/player@du_pose03");
		public CCAnimationData CA_Dressup_top = new CCAnimationData("du_top","voice/player@du_top");
		public CCAnimationData CA_Dressup_buttom = new CCAnimationData("du_buttom","voice/player@du_bottom");


		List <CocoDressupCategoryData> m_CategoryList;

		public GameDollData (){
//			InitDollData ();
		}

		private void InitDollData (){
			m_CategoryList = new List<CocoDressupCategoryData> ();

			CocoDressupCategoryData categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "dress";
            categoryData.m_IconNormalPath = "Sprite/Category/ico_dress_normal";
            categoryData.m_IconSelectedPath = "Sprite/Category/ico_dress_focus";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
            categoryData.m_Animation = CA_Dressup_top;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "bottom";
            categoryData.m_IconNormalPath = "Sprite/Category/ico_shortskirt_normal";
            categoryData.m_IconSelectedPath = "Sprite/Category/ico_shortskirt_focus";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
            categoryData.m_Animation = CA_Dressup_buttom;
			categoryData.m_LockType = CocoLockType.RV;
			m_CategoryList.Add (categoryData);


			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "sock";
            categoryData.m_IconNormalPath = "Sprite/Category/ico_socks_normal";
            categoryData.m_IconSelectedPath = "Sprite/Category/ico_socks_focus";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
            categoryData.m_Animation = CA_Dressup_buttom;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "top";
            categoryData.m_IconNormalPath = "Sprite/Category/ico_jacket_normal";
            categoryData.m_IconSelectedPath = "Sprite/Category/ico_jacket_focus";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
            categoryData.m_Animation = CA_Dressup_top;
			m_CategoryList.Add (categoryData);

			categoryData = new CocoDressupCategoryData ();
			categoryData.CategoryID = "accessories";
            categoryData.m_IconNormalPath = "Sprite/Category/ico_acc_normal";
            categoryData.m_IconSelectedPath = "Sprite/Category/ico_acc_focus";
			categoryData.m_BGNormalPath = "Category/btn_normal";
			categoryData.m_BGSelectPath = "Category/btn_focus";
			categoryData.m_ItemPrefabsPath = "prefabs/Dressup_Item";
			categoryData.m_ItemGridInfo = GetGridInfo (categoryData.CategoryID);
            categoryData.m_Animation = CA_Dressup_top;
			categoryData.m_LockType = CocoLockType.RV;
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
			switch(categoryID)
			{
			case "dress":
			case "bottom":
				info.padding = new RectOffset(0,0,25,25);
				info.cellSize = new Vector2(150, 150);
				info.spacing = new Vector2(0, 15);
				break;

			default:
				info.padding = new RectOffset(0,0,25,25);
				info.cellSize = new Vector2(150, 150);
				info.spacing = new Vector2(0, 15);
				break;
			}
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
