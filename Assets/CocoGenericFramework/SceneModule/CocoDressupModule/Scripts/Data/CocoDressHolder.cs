using System.Collections;
using System.Collections.Generic;
using Game;
using LitJson;
using UnityEngine.UI;


namespace CocoPlay
{
	public partial class CocoDressItemHolder : CocoAssetHolderBase
	{
		public List<CocoAssetModelHolder> modelHolders = new List<CocoAssetModelHolder> ();
		public List<CocoAssetSpriteHolder> spriteHolders = new List<CocoAssetSpriteHolder> ();


		#region implemented abstract members of CocoHolderBase

		protected override void LoadProcess ()
		{
			LoadSubHolderAssets (modelHolders);
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			yield return LoadAsyncSubHolderAssets (modelHolders);
		}

		protected override void UnloadProcess ()
		{
			UnloadSubHolderAssets (modelHolders);
		}

		#endregion


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (modelHolders);
			LinkSubHolders (spriteHolders);
		}

		[JsonIgnore]
		public override CocoDressCoverLayer CoverLayer {
			get {
				var layer = base.CoverLayer;
				if (layer == CocoDressCoverLayer.None) {
					layer = ((CocoDressCategoryHolder)ParentHolder).CoverLayer;
				}
				return layer;
			}
		}

		private HashSet<CocoRoleDressItemHolder> m_LinkedRoleDressItemHolders = new HashSet<CocoRoleDressItemHolder> ();

		[JsonIgnore]
		public HashSet<CocoRoleDressItemHolder> LinkedRoleDressItemHolders {
			get { return m_LinkedRoleDressItemHolders; }
		}

		public void LinkRoleDressItemHolder (CocoRoleDressItemHolder roleDressItemHolder)
		{
			if (m_LinkedRoleDressItemHolders.Contains (roleDressItemHolder)) {
				return;
			}

			m_LinkedRoleDressItemHolders.Add (roleDressItemHolder);
		}

		[JsonIgnore]
		public CocoDressSceneItemHolder LinkedSceneItemHolder { get; set; }

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = string.Format ("{0}\n", base.PrintContent (indentations));
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("modelHolders", modelHolders, subIndentations);
			str += PrintSubHolders ("spriteHolders", spriteHolders, subIndentations);
			str += indentations;
			return str;
		}

		#endregion


		#region Icon

		public void LoadIconImage (Image image, bool nativeSize = false)
		{
			if (spriteHolders.Count <= 0) {
				return;
			}

			LoadIconAssets ();

			UpdateIconImage (image, nativeSize);
			UnloadIconAssets ();
		}

		public IEnumerator LoadAsyncIconImage (Image image, bool nativeSize = false)
		{
			if (spriteHolders.Count <= 0) {
				yield break;
			}

			yield return LoadAsyncIconAssets ();

			UpdateIconImage (image, nativeSize);
			UnloadIconAssets ();
		}

		private void UpdateIconImage (Image image, bool nativeSize = false)
		{
			var spriteHolder = spriteHolders [0];
			image.sprite = spriteHolder.AssetEntity;
			if (spriteHolder.materialHolder != null) {
				image.material = spriteHolder.materialHolder.AssetEntity;
				image.SetMaterialDirty ();
			}

			if (nativeSize) {
				image.SetNativeSize ();
			}
		}

		private bool m_IconAssetsIsLoaded;

		private void LoadIconAssets ()
		{
			if (m_IconAssetsIsLoaded) {
				return;
			}

			LoadSubHolderAssets (spriteHolders);
			m_IconAssetsIsLoaded = true;
		}

		private IEnumerator LoadAsyncIconAssets ()
		{
			if (m_IconAssetsIsLoaded) {
				yield break;
			}

			yield return LoadAsyncSubHolderAssets (spriteHolders);
			m_IconAssetsIsLoaded = true;
		}

		private void UnloadIconAssets ()
		{
			UnloadSubHolderAssets (spriteHolders);
			m_IconAssetsIsLoaded = false;
		}

		#endregion
	}


	public class CocoDressCategoryHolder : CocoAssetHolderBase
	{
		public bool isMain = false;
		public List<CocoDressItemHolder> itemHolders = new List<CocoDressItemHolder> ();


		#region implemented abstract members of CocoHolderBase

		protected override void LoadProcess ()
		{
			LoadSubHolderAssets (itemHolders);
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			yield return LoadAsyncSubHolderAssets (itemHolders);
		}

		protected override void UnloadProcess ()
		{
			UnloadSubHolderAssets (itemHolders);
		}

		#endregion


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (itemHolders);
			InitItemHolderDic ();
		}

		private Dictionary<string, CocoDressItemHolder> m_ItemHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoDressItemHolder> ItemHolderDic {
			get { return m_ItemHolderDic; }
		}

		private void InitItemHolderDic ()
		{
			m_ItemHolderDic = new Dictionary<string, CocoDressItemHolder> (itemHolders.Count);

			itemHolders.ForEach (holder => { m_ItemHolderDic.Add (holder.id, holder); });
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("itemHolders", itemHolders, subIndentations);
			str += indentations;
			return str;
		}

		#endregion
	}


	public class CocoDressSceneHolder : CocoHolderBase
	{
		public List<CocoDressSceneCategoryHolder> categoryHolders = new List<CocoDressSceneCategoryHolder> ();

		private Dictionary<string, CocoDressSceneCategoryHolder> m_CategoryHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoDressSceneCategoryHolder> CategoryHolderDic {
			get { return m_CategoryHolderDic; }
		}


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (categoryHolders);
			InitCategoryItemIdSet ();
		}

		private void InitCategoryItemIdSet ()
		{
			m_CategoryHolderDic = CocoData.CreateDictionary (categoryHolders, categoryHolder => categoryHolder.id);
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("categoryItemIds", categoryHolders, subIndentations);
			str += indentations;
			return str;
		}

		#endregion
	}


	public class CocoDressSceneCategoryHolder : CocoHolderBase
	{
		public List<CocoDressSceneItemHolder> itemHolders = new List<CocoDressSceneItemHolder> ();

		private Dictionary<string, CocoDressSceneItemHolder> m_ItemHolderDic;

		[JsonIgnore]
		public Dictionary<string, CocoDressSceneItemHolder> ItemHolderDic {
			get { return m_ItemHolderDic; }
		}


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (itemHolders);
			InitItemDic ();
		}

		private void InitItemDic ()
		{
			m_ItemHolderDic = CocoData.CreateDictionary (itemHolders, itemHolder => itemHolder.id);

			var configHolder = ParentHolder.ParentHolder as CocoAssetConfigHolder;
			if (configHolder == null) {
				return;
			}

			var itemHolderDic = configHolder.ItemHolderDic;

			foreach (var sceneItemHolder in itemHolders) {
				var dressItemHolder = itemHolderDic.GetValue (sceneItemHolder.id);
				if (dressItemHolder == null) {
					continue;
				}

				sceneItemHolder.LinkedDressItemHolder = dressItemHolder;
				dressItemHolder.LinkedSceneItemHolder = sceneItemHolder;
			}
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("itemHolders", itemHolders, subIndentations);
			str += indentations;
			return str;
		}

		#endregion
	}


	public class CocoDressSceneItemHolder : CocoHolderBase
	{
		[JsonIgnore]
		public CocoDressItemHolder LinkedDressItemHolder { get; set; }
	}
}