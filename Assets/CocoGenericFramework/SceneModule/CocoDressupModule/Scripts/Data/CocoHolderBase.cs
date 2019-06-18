using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CocoPlay.ResourceManagement;
using Game;
using LitJson;


namespace CocoPlay
{
	public abstract class CocoHolderBase
	{
		public string id = string.Empty;
		public int order = -1;


		#region Link

		private CocoHolderBase m_ParentHolder;

		[JsonIgnore]
		public CocoHolderBase ParentHolder {
			get { return m_ParentHolder; }
		}

		public virtual void LinkParent (CocoHolderBase parentHolder)
		{
			m_ParentHolder = parentHolder;
		}

		protected void LinkSubHolders<T> (List<T> subHolders) where T : CocoHolderBase
		{
			subHolders.ForEach (holder => holder.LinkParent (this));
		}

		protected void LinkSubHolders<T> (Dictionary<string, T> subHolders) where T : CocoHolderBase
		{
			subHolders.ForEach (holder => holder.LinkParent (this));
		}

		#endregion


		#region Print

		public string Print (string indentations = "")
		{
			return string.Format ("{0}[{1}: {2}]\n", indentations, GetType ().Name, PrintContent (indentations));
		}

		protected virtual string PrintContent (string indentations)
		{
			return string.Format ("id={0} order={1} ", id, order);
		}

		protected string PrintSubHolders<T> (string name, List<T> subHolders, string indentations) where T : CocoHolderBase
		{
			if (subHolders.Count <= 0) {
				return string.Empty;
			}

			var str = string.Format ("{0}[{1}<List({2})>], count=[{3}]: {{\n", indentations, name, (typeof(T)).Name, subHolders.Count);
			var subIndentations = string.Format ("{0}\t", indentations);
			foreach (var subHolder in subHolders) {
				str += subHolder.Print (subIndentations);
			}
			str += string.Format ("{0}}}\n", indentations);
			return str;
		}

		protected string PrintSubHolders<T> (string name, Dictionary<string, T> subHolders, string indentations) where T : CocoHolderBase
		{
			if (subHolders.Count <= 0) {
				return string.Empty;
			}

			var str = string.Format ("{0}[{1}<Dictionary(string, {2})>], count=[{3}]: {{\n", indentations, name, (typeof(T)).Name, subHolders.Count);
			var subIndentations = string.Format ("{0}\t", indentations);
			foreach (var kvpSubHolder in subHolders) {
				str += string.Format ("{0}{1}:\n{2}\n", subIndentations, kvpSubHolder.Key, kvpSubHolder.Value.Print (subIndentations));
			}
			str += string.Format ("{0}}}\n", indentations);
			return str;
		}

		protected string PrintSubValues<T> (string name, List<T> subValues, string indentations)
		{
			if (subValues.Count <= 0) {
				return string.Empty;
			}

			var str = string.Format ("{0}[{1}<List({2})>], count=[{3}]: {{\n", indentations, name, (typeof(T)).Name, subValues.Count);
			var subIndentations = string.Format ("{0}\t", indentations);
			foreach (var subValue in subValues) {
				str += string.Format ("{0}{1}\n", subIndentations, subValue.ToString ());
			}
			str += string.Format ("{0}}}\n", indentations);
			return str;
		}

		protected string PrintSubValues<T> (string name, Dictionary<string, T> subValues, string indentations)
		{
			if (subValues.Count <= 0) {
				return string.Empty;
			}

			var str = string.Format ("{0}[{1}<Dictionary(string, {2})>], count=[{3}]: {{\n", indentations, name, (typeof(T)).Name, subValues.Count);
			var subIndentations = string.Format ("{0}\t", indentations);
			foreach (var kvpSubValue in subValues) {
				str += string.Format ("{0}{1}: {2}\n", subIndentations, kvpSubValue.Key, kvpSubValue.Value.ToString ());
			}
			str += string.Format ("{0}}}\n", indentations);
			return str;
		}

		#endregion
	}


	public abstract class CocoAssetHolderBase : CocoHolderBase
	{
		public string assetPath = string.Empty;

