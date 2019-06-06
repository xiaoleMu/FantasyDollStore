using UnityEngine;
using UnityEditor;
using System.Linq;
using CocoPlay;


public class GamePostprocessor : AssetPostprocessor
{
	private const string ASSET_PATH_GAME = "Assets/_Game/";
	private const string ASSET_PATH_CHARACTER_ANIMATOR = "Models/Character/Animator";


	private void OnPostprocessTexture (Texture2D texture)
	{
		if (!assetImporter.assetPath.StartsWith (ASSET_PATH_GAME))
			return;

		TextureImporter textureImporter = (TextureImporter)assetImporter;
		if (textureImporter.assetPath.Contains ("cubemap"))
			return;

		if (IsNoMipmapAssetPath (assetImporter.assetPath)) {
			// for all textures, disable mipmap
			textureImporter.mipmapEnabled = false;
		}
	}

	private void OnPostprocessModel (GameObject model)
	{
		if (!assetImporter.assetPath.StartsWith (ASSET_PATH_GAME))
			return;

		ModelImporter modelImporter = (ModelImporter)assetImporter;
		if (modelImporter.assetPath.Contains (ASSET_PATH_CHARACTER_ANIMATOR)) {
			modelImporter.importMaterials = false;
		}
	}

	private static bool IsNoMipmapAssetPath (string assetPath)
	{
		var paths = CocoDebugSettingsData.Instance.NoMipmapExcludedAssetPaths;
		if (paths == null) {
			return true;
		}

		return paths.All (path => !assetPath.Contains (path));
	}
}