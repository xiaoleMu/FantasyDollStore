using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;
using System.Linq;
using CocoPlay;

namespace Game
{
	public class GameDollUIControl : GameView
	{
		[Inject]
		public GameDollCategoryBtnClickSignal dressupCategoryBtnClickSignal { get; set; }

		[Inject]
		public CocoRoleControl roleControl { get; set; }

		[Inject]
		public GameGlobalData gameGlobalData { get; set; }

		[SerializeField]
		CocoScrollControl m_CategoryScroll;
		[SerializeField]
		CocoScrollControl m_ItemScroll;

		Dictionary<string, List<CocoRoleDressItemHolder>> m_CategoryItemDic;

		protected override void AddListeners ()
		{
			base.AddListeners ();

			dressupCategoryBtnClickSignal.AddListener (OnDressupCategoryBtnClick);
		}

		protected override void RemoveListeners ()
		{
			dressupCategoryBtnClickSignal.RemoveListener (OnDressupCategoryBtnClick);

			base.RemoveListeners ();
		}

		private SceneStep m_CurStep = SceneStep.Step_Common;

		public void Init (SceneStep step)
		{
			m_CurStep = step;
			InitCategories ();
		}

		void InitCategories ()
		{
			m_CategoryScroll.Clear ();
			CocoRoleDressSceneHolder tSceneHolder = roleControl.CurRole.Dress.GetDressSceneHolder ("common");
			m_CategoryItemDic = tSceneHolder.CategoryItemHolderDic;
			string[] categoryList = GetCategoryList("common");
			GameDollCategoryButton FirstButton = null;

			for (int i = 0; i < categoryList.Length; i++) {
				GameDollCategoryButton button = GameDollCategoryButton.Create (categoryList [i], m_CategoryScroll.GridGroup.transform);
				if (FirstButton == null) {
					FirstButton = button;
				}
			}
			m_CategoryScroll.GridGroup.spacing = new Vector2 (0, 20);
			m_CategoryScroll.InitSize ();

			if (FirstButton != null)
				dressupCategoryBtnClickSignal.Dispatch (FirstButton);
		}

		public string[] GetCategoryList (string sceneID)
		{

			List<string> categoryList = new List<string> ();
			if (m_CurStep == SceneStep.Step_Common){
				categoryList.Add ("body");
				categoryList.Add ("eye");
				categoryList.Add ("ear");
				categoryList.Add ("nose");
				categoryList.Add ("tail");
			}
			else {
				categoryList.Add ("ear");
				categoryList.Add ("nose");
				categoryList.Add ("tail");
			}
			return categoryList.ToArray<string> ();

		}

		private void OnDressupCategoryBtnClick (GameDollCategoryButton pBtn)
		{
			StartCoroutine (UpdateItemList (pBtn.DressupCategoryData));
		}


		IEnumerator UpdateItemList (CocoDressupCategoryData data)
		{
			CocoMainController.Instance.TouchEnable = false;
			m_ItemScroll.ScrollRect.enabled = false;
			RectTransform rectTrans = m_ItemScroll.GridGroup.GetComponent<RectTransform> ();

			CCAction.MoveLocalY (m_ItemScroll.GridGroup.gameObject, -768, 0.3f);
			yield return new WaitForSeconds (0.3f);
			m_ItemScroll.Clear ();
			yield return new WaitForEndOfFrame ();

			InitCategoryItems (data);

			yield return new WaitForEndOfFrame ();

			m_ItemScroll.ScrollRect.enabled = true;
			CocoMainController.Instance.TouchEnable = true;
		}


		List<GameDollItemButton> m_ButtonList = new List<GameDollItemButton> ();

		void InitCategoryItems (CocoDressupCategoryData data)
		{
			m_ItemScroll.GridGroup.SetInfo (data.m_ItemGridInfo);

			List<CocoRoleDressItemHolder> itemDataList = m_CategoryItemDic [data.CategoryID];
			itemDataList = GetItemList (itemDataList, data.CategoryID);

			m_ButtonList.Clear ();
			if (itemDataList != null) {
				for (int i = 0; i < itemDataList.Count; i++) {
                    #region fix
                    CocoRoleDressItemHolder itemHolder = itemDataList[i];
					Debug.LogError ("itemHolder id : " + itemHolder.id);
					int index = i-1;
					if (index> 0) {
						switch (index % 3) {
							case 0:
								itemHolder.lockType = CocoLockType.Non;
							break;
                            case 1:
								itemHolder.lockType = CocoLockType.RV;
                                break;
                            case 2:
								itemHolder.lockType = CocoLockType.IAP;
                                break;
                            default:
								itemHolder.lockType = CocoLockType.Non;
                                break;
                        }
                    } else {
                        itemHolder.lockType = CocoLockType.Non;
                    }

					if (data.m_LockType == CocoLockType.RV){
						itemHolder.lockType = CocoLockType.Non;
					}

                    itemHolder.order = i ;
                    #endregion
					GameDollItemButton button = GameDollItemButton.Create (data.m_ItemPrefabsPath,itemHolder , m_ItemScroll.GridGroup.transform, data.CategoryID);
					m_ButtonList.Add (button);
				}
			}

			m_ItemScroll.InitSize ();
			m_ItemScroll.SetGridEnable (true);
		}

		public static List<CocoRoleDressItemHolder> GetItemList (List<CocoRoleDressItemHolder> itemDataList, string category)
		{
			if(category == "top")
			{
				List<CocoRoleDressItemHolder> firstList = new List<CocoRoleDressItemHolder>();
				List<CocoRoleDressItemHolder> secondList = new List<CocoRoleDressItemHolder>();
				for(int i = 0; i < itemDataList.Count; i++)
				{
					if(itemDataList[i].id == "jacket_001_001" || itemDataList[i].id == "jacket_002_001" || itemDataList[i].id == "jacket_003_001" || itemDataList[i].id == "jacket_004_001"
					   || itemDataList[i].id == "jacket_005_001" || itemDataList[i].id == "jacket_006_001" || itemDataList[i].id == "jacket_007_001" || itemDataList[i].id == "jacket_008_001"
					   || itemDataList[i].id == "jacket_009_001" || itemDataList[i].id == "jacket_010_001")
					{
						firstList.Add(itemDataList[i]);
					}else
					{
						secondList.Add(itemDataList[i]);
					}
				}
				firstList.AddRange(secondList);
				itemDataList = firstList;

			}

			return itemDataList;
		}



		#region UI Ani

		[SerializeField]
		GameObject m_UIPanelObj;

		public IEnumerator ShowAni (){
			LeanTween.moveLocalX (m_UIPanelObj, 0f, 0.3f);
			yield return new WaitForSeconds (0.3f);
		}

		public IEnumerator HideAni (){
			LeanTween.moveLocalX (m_UIPanelObj, 500f, 0.3f);
			yield return new WaitForSeconds (0.3f);
		}

		#endregion
			
	}
}