		public string coverLayerId = string.Empty;


		#region Load/Unload

		private bool m_AssetIsLoaded;

		[JsonIgnore]
		public bool AssetIsLoaded {
			get { return m_AssetIsLoaded; }
		}

		public void LoadAsset ()
		{
			if (m_AssetIsLoaded) {
				return;
			}

			LoadProcess ();
			m_AssetIsLoaded = true;
		}

		public IEnumerator LoadAssetAsync (Action endAction = null)
		{
			if (!m_AssetIsLoaded) {
				yield return LoadAsyncProcess ();
				m_AssetIsLoaded = true;
			}

			if (endAction != null) {
				endAction ();
			}
		}

		protected abstract void LoadProcess ();

		protected abstract IEnumerator LoadAsyncProcess ();

		public void UnloadAsset ()
		{
			UnloadProcess ();
			m_AssetIsLoaded = false;
		}

		protected abstract void UnloadProcess ();

		#endregion


		#region Sub Load/Unload

		protected void LoadSubHolderAssets<T> (List<T> subHolders) where T : CocoAssetHolderBase
		{
			subHolders.ForEach (holder => holder.LoadAsset ());
		}

		protected void LoadSubHolderAssets<T> (Dictionary<string, T> subHolders) where T : CocoAssetHolderBase
		{
			subHolders.ForEach (holder => holder.LoadAsset ());
		}

		protected IEnumerator LoadAsyncSubHolderAssets<T> (List<T> subHolders) where T : CocoAssetHolderBase
		{
			var asyncHelper = ResourceManager.AsyncHelper;

			var loadingCount = subHolders.Count;
			foreach (var holder in subHolders) {
				asyncHelper.StartCoroutine (holder.LoadAssetAsync (() => loadingCount--));
			}

			while (loadingCount > 0) {
				yield return new WaitForEndOfFrame ();
			}
		}

		protected IEnumerator LoadAsyncSubHolderAssets<T> (Dictionary<string, T> subHolders) where T : CocoAssetHolderBase
		{
			var asyncHelper = ResourceManager.AsyncHelper;

			var loadingCount = subHolders.Count;
			foreach (var holder in subHolders.Values) {
				asyncHelper.StartCoroutine (holder.LoadAssetAsync (() => loadingCount--));
			}

			while (loadingCount > 0) {
				yield return new WaitForEndOfFrame ();
			}
		}

		protected void UnloadSubHolderAssets<T> (List<T> subHolders) where T : CocoAssetHolderBase
		{
			foreach (var holder in subHolders) {
				holder.UnloadAsset ();
			}
		}

		protected void UnloadSubHolderAssets<T> (Dictionary<string, T> subHolders) where T : CocoAssetHolderBase
		{
			foreach (var holder in subHolders.Values) {
				holder.UnloadAsset ();
			}
		}

		#endregion


		#region Link

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);

			if (!string.IsNullOrEmpty (coverLayerId)) {
				var phased = coverLayerId.ToEnum (out m_CoverLayer);
				if (!phased) {
					Debug.LogErrorFormat ("{0}->LinkParent: phase cover layer id [{0}] for [{1}] failed !", coverLayerId, id);
				}
			}
		}

		private CocoDressCoverLayer m_CoverLayer = CocoDressCoverLayer.None;

		[JsonIgnore]
		public virtual CocoDressCoverLayer CoverLayer {
			get { return m_CoverLayer; }
		}

		public bool IsCoverLayerConflicted (CocoAssetHolderBase otherHolder)
		{
			if (otherHolder == null) {
				return false;
			}

			return (CoverLayer & otherHolder.CoverLayer) != 0;
		}

		public bool IsCoverLayerConflicted (CocoDressCoverLayer otherCoverLayer)
		{
			return (CoverLayer & otherCoverLayer) != 0;
		}

		#endregion


		#region Print

		protected override string PrintContent (string indentations)
		{
			return string.Format ("{0}assetPath={1}, AssetIsLoaded={2}", base.PrintContent (indentations), assetPath, AssetIsLoaded);
		}

		#endregion
	}
}