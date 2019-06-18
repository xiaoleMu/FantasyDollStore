using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;


namespace CocoPlay
{
	public class CocoDressCategoryConfigHolder : CocoAssetJsonHolder<CocoDressCategoryHolder>
	{
	}


	public class CocoDressSceneConfigHolder : CocoAssetJsonHolder<CocoDressSceneHolder>
	{
	}


	public class CocoRoleDressConfigHolder : CocoAssetJsonHolder<CocoRoleDressHolder>
	{
	}


	public class CocoRoleConfigHolder : CocoAssetJsonHolder<CocoRoleHolder>
	{
	}


	public class CocoRoleBodyConfigHolder : CocoAssetJsonHolder<CocoRoleBodyHolder>
	{
	}


	public class CocoAssetConfigHolder : CocoAssetHolderBase
	{
		public List<CocoDressCategoryConfigHolder> categoryConfigHolders = new List<CocoDressCategoryConfigHolder> ();
		public List<CocoDressSceneConfigHolder> sceneConfigHolders = new List<CocoDressSceneConfigHolder> ();

		public List<CocoRoleDressConfigHolder> roleDressConfigHolders = new List<CocoRoleDressConfigHolder> ();
		public List<CocoRoleBodyConfigHolder> roleBodyConfigHolders = new List<CocoRoleBodyConfigHolder> ();
		public List<CocoRoleConfigHolder> roleConfigHolders = new List<CocoRoleConfigHolder> ();


		#region implemented abstract members of CocoHolderBase

		protected override void LoadProcess ()
		{
			LoadSubHolderAssets (categoryConfigHolders);
			LoadSubHolderAssets (sceneConfigHolders);
			LoadSubHolderAssets (roleDressConfigHolders);
			LoadSubHolderAssets (roleBodyConfigHolders);
			LoadSubHolderAssets (roleConfigHolders);
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			yield return LoadAsyncSubHolderAssets (categoryConfigHolders);
			yield return LoadAsyncSubHolderAssets (sceneConfigHolders);
			yield return LoadAsyncSubHolderAssets (roleDressConfigHolders);
			yield return LoadAsyncSubHolderAssets (roleBodyConfigHolders);
			yield return LoadAsyncSubHolderAssets (roleConfigHolders);
		}

		protected override void UnloadProcess ()
		{
			UnloadSubHolderAssets (categoryConfigHolders);
			UnloadSubHolderAssets (sceneConfigHolders);
			UnloadSubHolderAssets (roleDressConfigHolders);
			UnloadSubHolderAssets (roleBodyConfigHolders);
			UnloadSubHolderAssets (roleConfigHolders);
		}

		#endregion


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);

			LinkSubHolders (categoryConfigHolders);
			LinkCategoryHolders ();

			LinkSubHolders (sceneConfigHolders);
			LinkSceneHolders ();

			LinkSubHolders (roleDressConfigHolders);
			LinkRoleDressHolders ();

			LinkSubHolders (roleBodyConfigHolders);
			LinkRoleBodyHolders ();

