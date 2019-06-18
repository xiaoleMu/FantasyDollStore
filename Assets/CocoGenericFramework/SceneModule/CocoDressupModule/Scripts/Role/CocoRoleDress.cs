using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TabTale;

#if !COCO_FAKE
using CocoDressCoverLayer = Game.CocoDressCoverLayer;

#else
using CocoDressCoverLayer = CocoPlay.Fake.CocoDressCoverLayer;
#endif


namespace CocoPlay
{
	public class CocoRoleDress : CocoRoleUnitBase, ICocoRoleBodyProvider
	{
		public CocoRoleDressHolder DressHolder { get; private set; }

		CocoDressItemLoader m_ItemLoader = null;

		CocoRoleDressData m_DressData = null;

		bool m_IsReady = false;

		public bool IsReady {
			get {
				return m_IsReady;
			}
		}

		#region Init/Clean

		public override void Init (CocoRoleEntity owner)
		{
			base.Init (owner);

			DressHolder = Owner.RoleHolder.DressHolder;
			if (DressHolder == null) {
				Debug.LogErrorFormat ("[{0}<{1}>]->Init: dress holder NOT exists!", name, GetType ().Name);
			}

			m_DressData = new CocoRoleDressData (((CocoAssetConfigHolder)DressHolder.ParentHolder).ItemHolderDic);
			m_DressData.InitBasicCoverItemDic (Owner.RoleHolder.boneItemId, Owner.RoleHolder.basicItemIds);

			m_ItemLoader = CocoLoad.GetOrAddComponent<CocoDressItemLoader> (gameObject);

			LoadBone ();
		}

		#endregion


		#region Record

		[Inject]
		public CocoDressStateModel dressStateModel { get; set; }

		List<string> GetRecordDressItems ()
		{
			if (!Owner.IsDressRecordActive) {
				return null;
			}

			return dressStateModel.GetRoleDressItems (Owner.DressRecordKey);
		}

		void RecordDressItems ()
		{
			if (!Owner.IsDressRecordActive) {
				return;
			}

			dressStateModel.UpdateRoleDressItems (Owner.DressRecordKey, m_DressData.GetAllDressIds ());
		}

		#endregion


		#region Bone

		Transform m_RootBone = null;
		Dictionary<string, Transform> m_BoneDic = new Dictionary<string, Transform> ();

		void LoadBone ()
		{
			List<string> itemIds = new List<string> ();
			itemIds.Add (m_DressData.BoneItemId);
			m_ItemLoader.LoadItemDict (itemIds, itemModelsDic => {
				KeyValuePair<CocoDressItemHolder, List<GameObject>> kvpItemModels = itemModelsDic.FirstOrDefault ();
				InitBoneDic (kvpItemModels.Value [0].transform);

				// check if exists recorded datas
				List<string> basicItemIds = GetRecordDressItems ();
				if (basicItemIds == null || basicItemIds.Count <= 0) {
					basicItemIds = new List<string> (m_DressData.BasicItemIds);
				}
				LoadBasicModels (basicItemIds);
			});
		}

		void InitBoneDic (Transform root)
		{
			m_RootBone = root;
			m_BoneDic.Clear ();

			Transform[] bones = m_RootBone.GetComponentsInChildren<Transform> ();
			foreach (Transform bone in bones) {
				if (!m_BoneDic.ContainsKey (bone.name)) {
					m_BoneDic.Add (bone.name, bone);
				} else {
					Debug.LogErrorFormat ("[{0}<{1}>]->InitBoneDic: duplicate bone [{2}] in {3}", name, GetType ().Name, bone.name, root.name);
				}
			}
		}

		public void RebindModelBone (GameObject model)
		{
			// bind smr bones
			SkinnedMeshRenderer[] smrs = model.GetComponentsInChildren<SkinnedMeshRenderer> ();
			if (smrs.Length > 0) {
				foreach (SkinnedMeshRenderer smr in smrs) {
					RebindSmrBone (smr);
					smr.transform.SetParent (model.transform, false);
				}
			} else {
				Debug.LogErrorFormat ("[{0}<{1}>]->RebindModelBone: missing smrs for model {2}", name, GetType ().Name, model.name);
			}

			// clean old bones
			foreach (Transform child in model.transform) {
				if (child.GetComponent<SkinnedMeshRenderer> () == null) {
					Destroy (child.gameObject);
				}
			}
		}

