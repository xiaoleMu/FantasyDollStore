using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CocoPlay;

namespace Game
{
	public class GameDollItemButton : CocoScrollDragButton
	{

		[SerializeField]
		Image m_Icon;
		[SerializeField]
		Image m_Select;

		public Transform m_GoldTarget;

		public GameObject m_GoldLabel;

		public UnityEngine.Animation priceAnim;
		[HideInInspector]
		public int price;

		protected CocoRoleDressItemHolder m_DressItemData;
		string m_CurCategoryName = "";

        private CocoLockableStateControl lockStateControl;

		public static GameDollItemButton Create (string prefabsPath, CocoRoleDressItemHolder item, Transform parent, string categoryName)
		{
			GameDollItemButton button = CocoLoad.Instantiate<GameDollItemButton> (prefabsPath, parent);
			button.InitInfo (item, categoryName);
			return button;
		}

		protected void InitInfo (CocoRoleDressItemHolder item, string categoryName)
		{
			m_DressItemData = item;
			m_CurCategoryName = categoryName;
			this.name = item.id;
			price = item.price;
			Init ();
		}

		protected void Init ()
		{
			m_DressItemData.LinkedDressItemHolder.LoadIconImage (m_Icon);

			ChangeStatus ();
		}

		protected override void OnUnRegister ()
		{
			base.OnUnRegister ();
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			dressupItemBtnClickSignal.AddListener (OnDressupItemBtnClick);
		}

		protected override void RemoveListeners ()
		{
			dressupItemBtnClickSignal.RemoveListener (OnDressupItemBtnClick);
			base.RemoveListeners ();
		}

		private void OnDressupItemBtnClick (GameDollItemButton button)
		{
			ChangeStatus ();
		}

		private void ChangeStatus ()
		{
			if (roleControl.CurRole.Dress.ItemIsDressed (m_DressItemData.id)) {
				m_Select.gameObject.SetActive (true);
			} else {
				m_Select.gameObject.SetActive (false);
			}
		}

		public CocoRoleDressItemHolder DressupItemData {
			get {
				return m_DressItemData;
			}
		}

		[Inject]
		public CocoRoleControl roleControl { get; set; }

		[Inject]
		public GameDollItemBtnClickSignal dressupItemBtnClickSignal { get; set; }

		protected override void OnClick ()
		{

			if (!m_Select.gameObject.activeInHierarchy) {
				roleControl.CurRole.Dress.AddDressItem (m_DressItemData.id, modelSet => {
					dressupItemBtnClickSignal.Dispatch (this);
				});
				PlayEffect (m_DressItemData.id);
			} else {
				roleControl.CurRole.Dress.RemoveDressItem (m_DressItemData.id, () => {
					dressupItemBtnClickSignal.Dispatch (this);
				});
			}
		}

		void PlayEffect (string id)
		{
//			CocoDressItemModelSet dressItemModel = roleControl.CurRole.Dress.GetDressItem (id);
//			dressItemModel.ItemRenderers.ForEach (smr => {
//				CocoSkinnedMeshEffect.Create (smr);
//			});
//			CocoAudio.PlaySound(CocoAudioID.Button_Click_15);
//			int gold = 0;
		}

		void ProcessScore ()
		{
			int score = 0;

		}

	}
}
