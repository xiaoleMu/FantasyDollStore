using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using CocoPlay;

namespace Game{
	public class GameDollCategoryButton : CocoScrollDragButton
	{
		[Inject] 
		public GameDollCategoryBtnClickSignal dressupCategoryBtnClick {get; set;}

		private const string DefaultDressupCategoryPath = "prefabs/Dressup_Category";

		CocoDressupCategoryData m_DressupCategoryData;
		bool m_IsSelect = false;

		[SerializeField]
		Image m_CategoryIcon;
		[SerializeField]
		Image m_CategoryBG;

		public CocoLockType selfPropertyLockType = CocoLockType.Non;

		protected override void AddListeners ()
		{
			base.AddListeners ();

			dressupCategoryBtnClick.AddListener (OnDressupCategoryBtnClick);
		}

		protected override void RemoveListeners ()
		{
			dressupCategoryBtnClick.RemoveListener (OnDressupCategoryBtnClick);

			base.RemoveListeners ();
		}

		public static GameDollCategoryButton Create (string catergoryID, Transform parent)
		{
			GameDollCategoryButton button = CocoLoad.Instantiate<GameDollCategoryButton> (DefaultDressupCategoryPath, parent);
			button.InitInfo (catergoryID);
			return button;
		}

		[Inject]
		public GameDollData DressupData {get; set;}
		void InitInfo (string catergoryID)
		{
			m_DressupCategoryData = DressupData.GetCategoryData (catergoryID);
			this.name = m_DressupCategoryData.CategoryID;

			ChangeStatus ();

		} 

		public CocoDressupCategoryData DressupCategoryData {
			get {
				return m_DressupCategoryData;
			}
		}

		private void OnDressupCategoryBtnClick (GameDollCategoryButton pBtn){
			if (pBtn.DressupCategoryData.CategoryID == DressupCategoryData.CategoryID){
				m_IsSelect = true;
			}
			else{
				m_IsSelect = false;
			}

			ChangeStatus ();

		}

		private void ChangeStatus (){
			if (m_IsSelect) {
				CocoSprite.SetSprite (m_CategoryIcon, DressupCategoryData.m_IconSelectedPath, true);
				CocoSprite.SetSprite (m_CategoryBG, DressupCategoryData.m_BGSelectPath, true);
			} else {
				CocoSprite.SetSprite (m_CategoryIcon, DressupCategoryData.m_IconNormalPath, true);
				CocoSprite.SetSprite (m_CategoryBG, DressupCategoryData.m_BGNormalPath, true);
			}
		}

		protected override bool IsTouchEnabled {
			get {
				if (m_IsSelect) {
					return false;
				}
				return base.IsTouchEnabled;
			}
		}

		protected override void OnClick ()
		{
			dressupCategoryBtnClick.Dispatch (this);
		}

	}
}