		void RebindSmrBone (SkinnedMeshRenderer smr)
		{
			// collect bones
			Transform[] oldBones = smr.bones;
			Transform[] newBones = new Transform[oldBones.Length];
			string boneName = string.Empty;
			for (int i = 0; i < oldBones.Length; i++) {
				boneName = oldBones [i].name;
				if (m_BoneDic.ContainsKey (boneName)) {
					newBones [i] = m_BoneDic [boneName];
				} else {
					Debug.LogErrorFormat ("[{0}<{1}>]->RebindSmrBone: missing bone [{2}] for {3}", name, GetType ().Name, boneName, smr.name);
				}
			}
			smr.bones = newBones;

			// root bone
			boneName = smr.rootBone.name;
			if (m_BoneDic.ContainsKey (boneName)) {
				smr.rootBone = m_BoneDic [boneName];
			} else {
				smr.rootBone = m_RootBone;
			}
            #if !UNITY_5_6_2
			smr.updateWhenOffscreen = true;
            #endif
		}

		#endregion


		#region Basic Model

		void LoadBasicModels (List<string> itemIds)
		{
			AddDressItem (itemIds, modelSets => {
				m_DressData.InitRendererDic (modelSets);
				m_IsReady = true;
			});
		}

		public CocoRoleDressSceneHolder GetDressSceneHolder (string sceneId)
		{
			return DressHolder.SceneHolderDic.GetValue (sceneId);
		}

		#endregion


		#region ICocoRoleBodyProvider implementation

		public Transform GetBone (string boneName)
		{
			return m_BoneDic.GetValue (boneName);
		}

		public SkinnedMeshRenderer GetRenderer (string rendererName)
		{
			return m_DressData.GetRenderer (rendererName);
		}

		public event Action<SkinnedMeshRenderer, bool> OnRendererChanged;

		#endregion


		#region Dress Add/Remove

		public void AddDressItem (List<string> itemIds, Action<List<CocoDressItemModelSet>> endAction = null)
		{
			if (DressIsLoading) {
				return;
			}

			// check if already added
			List<string> shouldAddedItemIds = new List<string> ();
			List<CocoDressItemModelSet> addedItemModelSets = new List<CocoDressItemModelSet> ();
			itemIds.ForEach (itemId => {
				CocoDressItemModelSet modelSet = m_DressData.GetDress (itemId);
				if (modelSet == null) {
					shouldAddedItemIds.Add (itemId);
				} else {
					addedItemModelSets.Add (modelSet);
				}
			});

			// all already added, return them
			if (shouldAddedItemIds.Count <= 0) {
				if (endAction != null) {
					endAction (addedItemModelSets);
				}
				return;
			}

			// load should added items
			DressIsLoading = true;

			// get real should added items
			List<CocoDressItemConflictData> conflictDatas = null;
			shouldAddedItemIds = m_DressData.PrepareAddDressItems (shouldAddedItemIds, out conflictDatas);

			m_ItemLoader.LoadItemDict (shouldAddedItemIds, itemModelsDic => {
				AddDressModelSets (itemModelsDic, conflictDatas);
				DressIsLoading = false;

				if (endAction != null) {
					shouldAddedItemIds.ForEach (itemId => {
						CocoDressItemModelSet modelSet = m_DressData.GetDress (itemId);
						if (modelSet != null) {
							addedItemModelSets.Add (modelSet);
						}
					});

					endAction (addedItemModelSets);
				}
			});
		}

		public void AddDressItem (string itemId, Action<CocoDressItemModelSet> endAction = null)
		{
			AddDressItem (new List<string> (1) { itemId }, modelSets => {
				if (endAction != null) {
					CocoDressItemModelSet modelSet = null;
					if (modelSets.Count > 0) {
						modelSet = modelSets [0];
					}
					endAction (modelSet);
				}
			});
		}