			LinkSubHolders (roleConfigHolders);
			LinkRoleHolders ();
		}

		private void LinkCategoryHolders ()
		{
			m_CategoryHolderDic = new Dictionary<string, CocoDressCategoryHolder> (categoryConfigHolders.Count);
			m_ItemHolderDic = new Dictionary<string, CocoDressItemHolder> ();

			categoryConfigHolders.ForEach (holder => {
				var categoryHolder = holder.AssetEntity;
				categoryHolder.LinkParent (this);

				m_CategoryHolderDic.Add (categoryHolder.id, categoryHolder);
				categoryHolder.ItemHolderDic.ForEach ((itemHolder) => {
					if (!m_ItemHolderDic.ContainsKey (itemHolder.id)) {
						m_ItemHolderDic.Add (itemHolder.id, itemHolder);
					} else {
						Debug.LogError (itemHolder.id);
					}
				});
			});
		}

		private void LinkSceneHolders ()
		{
			m_SceneHolderDic = new Dictionary<string, CocoDressSceneHolder> (sceneConfigHolders.Count);
			sceneConfigHolders.ForEach (holder => {
				var sceneHolder = holder.AssetEntity;
				sceneHolder.LinkParent (this);
				m_SceneHolderDic.Add (sceneHolder.id, sceneHolder);
			});
		}

		private void LinkRoleDressHolders ()
		{
			m_RoleDressHolderDic = new Dictionary<string, CocoRoleDressHolder> (roleDressConfigHolders.Count);
			roleDressConfigHolders.ForEach (holder => {
				var roleDressHolder = holder.AssetEntity;
				roleDressHolder.LinkParent (this);
				m_RoleDressHolderDic.Add (roleDressHolder.id, roleDressHolder);
			});
		}

		private void LinkRoleBodyHolders ()
		{
			m_RoleBodyHolderDic = new Dictionary<string, CocoRoleBodyHolder> (roleBodyConfigHolders.Count);
			roleBodyConfigHolders.ForEach (holder => {
				var roleBodyHolder = holder.AssetEntity;
				roleBodyHolder.LinkParent (this);
				m_RoleBodyHolderDic.Add (roleBodyHolder.id, roleBodyHolder);
			});
		}

		private void LinkRoleHolders ()
		{
			m_RoleHolderDic = new Dictionary<string, CocoRoleHolder> (roleConfigHolders.Count);
			roleConfigHolders.ForEach (holder => {
				var roleHolder = holder.AssetEntity;
				roleHolder.LinkParent (this);
				m_RoleHolderDic.Add (roleHolder.id, roleHolder);
			});
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("categoryConfigHolders", categoryConfigHolders, subIndentations);
			str += PrintSubHolders ("sceneConfigHolders", sceneConfigHolders, subIndentations);
			str += PrintSubHolders ("roleDressConfigHolders", roleDressConfigHolders, subIndentations);
			str += PrintSubHolders ("roleBodyConfigHolders", roleBodyConfigHolders, subIndentations);
			str += PrintSubHolders ("roleConfigHolders", roleConfigHolders, subIndentations);
			str += '\n';
			str += PrintSubHolders ("CategoryHolderDic", CategoryHolderDic, subIndentations);
			str += PrintSubHolders ("SceneHolderDic", SceneHolderDic, subIndentations);
			str += PrintSubHolders ("RoleDressHolderDic", RoleDressHolderDic, subIndentations);
			str += PrintSubHolders ("RoleBodyHolderDic", RoleBodyHolderDic, subIndentations);
			str += PrintSubHolders ("RoleHolderDic", RoleHolderDic, subIndentations);
			str += indentations;
			return str;
		}

		#endregion


		#region Load/Save (File)

		public void LoadSubConfigs (string rootDirectory)
		{
			categoryConfigHolders.ForEach (holder => holder.LoadFromFile (rootDirectory));
			sceneConfigHolders.ForEach (holder => holder.LoadFromFile (rootDirectory));

			roleDressConfigHolders.ForEach (holder => holder.LoadFromFile (rootDirectory));
			roleBodyConfigHolders.ForEach (holder => holder.LoadFromFile (rootDirectory));
			roleConfigHolders.ForEach (holder => holder.LoadFromFile (rootDirectory));
		}

		public void SaveSubConfigs (string rootDirectory, bool prettyPrint)
		{
			categoryConfigHolders.ForEach (holder => holder.SaveToFile (rootDirectory, prettyPrint));
			sceneConfigHolders.ForEach (holder => holder.SaveToFile (rootDirectory, prettyPrint));

			roleDressConfigHolders.ForEach (holder => holder.SaveToFile (rootDirectory, prettyPrint));
			roleBodyConfigHolders.ForEach (holder => holder.SaveToFile (rootDirectory, prettyPrint));
			roleConfigHolders.ForEach (holder => holder.SaveToFile (rootDirectory, prettyPrint));
		}

		#endregion


		#region Config Assets

		private Dictionary<string, CocoDressCategoryHolder> m_CategoryHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoDressCategoryHolder> CategoryHolderDic {
			get { return m_CategoryHolderDic; }
		}

		public CocoDressCategoryHolder GetCategoryHolder (string categoryId)
		{
			return CategoryHolderDic.GetValue (categoryId);
		}

		private Dictionary<string, CocoDressItemHolder> m_ItemHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoDressItemHolder> ItemHolderDic {
			get { return m_ItemHolderDic; }
		}

		public CocoDressItemHolder GetItemHolder (string categoryId, string itemId)
		{
			var categoryHolder = GetCategoryHolder (categoryId);
			if (categoryHolder == null) {
				return null;
			}

			return categoryHolder.ItemHolderDic.GetValue (itemId);
		}

		public CocoDressItemHolder GetItemHolder (string itemId)
		{
			return ItemHolderDic.GetValue (itemId);
		}

		public List<CocoDressItemHolder> GetItemHolders (List<string> itemIds)
		{
			var itemHolders = new List<CocoDressItemHolder> (itemIds.Count);
			itemIds.ForEach (itemId => {
				if (ItemHolderDic.ContainsKey (itemId)) {
					itemHolders.Add (ItemHolderDic [itemId]);
				} else {
					Debug.LogErrorFormat ("[{0}]->LoadItems: item id [{1}] NOT exists !", GetType ().Name, itemId);
				}
			});
			return itemHolders;
		}

		private Dictionary<string, CocoDressSceneHolder> m_SceneHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoDressSceneHolder> SceneHolderDic {
			get { return m_SceneHolderDic; }
		}

		private Dictionary<string, CocoRoleDressHolder> m_RoleDressHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoRoleDressHolder> RoleDressHolderDic {
			get { return m_RoleDressHolderDic; }
		}

		public CocoRoleDressHolder GetRoleDressHolder (string dressId)
		{
			var holder = RoleDressHolderDic.GetValue (dressId);
			if (holder == null) {
				holder = RoleDressHolderDic.GetValue ("common");
			}
			return holder;
		}

		private Dictionary<string, CocoRoleBodyHolder> m_RoleBodyHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoRoleBodyHolder> RoleBodyHolderDic {
			get { return m_RoleBodyHolderDic; }
		}

		public CocoRoleBodyHolder GetRoleBodyHolder (string bodyId)
		{
			var holder = m_RoleBodyHolderDic.GetValue (bodyId);
			if (holder == null) {
				holder = m_RoleBodyHolderDic.GetValue ("common");
			}
			return holder;
		}

		private Dictionary<string, CocoRoleHolder> m_RoleHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoRoleHolder> RoleHolderDic {
			get { return m_RoleHolderDic; }
		}

		public CocoRoleHolder GetRoleHolder (string roleId)
		{
			var holder = m_RoleHolderDic.GetValue (roleId);
			if (holder == null) {
				holder = m_RoleHolderDic.GetValue ("common");
			}
			return holder;
		}

		#endregion


		#region Get Random Item

		public CocoDressItemHolder GetRandomItemHolder ()
		{
			if (CategoryHolderDic.Count <= 0) {
				return null;
			}

			var categoryHolders = new List<CocoDressCategoryHolder> (CategoryHolderDic.Values);
			var categoryHolder = categoryHolders [Random.Range (0, categoryHolders.Count)];
			if (categoryHolder == null || categoryHolder.itemHolders.Count <= 0) {
				return null;
			}

			return categoryHolder.itemHolders [Random.Range (0, categoryHolder.itemHolders.Count)];
		}

		public CocoDressItemHolder GetRandomItemHolder (string categoryId)
		{
			var categoryHolder = GetCategoryHolder (categoryId);
			if (categoryHolder == null || categoryHolder.itemHolders.Count <= 0) {
				return null;
			}

			return categoryHolder.itemHolders [Random.Range (0, categoryHolder.itemHolders.Count)];
		}

		public CocoRoleDressItemHolder GetRoleRandomItemHolder (string dressId, bool onlyOwned = false)
		{
			var roleDressHolder = GetRoleDressHolder (dressId);
			if (roleDressHolder == null) {
				return null;
			}

			return roleDressHolder.GetRandomItemHolder (onlyOwned);
		}

		public CocoRoleDressItemHolder GetRoleRandomItemHolder (string dressId, string categoryId, bool onlyOwned = false)
		{
			var roleDressHolder = GetRoleDressHolder (dressId);
			if (roleDressHolder == null) {
				return null;
			}

			return roleDressHolder.GetRandomItemHolder (categoryId, onlyOwned);
		}

		public CocoRoleDressItemHolder GetRoleRandomItemHolder (string dressId, string sceneId, string categoryId, bool onlyOwned = false)
		{
			var roleDressHolder = GetRoleDressHolder (dressId);
			if (roleDressHolder == null) {
				return null;
			}

			return roleDressHolder.GetRandomItemHolder (sceneId, categoryId, onlyOwned);
		}

		public List<string> GetRandomSuitCategoryIds (IEnumerable<string> categoryIds)
		{
			var suitCategoryIds = new List<string> ();

			foreach (var categoryId in categoryIds) {
				var categoryHolder = GetCategoryHolder (categoryId);
				if (categoryHolder.isMain || Random.value <= 0.5f) {
					suitCategoryIds.Add (categoryId);
				}
			}

			return suitCategoryIds;
		}

		#endregion
	}
}