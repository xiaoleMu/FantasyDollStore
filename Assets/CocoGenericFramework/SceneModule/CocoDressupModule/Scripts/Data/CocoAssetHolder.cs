using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CocoPlay.ResourceManagement;
using LitJson;


namespace CocoPlay
{
	/// <summary>
	/// Asset Holder (Base)
	/// </summary>
	public class CocoAssetHolder<T> : CocoAssetHolderBase where T : Object
	{
		#region Property

		private T m_AssetEntity;

		[JsonIgnore]
		public T AssetEntity {
			get { return m_AssetEntity; }
			protected set { m_AssetEntity = value; }
		}

		private string AssetPathWithoutExtension {
			get { return assetPath.Substring (0, assetPath.Length - Path.GetExtension (assetPath).Length); }
		}

		#endregion


		#region Load/Unload

		protected override void LoadProcess ()
		{
			AssetEntity = ResourceManager.Load<T> (AssetPathWithoutExtension);

			if (AssetEntity == null) {
				Debug.LogErrorFormat ("{0}->LoadProcess: asset load failed [{1}].", GetType ().Name, assetPath);
			}
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			var loadAsync = ResourceManager.LoadAsync<T> (AssetPathWithoutExtension);
			yield return loadAsync;
			if (loadAsync.Asset != null) {
				AssetEntity = loadAsync.Asset as T;
			}

			if (AssetEntity == null) {
				Debug.LogErrorFormat ("{0}->LoadAsyncProcess: asset load failed [{1}].", GetType ().Name, assetPath);
			}
		}

		protected override void UnloadProcess ()
		{
			AssetEntity = null;
		}

		#endregion
	}


	/// <summary>
	/// Asset Holder (Texture2D).
	/// </summary>
	public class CocoAssetTextureHolder : CocoAssetHolder<Texture2D>
	{
	}


	/// <summary>
	/// Asset Holder (Material).
	/// </summary>
	public class CocoAssetMaterialHolder : CocoAssetHolder<Material>
	{
	}


	/// <summary>
	/// Asset Holder (Model).
	/// </summary>
	public class CocoAssetModelHolder : CocoAssetHolder<GameObject>
	{
		public List<CocoAssetMaterialHolder> materialHolders = new List<CocoAssetMaterialHolder> ();
		public List<string> targetSmrs = new List<string> ();

		protected override void LoadProcess ()
		{
			base.LoadProcess ();
			LoadSubHolderAssets (materialHolders);
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			yield return base.LoadAsyncProcess ();
			yield return LoadAsyncSubHolderAssets (materialHolders);
		}

		protected override void UnloadProcess ()
		{
			UnloadSubHolderAssets (materialHolders);
			base.UnloadProcess ();
		}

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			LinkSubHolders (materialHolders);
			InitTargetSmrSet ();
		}

		private HashSet<string> m_TargetSmrSet;

		[JsonIgnore]
		public HashSet<string> TargetSmrSet {
			get { return m_TargetSmrSet; }
		}

		private void InitTargetSmrSet ()
		{
			m_TargetSmrSet = new HashSet<string> (targetSmrs);
		}


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			var subIndentations = string.Format ("{0}\t", indentations);
			str += PrintSubHolders ("materialHolders", materialHolders, subIndentations);
			str += PrintSubValues ("targetSmrs", targetSmrs, subIndentations);
			str += indentations;
			return str;
		}

		#endregion
	}


	/// <summary>
	/// Asset Holder (Sprite).
	/// </summary>
	public class CocoAssetSpriteHolder : CocoAssetHolder<Sprite>
	{
		public CocoAssetMaterialHolder materialHolder = null;

		protected override void LoadProcess ()
		{
			base.LoadProcess ();
			if (materialHolder != null) {
				materialHolder.LoadAsset ();
			}
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			yield return base.LoadAsyncProcess ();
			if (materialHolder != null) {
				yield return materialHolder.LoadAssetAsync ();
			}
		}

		protected override void UnloadProcess ()
		{
			if (materialHolder != null) {
				materialHolder.UnloadAsset ();
			}
			base.UnloadProcess ();
		}

		public override void LinkParent (CocoHolderBase parentHolder)
		{
			base.LinkParent (parentHolder);
			if (materialHolder != null) {
				materialHolder.LinkParent (this);
			}
		}


		#region Print

		protected override string PrintContent (string indentations)
		{
			var str = base.PrintContent (indentations) + '\n';
			if (materialHolder != null) {
				var subIndentations = string.Format ("{0}\t", indentations);
				str += materialHolder.Print (subIndentations);
			}
			str += indentations;
			return str;
		}

		#endregion
	}


	/// <summary>
	/// Asset Holder (Text).
	/// </summary>
	public class CocoAssetTextHolder : CocoAssetHolder<TextAsset>
	{
	}


	/// <summary>
	/// Asset Holder (Json Object).
	/// </summary>
	public class CocoAssetJsonHolder<T> : CocoAssetHolderBase
	{
		#region Property

		private T m_AssetEntity;

		[JsonIgnore]
		public T AssetEntity {
			get { return m_AssetEntity; }
			protected set { m_AssetEntity = value; }
		}

		private string AssetPathWithoutExtension {
			get { return assetPath.Substring (0, assetPath.Length - Path.GetExtension (assetPath).Length); }
		}

		#endregion


		#region Load/Unload

		protected override void LoadProcess ()
		{
			var data = ResourceManager.Load<TextAsset> (AssetPathWithoutExtension);
			if (data != null) {
				AssetEntity = JsonMapper.ToObject<T> (data.text);
			}

			if (AssetEntity == null) {
				Debug.LogErrorFormat ("{0}->LoadProcess: asset load failed [{1}].", GetType ().Name, assetPath);
			}
		}

		protected override IEnumerator LoadAsyncProcess ()
		{
			var loadAsync = ResourceManager.LoadAsync<TextAsset> (AssetPathWithoutExtension);
			yield return loadAsync;
			if (loadAsync.Asset != null) {
				var data = loadAsync.Asset as TextAsset;
				if (data != null) {
					AssetEntity = JsonMapper.ToObject<T> (data.text);
				}
			}

			if (AssetEntity == null) {
				Debug.LogErrorFormat ("{0}->LoadAsyncProcess: asset load failed [{1}].", GetType ().Name, assetPath);
			}
		}

		protected override void UnloadProcess ()
		{
			AssetEntity = default(T);
		}

		#endregion


		#region Load/Save (File)

		public void LoadFromFile (string rootDirectory)
		{
			if (m_AssetEntity != null) {
				return;
			}

			var path = Path.Combine (rootDirectory, assetPath);
			if (File.Exists (path)) {
				var json = File.ReadAllText (path);
				m_AssetEntity = JsonMapper.ToObject<T> (json);
			}

			if (m_AssetEntity == null) {
				Debug.LogErrorFormat ("[{0}]->LoadFromFile: asset [{1}] load failed!", GetType ().Name, assetPath);
			}
		}

		public void SaveToFile (string rootDirectory, bool prettyPrint)
		{
			if (m_AssetEntity == null) {
				Debug.LogWarningFormat ("[{0}]->SaveToFile: asset [{1}] NOT loaded!", GetType ().Name, assetPath);
				return;
			}


			var path = Path.Combine (rootDirectory, assetPath);

			CocoData.SaveToJsonFile (m_AssetEntity, path, prettyPrint);
		}

		#endregion
	}
}