		public void RemoveDressItem (List<string> itemIds, Action endAction = null)
		{
			if (DressIsLoading) {
				return;
			}

			// check if dressed
			List<string> shouldRemoveItemIds = new List<string> ();
			itemIds.ForEach (itemId => {
				CocoDressItemModelSet modelSet = m_DressData.GetDress (itemId);
				if (modelSet != null) {
					shouldRemoveItemIds.Add (itemId);
				}
			});

			// all no dressed, return
			if (shouldRemoveItemIds.Count <= 0) {
				if (endAction != null) {
					endAction ();
				}
				return;
			}

			// get real should added items
			List<CocoDressItemConflictData> conflictDatas = null;
			List<string> shouldAddedItemIds = m_DressData.PrepareRemoveDressItems (shouldRemoveItemIds, out conflictDatas);

			// no need to added
			if (shouldAddedItemIds.Count <= 0) {
				AddDressModelSets (null, conflictDatas);
				if (endAction != null) {
					endAction ();
				}
				return;
			}

			// load should added items
			DressIsLoading = true;

			m_ItemLoader.LoadItemDict (shouldAddedItemIds, modelSets => {
				AddDressModelSets (modelSets, conflictDatas);
				DressIsLoading = false;

				if (endAction != null) {
					endAction ();
				}
			});
		}

		public void RemoveDressItem (string itemId, Action endAction = null)
		{
			RemoveDressItem (new List<string> (1) { itemId }, endAction);
		}

		public void RemoveDressItemByCategory (string category)
		{
			var ids = GetDressIdsByCategory (category);
			RemoveDressItem (ids);
		}

		void AddDressModelSets (Dictionary<CocoDressItemHolder, List<GameObject>> itemModelsDic, List<CocoDressItemConflictData> conflictDatas)
		{
			// process conflicts
			if (conflictDatas != null) {
				conflictDatas.ForEach (conflictData => PorcessConflictData (conflictData));
			}

			// add new items
			if (itemModelsDic != null) {
				itemModelsDic.ForEach ((itemHolder, models) => AddDressModelSet (itemHolder, models));
			}

			RecordDressItems ();

			Resources.UnloadUnusedAssets ();
		}

		void AddDressModelSet (CocoDressItemHolder itemHolder, List<GameObject> itemModels)
		{
			CocoDressItemModelSet modelSet = new CocoDressItemModelSet (itemHolder);

			modelSet.ItemRenderers = new List<SkinnedMeshRenderer> ();
			itemModels.ForEach (model => {
				RebindModelBone (model);

				SkinnedMeshRenderer[] smrs = model.GetComponentsInChildren<SkinnedMeshRenderer> ();
				smrs.ForEach (smr => {
					smr.transform.SetParent (m_RootBone, false);
					smr.gameObject.layer = m_RootBone.gameObject.layer;
					modelSet.ItemRenderers.Add (smr);

					if (OnRendererChanged != null) {
						OnRendererChanged (smr, true);
					}
				});

				Destroy (model);
			});

			m_DressData.AddDress (modelSet);
		}

		void RemoveDressModelSet (string itemId)
		{
			CocoDressItemModelSet modelSet = m_DressData.GetDress (itemId);
			if (modelSet == null) {
				return;
			}

			// destroy smrs
			modelSet.ItemRenderers.ForEach (smr => {
				if (OnRendererChanged != null) {
					OnRendererChanged (smr, false);
				}

				Destroy (smr.gameObject);
			});
			//modelSet.ItemHolder.UnloadAsset ();

			m_DressData.RemoveDress (itemId);
		}

		void PorcessConflictData (CocoDressItemConflictData conflctData)
		{
			RemoveDressModelSet (conflctData.itemHolder.id);
		}

		#endregion


		#region Dress Reset

		public void ResetDressItem (string itemId, System.Action<CocoDressItemHolder> endAction = null)
		{
			ResetDressItem (new List<string> { itemId }, itemHolders => {
				CocoDressItemHolder itemHolder = (itemHolders != null && itemHolders.Count > 0) ? itemHolders [0] : null;
				if (endAction != null) {
					endAction (itemHolder);
				}
			});
		}

		public void ResetDressItem (List<string> itemIds, System.Action<List<CocoDressItemHolder>> endAction = null)
		{
			if (DressIsLoading) {
				return;
			}

			List<CocoDressItemHolder> itemHolders = new List<CocoDressItemHolder> (itemIds.Count);
			itemIds.ForEach (itemId => {
				CocoDressItemModelSet modelSet = m_DressData.GetDress (itemId);
				if (modelSet != null) {
					itemHolders.Add (modelSet.ItemHolder);
				}
			});

			m_ItemLoader.LoadItemDict (itemHolders, itemModelsDic => {
				itemModelsDic.Keys.ForEach (itemHolder => RemoveDressModelSet (itemHolder.id));
				AddDressModelSets (itemModelsDic, null);
				DressIsLoading = false;

				if (endAction != null) {
					endAction (itemHolders);
				}
			});
		}

