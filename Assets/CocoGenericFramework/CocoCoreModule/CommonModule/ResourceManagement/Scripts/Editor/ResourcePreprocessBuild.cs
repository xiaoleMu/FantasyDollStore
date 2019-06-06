using UnityEditor;
using UnityEditor.Build;

namespace CocoPlay.ResourceManagement
{
	public class ResourcePreprocessBuild : IPreprocessBuild
	{
		public int callbackOrder {
			get { return 0; }
		}

		public void OnPreprocessBuild (BuildTarget target, string path)
		{
			switch (target) {
			case BuildTarget.iOS:
				CopyResourceToStreamingAssets (ResourceSettings.PLATFORM_DIRECTORY_IOS);
				break;
			case BuildTarget.Android:
				CopyResourceToStreamingAssets (ResourceSettings.PLATFORM_DIRECTORY_GP);
				break;
			}
		}

		private void CopyResourceToStreamingAssets (string targetPlatform)
		{
			var configDataConfigurator = new ConfigDataConfigurator ();
			var assetBundleConfigurator = new AssetBundleConfigurator ();

			var copyOptions = new ResourceEditorTargetCopyOptions {
				ClearFolders = true,
				ClearOtherPlatforms = true
			};
			ResourceEditorWindow.CopyToStreamingAssets (targetPlatform, copyOptions, configDataConfigurator, assetBundleConfigurator);
		}
	}
}