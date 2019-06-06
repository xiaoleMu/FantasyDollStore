using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TabTale.AssetManagement;
using TabTale;
using System.Linq;

namespace TabTale
{
	public abstract class GenericViewList<TItemView,TItemData,TDataId> : MonoBehaviour where TItemView:class
	{

		public PrefabData prefabDataFirst, prefabData;

		public List <GameObject> gameObjectList = new List<GameObject> ();
		public Action<TItemView> ItemAddedAction = t => {};
		public Action<TItemView> ItemRemovedAction = t => {};
		protected IAssetManager _assetManager;
		protected List <TItemView> _itemList = new List<TItemView> ();

		protected virtual void Awake ()
		{
			if(GameApplication.Instance == null)
				return;
			
			_assetManager = GameApplication.Instance.AssetManager;

			HandleLoadPrefab(prefabDataFirst);
			HandleLoadPrefab(prefabData);

			if(_itemList.Count==0){
				for (int i=0; i<gameObjectList.Count; i++) {
					TItemView view = gameObjectList [i].GetComponentOrInterface<TItemView> ();
					_itemList.Add(view);
					OnItemAdded (view,gameObjectList [i]);
				}
			}
		}

		public List<TItemView> ItemList {
			get {
				return _itemList;
			}
		}

		void HandleLoadPrefab (PrefabData prefabData)
		{
			if (prefabData.itemPrefab == null && prefabData.itemPrefabPath != ""){
				LoadPrefab(prefabData.itemPrefabPath);
			}
		}

		public void LoadPrefab(string prefabPath){
			if(prefabPath==prefabData.itemPrefabPath && prefabData.itemPrefab!=null)
				return;

			_assetManager = GameApplication.Instance.AssetManager;

			prefabData.itemPrefabPath=prefabPath;
			prefabData.itemPrefab = _assetManager.GetResource<GameObject> (prefabPath);
			if (prefabData.itemPrefab == null) {
				CoreLogger.LogError ("GenericViewList", "LoadPrefab cannot find prefab " + prefabData.itemPrefabPath);
			}
		}

		protected GameObject AddItemGo ()
		{
			GameObject itemPrefab;

			if (_itemList.Count == 0 && prefabDataFirst.itemPrefab != null){
				itemPrefab = prefabDataFirst.itemPrefab;
			}else{
				itemPrefab = prefabData.itemPrefab;
			}

			if (itemPrefab == null) {
				CoreLogger.LogError ("GenericViewList", "AddItemGo missing prefab");
				return null;
			}
			return Instantiate (itemPrefab) as GameObject;
		}

		public TItemView AddItem ()
		{

			TItemView itemView = null;

			GameObject itemGo = AddItemGo ();
			if (itemGo == null)
				return itemView;

			itemGo.transform.SetParent (transform, false);

			itemView = itemGo.GetComponentOrInterface<TItemView> ();

			if (itemView == null) {
				CoreLogger.LogError ("GenericViewList", "AddItem " + itemGo + " missing component " + typeof(TItemView));
				Destroy (itemGo);
				return itemView;
			}

			_itemList.Add (itemView);
			gameObjectList.Add (itemGo);

			OnItemAdded (itemView,itemGo);

			return itemView;
		}

		protected virtual void OnItemAdded (TItemView itemView,GameObject itemGo)
		{
			ItemAddedAction (itemView);
		}

		public TItemView AddItem (TItemData data)
		{
			TItemView item = AddItem ();

			if (item == null)
				return item;

			IGenericListData<TItemData> itemData = item as IGenericListData<TItemData>;
			if (itemData != null)
				itemData.SetData (data);

			return item;
		}

		public void SetItems (List<TItemData> dataList)
		{
			Reset ();
			AddItems (dataList);
		}

		public void AddItems (List<TItemData> dataList)
		{
			for (int i=0; i<dataList.Count; i++) {
				AddItem (dataList [i]);
			}
		}

		public int GetItemIndexById(TDataId id){
			for (int i = 0; i < _itemList.Count; i++) {
				TItemView item = _itemList[i];
				if (item is IIdentifiable<TDataId>){
					if (((IIdentifiable<TDataId>)item).Identify(id)){
						return i;
					}
				}
			}
			
			return -1;
		}
		
		public TItemView GetItemById(TDataId id){
			int foundItemIndex = GetItemIndexById(id);
			if (foundItemIndex != -1){
				return _itemList[foundItemIndex];
			}
			return null;
		}

		public void RemoveItemById(TDataId id){
			int index = GetItemIndexById(id);
			if(index!=-1)
				RemoveItemAt(index);
		}

		public void RemoveItemAt (int index)
		{
			if (index >= _itemList.Count || index < 0)
				return;

			OnItemRemoved (_itemList [index],gameObjectList [index]);

			Destroy (gameObjectList [index]);
			_itemList.RemoveAt (index);
			gameObjectList.RemoveAt (index);
		}

		protected virtual void OnItemRemoved (TItemView itemView,GameObject itemGo)
		{
			ItemRemovedAction (itemView);
		}

		public void Reset ()
		{
			for (int i=_itemList.Count-1; i>=0; i--) {
				RemoveItemAt (i);
			}
		}

		public void OnDestroy ()
		{
			Reset ();
			ItemAddedAction = null;
			ItemRemovedAction = null;
		}

	}

	public class GenericViewList<TItemView,TItemData> : GenericViewList<TItemView,TItemData,string> where TItemView:class
	{
		
	}

	public class GenericViewList<TItemView> : GenericViewList<TItemView,object,string> where TItemView:class
	{

	}

	public class GenericViewList : GenericViewList<Component,object,string>
	{

	}

	public interface IGenericListData<TData>
	{
		void SetData (TData data);
	}	

	[Serializable]
	public class PrefabData{
		public GameObject itemPrefab;
		public string itemPrefabPath;
	}


}

