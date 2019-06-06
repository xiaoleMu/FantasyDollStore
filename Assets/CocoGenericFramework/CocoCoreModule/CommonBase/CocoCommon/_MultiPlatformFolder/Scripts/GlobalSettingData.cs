using System;
using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay
{
	public class GlobalSettingData : ScriptableObject
	{
		private static GlobalSettingData _instance;

		public static GlobalSettingData Instance {
			get {
				if (_instance == null) {
					_instance = Resources.Load<ScriptableObject> ("GlobalSettingData") as GlobalSettingData;
				}
				return _instance;
			}
		}

		public bool isDebugMode;

		public List<MultiPlatformItem> allPlatformItems = new List<MultiPlatformItem> {
			new MultiPlatformItem (MultiPlatformType.iOS),
			new MultiPlatformItem (MultiPlatformType.GooglePlay),
			new MultiPlatformItem (MultiPlatformType.Amazon),
		};
	}


	[Serializable]
	public class MultiPlatformItem
	{
		//在面板上是否折叠
		public bool foldout = true;

		public MultiPlatformType curPlatform = MultiPlatformType.iOS;
		public bool isSelected;

		public string bundleId = "";

		public string bundleVersion = "0.0.1";

		public string iconFolder = "";

		public string scriptingDefs = "";

		public bool splitBinary;

		public Orientation allowOrientations;


		#region iOS 特有

		#endregion


		#region Android 特有

		public int bundleVersionCode = 1;

		public string keystoreName = "AndroidKeyStore/tabtale.keystore";
		public string keystorePass = "storybook";
		public string keyaliasName = "tabtale";
		public string keyaliasPass = "storybook";

		#endregion


		public MultiPlatformItem ()
		{
		}

		public MultiPlatformItem (MultiPlatformType pPlatform)
		{
			curPlatform = pPlatform;

			switch (curPlatform) {
			case MultiPlatformType.iOS:
				iconFolder = "_Game/CocoPlay/GameCommon/PublishIcons/iOS";
				scriptingDefs = "NO_RECEIPT_VALIDATION;NO_GPGS";
				allowOrientations = Orientation.LandscapeRight | Orientation.LandscapeLeft;
				break;
			case MultiPlatformType.GooglePlay:
				iconFolder = "_Game/CocoPlay/GameCommon/PublishIcons/GP";
				scriptingDefs = "NO_RECEIPT_VALIDATION";
				allowOrientations = Orientation.LandscapeRight | Orientation.LandscapeLeft;
				break;
			case MultiPlatformType.Amazon:
				iconFolder = "_Game/CocoPlay/GameCommon/PublishIcons/AM";
				scriptingDefs = "AMAZON";
				allowOrientations = Orientation.LandscapeLeft;
				break;
			}
		}


		[Flags]
		public enum Orientation
		{
			// single
			Portrait = 0x01,
			PortraitUpsideDown = 0x02,
			LandscapeRight = 0x04,
			LandscapeLeft = 0x08,
		}
	}


	public enum MultiPlatformType
	{
		iOS = 0,
		GooglePlay = 1,
		Amazon = 2,
	}
}