		#endregion


		#region Dress Get

		public bool DressIsLoading { get; private set; }

		public bool ItemIsDressed (string itemId)
		{
			return m_DressData.ItemIsDressed (itemId);
		}

		public CocoDressItemModelSet GetDressItem (string itemId)
		{
			return m_DressData.GetDress (itemId);
		}

		public List<string> GetDressIdsByCategory (string categoryId)
		{
			return m_DressData.GetDressIdsByCategory (categoryId);
		}

		public List<CocoDressItemModelSet> GetDressItemsByCategory (string categoryId)
		{
			return m_DressData.GetDressItemsByCategory (categoryId);
		}

		public List<string> GetDressItemIdsByCoverLayer (CocoDressCoverLayer coverLayer)
		{
			List<CocoDressItemModelSet> allModelSets = GetAllDressItems ();
			List<string> itemIds = new List<string> (allModelSets.Count);

			allModelSets.ForEach (modelSet => {
				if ((modelSet.ItemHolder.CoverLayer & coverLayer) != 0) {
					 itemIds.Add (modelSet.ItemHolder.id);
				}
			});

			return  itemIds;
		}

		public List<CocoDressItemModelSet> GetDressItemsByCoverLayer (CocoDressCoverLayer coverLayer)
		{
			List<CocoDressItemModelSet> allModelSets = GetAllDressItems ();
			List<CocoDressItemModelSet> modelSets = new List<CocoDressItemModelSet> (allModelSets.Count);

			allModelSets.ForEach (modelSet => {
				if ((modelSet.ItemHolder.CoverLayer & coverLayer) != 0) {
					modelSets.Add (modelSet);
				}
			});

			return modelSets;
		}

		public List<string> GetAllDressIds ()
		{
			return m_DressData.GetAllDressIds ();
		}

		public List<CocoDressItemModelSet> GetAllDressItems ()
		{
			return m_DressData.GetAllDressItems ();
		}

		public List<string> GetAllNonMainItemIds ()
		{
			return m_DressData.GetAllNonMainItemIds ();
		}

		public void SetDressItemsActive (string itemId, bool active)
		{
			CocoDressItemModelSet modelSet = GetDressItem (itemId);
			if (modelSet == null) {
				return;
			}

			modelSet.IsActive = active;
		}

		public void SetDressItemsActiveByCategory (string categoryId, bool active)
		{
			List<CocoDressItemModelSet> modelSets = GetDressItemsByCategory (categoryId);
			modelSets.ForEach (modelSet => modelSet.IsActive = active);
		}

		public void SetDressItemsActiveByCoverLayer (CocoDressCoverLayer coverLayer, bool active)
		{
			List<CocoDressItemModelSet> modelSets = GetAllDressItems ();
			modelSets.ForEach (modelSet => {
				if ((modelSet.ItemHolder.CoverLayer & coverLayer) != 0) {
					modelSet.IsActive = active;
				}
			});
		}

		#endregion


		#region Object (without item holder)

		Dictionary<string, GameObject> m_DressObjectDic = new Dictionary<string, GameObject> ();

		public bool AddDressObject (string id, GameObject go)
		{
			if (m_DressObjectDic.ContainsKey (id)) {
				return false;
			}

			CocoLoad.SetParent (go, transform);
			RebindModelBone (go);
			m_DressObjectDic.Add (id, go);
			return true;
		}

		public bool AddDressObject (string id, string assetPath)
		{
			if (m_DressObjectDic.ContainsKey (id)) {
				return false;
			}

			GameObject go = CocoLoad.Instantiate (assetPath, transform);
			RebindModelBone (go);
			m_DressObjectDic.Add (id, go);
			return true;
		}

		public GameObject GetDressObject (string id)
		{
			return m_DressObjectDic.GetValue (id);
		}

		public bool RemoveDressObject (string id, bool destoryGo = true)
		{
			if (!m_DressObjectDic.ContainsKey (id)) {
				return false;
			}

			GameObject go = m_DressObjectDic [id];
			m_DressObjectDic.Remove (id);
			if (destoryGo) {
				Destroy (go);
			}
			return true;
		}

		#endregion
	}
}
