using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CocoPlay;
using System.Linq;

namespace Game
{
	public class GameDollDetailItemButton : CocoScrollDragButton
	{

		[SerializeField]
		Image m_Icon;
		[SerializeField]
		Image m_Select;

		string m_CurCategoryName = "";
		int m_Index = 0;

		public static GameDollDetailItemButton Create (Transform parent, string categoryName, int index)
		{
			GameDollDetailItemButton button = CocoLoad.Instantiate<GameDollDetailItemButton> ("prefabs/Dressup_Detail", parent);
			button.InitInfo (categoryName, index);
			return button;
		}

		protected void InitInfo (string categoryName, int index)
		{
			m_Index = index;
			m_CurCategoryName = categoryName;
			this.name = categoryName;
			Init ();
		}

		protected void Init ()
		{
//			m_DressItemData.LinkedDressItemHolder.LoadIconImage (m_Icon);
			m_Icon.SetSprite (string.Format ("{0}/{1}_{2:D3}", m_CurCategoryName, m_CurCategoryName, m_Index));
			m_Icon.SetNativeSize ();

			ChangeStatus ();
		}

		protected override void OnUnRegister ()
		{
			base.OnUnRegister ();
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			DetailBtnClickSignal.AddListener (OnDressupItemBtnClick);
		}

		protected override void RemoveListeners ()
		{
			DetailBtnClickSignal.RemoveListener (OnDressupItemBtnClick);
			base.RemoveListeners ();
		}

		private void OnDressupItemBtnClick (int index)
		{
			ChangeStatus ();
		}

		[Inject]
		public GameDollData dollData {get; set;}

		private void ChangeStatus ()
		{
			if (m_Index == dollData.m_DetailIndex) {
				m_Select.gameObject.SetActive (true);
			} else {
				m_Select.gameObject.SetActive (false);
			}
		}
			
		[Inject]
		public CocoRoleControl roleControl { get; set; }

		[Inject]
		public GameDollDetailItemBtnClickSignal DetailBtnClickSignal { get; set; }

		protected override void OnClick ()
		{

//			if (!m_Select.gameObject.activeInHierarchy) {
//				roleControl.CurRole.Dress.AddDressItem (m_DressItemData.id, modelSet => {
//					dressupItemBtnClickSignal.Dispatch (this);
//				});
//				PlayEffect (m_DressItemData.id);
//			} else {
//				roleControl.CurRole.Dress.RemoveDressItem (m_DressItemData.id, () => {
//					dressupItemBtnClickSignal.Dispatch (this);
//				});
//			}

			List<string> body = roleControl.CurRole.Dress.GetDressIdsByCategory ("body");
			List<string> ear = roleControl.CurRole.Dress.GetDressIdsByCategory ("ear");
			List<string> nose = roleControl.CurRole.Dress.GetDressIdsByCategory ("nose");
			List<string> tail = roleControl.CurRole.Dress.GetDressIdsByCategory ("tail");
			List<string> ids = body.Union(ear).Union(nose).Union(tail).ToList<string>(); 
			foreach (string id in ids){
				var dress = roleControl.CurRole.Dress.GetDressItem (id);
				SkinnedMeshRenderer render = dress.ItemRenderers[0];
				for(int i=0; i<render.materials.Length; i++){
					Texture2D normal = Resources.Load <Texture2D> (string.Format("role/basic/basic/textures/common/material_{0:D3}_nomal", m_Index+1));
					Texture2D rgb = Resources.Load <Texture2D> (string.Format("role/basic/basic/textures/common/material_{0:D3}_rgb", m_Index+1));
					render.materials[i].SetTexture ("_BumpMap", normal);
					render.materials[i].SetTexture ("_metallicRsmoothGdiffuseB", rgb);
				}
			}

			dollData.m_DetailIndex = m_Index;
			DetailBtnClickSignal.Dispatch (m_Index);
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

	}
}
