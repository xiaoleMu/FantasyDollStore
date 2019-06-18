using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoPlay
{
	public class CocoDressAssetBundleConfigurator
	{
		private string _rootDirectory;

		private string GetAssetPath (string path)
		{
			return Path.Combine (_rootDirectory, path);
		}


		#region Set Tag

		public void AutoSetAssetBundleTags (CocoAssetConfigHolder assetConfigHolder, string rootDirectory)
		{
			if (assetConfigHolder == null) {
				return;
			}

			_rootDirectory = rootDirectory;

			InitContentTags ();
			SetAssetTags (assetConfigHolder);
		}

		private void SetAssetTags (CocoAssetConfigHolder assetConfigHolder)
		{
			SetConfigAssetTags (assetConfigHolder);
			SetSceneAssetTags (assetConfigHolder);
		}

		private void SetSceneAssetTags (CocoAssetConfigHolder assetConfigHolder)
		{
			foreach (var sceneHolder in assetConfigHolder.SceneHolderDic.Values) {
				var tag = CocoDressSettings.ASSET_BUNDLE_TAG_PREFIX + sceneHolder.id;

				foreach (var categoryHolder in sceneHolder.categoryHolders) {
					foreach (var itemHolder in categoryHolder.itemHolders) {
						SetItemTag (itemHolder.LinkedDressItemHolder, tag);
					}
				}
			}
		}

		private void SetConfigAssetTags (CocoAssetConfigHolder assetConfigHolder)
		{
			SetConfigTags (assetConfigHolder.categoryConfigHolders);
			SetConfigTags (assetConfigHolder.sceneConfigHolders);
			SetConfigTags (assetConfigHolder.roleDressConfigHolders);
			SetConfigTags (assetConfigHolder.roleBodyConfigHolders);
			SetConfigTags (assetConfigHolder.roleConfigHolders);

			SetConfigTag (assetConfigHolder);
		}

		private void SetConfigTags<T> (IEnumerable<T> configHolders) where T : CocoAssetHolderBase
		{
			foreach (var configHolder in configHolders) {
				SetConfigTag (configHolder);
			}
		}

		private void SetConfigTag (CocoAssetHolderBase configHolder)
		{
			var assetPath = GetAssetPath (configHolder.assetPath);
			SetAssetTag (assetPath, CocoDressSettings.ASSET_BUNDLE_TAG_CONFIG, configHolder.id);
		}

		private void SetItemTag (CocoDressItemHolder itemHolder, string tag)
		{
			// model
			foreach (var modelHolder in itemHolder.modelHolders) {
				var modelPath = GetAssetPath (modelHolder.assetPath);
				SetAssetTag (modelPath, tag, modelHolder.id);

				// material
				foreach (var materialHolder in modelHolder.materialHolders) {
					var materialPath = GetAssetPath (materialHolder.assetPath);
					if (SetAssetTag (materialPath, tag, materialHolder.id)) {
						SetMaterialContentTag (materialPath, tag);
					}
				}
			}

			// icon
			foreach (var spriteHolder in itemHolder.spriteHolders) {
				var iconPath = GetAssetPath (spriteHolder.assetPath);
				SetAssetTag (iconPath, tag, spriteHolder.id);

				// icon material
				if (spriteHolder.materialHolder == null) {
					continue;
				}

				var materialPath = GetAssetPath (spriteHolder.materialHolder.assetPath);
				if (SetAssetTag (materialPath, tag, spriteHolder.materialHolder.id)) {
					SetMaterialContentTag (materialPath, tag);
				}
			}
		}

		#endregion


		#region Tag

		private readonly Dictionary<string, string> _contentTagDic = new Dictionary<string, string> ();

		private void InitContentTags ()
		{
			_contentTagDic.Clear ();
		}

		private bool SetAssetTag (string assetPath, string tag, string assetId)
		{
			if (string.IsNullOrEmpty (assetPath) || assetPath == _rootDirectory) {
				return false;
			}

			if (assetPath == "Resources/unity_builtin_extra" || assetPath == "Library/unity default resources") {
				Debug.LogFormat ("{0}->SetAssetTag: [{1}] is build-in asset in [{2}]", GetType ().Name, assetId, assetPath);
				return false;
			}

			var assetImporter = AssetImporter.GetAtPath (assetPath);
			if (assetImporter == null) {
				Debug.LogErrorFormat ("{0}->SetAssetTag: [{1}] NOT be found at path [{2}]", GetType ().Name, assetId, assetPath);
				return false;
			}

			assetImporter.SetAssetBundleNameAndVariant (tag, string.Empty);
			return true;
		}

		private void SetMaterialContentTag (string materialPath, string tag)
		{
			if (string.IsNullOrEmpty (materialPath)) {
				return;
			}

			var material = AssetDatabase.LoadAssetAtPath<Material> (materialPath);
			if (material == null) {
				return;
			}

			// shader
			var shader = material.shader;
			if (shader == null) {
				return;
			}

			SetMaterialContentTag (shader, tag, material.name);

			// property
			var propertyCount = ShaderUtil.GetPropertyCount (shader);
			for (var i = 0; i < propertyCount; i++) {
				if (ShaderUtil.GetPropertyType (shader, i) != ShaderUtil.ShaderPropertyType.TexEnv) {
					continue;
				}

				var propertyName = ShaderUtil.GetPropertyName (shader, i);
				var texture = material.GetTexture (propertyName);
				if (texture == null) {
					continue;
				}

				SetMaterialContentTag (texture, tag, material.name);
			}
		}

		private void SetMaterialContentTag (Object asset, string tag, string materialId)
		{
			var assetPath = AssetDatabase.GetAssetPath (asset);
			var assetId = string.Format ("{0}({1})", asset.name, materialId);

			// new content
			if (!_contentTagDic.ContainsKey (assetPath)) {
				// set asset bundle name to empty because it will be included automatically
				if (SetAssetTag (assetPath, string.Empty, assetId)) {
					_contentTagDic.Add (assetPath, tag);
				}
				return;
			}

			var originalTag = _contentTagDic [assetPath];

			// same bundle
			if (originalTag == tag) {
				return;
			}

			// already in share
			if (originalTag == CocoDressSettings.ASSET_BUNDLE_TAG_SHARE) {
				return;
			}

			// set to share bundle
			SetAssetTag (assetPath, CocoDressSettings.ASSET_BUNDLE_TAG_SHARE, assetId);
			_contentTagDic [assetPath] = CocoDressSettings.ASSET_BUNDLE_TAG_SHARE;
			Debug.LogWarningFormat (asset, "[{0}] is shared asset in path {1}", asset.name, assetPath);
		}

		#endregion
	}
}