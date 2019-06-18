using System;
using System.Collections;
using CocoPlay;
using UnityEngine;

namespace Game
{
	public partial class GameAssetHelperSample
	{
		#region Dress

		public bool IsDressConfigLoaded { get; private set; }

		public IEnumerator LoadDressConfig ()
		{
			if (IsDressConfigLoaded) {
				yield break;
			}

			var assetControl = CocoRoot.GetInstance<CocoAssetControl> ();
			if (assetControl == null) {
				yield break;
			}

			var loadSuccess = false;
			var loading = true;
			Action<bool> finishAction = result => {
				loadSuccess = result;
				loading = false;
			};
			assetControl.dressLoadConfigHolderFinishSignal.AddListener (finishAction);

			assetControl.dressLoadConfigHolderSignal.Dispatch (string.Empty);
			yield return new WaitWhile (() => loading);

			assetControl.dressLoadConfigHolderFinishSignal.RemoveListener (finishAction);
			IsDressConfigLoaded = loadSuccess;
		}

		#endregion
	}
}