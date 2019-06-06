using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TabTale;
using System.IO;
using CocoPlay.Localization;
#if UNITY_2017_2_OR_NEWER
using Prime31;
#endif

namespace CocoPlay{
	public class SaveImageSignal : Signal<string, System.Action<bool>> {}

	public class SaveImageCommand : Command 
	{
		[Inject]
		public string ImagePath { get; set;}

		[Inject]
		public System.Action<bool> endAction { get; set; }

		private enum ProcessResult
		{
			None = 0,
			ProcessSucceed = 1,
			ProcessFailed = 2,
			ProcessCancelled = 3
		}
		ProcessResult mProcessResult;
		string mProcessResultInfo = null;

		public override void Execute ()
		{
			Retain ();
			CocoMainController.Instance.StartCoroutine (PhotoAlbumSaveImage ());
		}

		public IEnumerator PhotoAlbumSaveImage ()
		{
			//StartActivityIndicator (Localization.Get ("txt_loading_info_processing"));
			mProcessResult = ProcessResult.None;
			mProcessResultInfo = "";

			if (Application.platform == RuntimePlatform.OSXEditor) {
				yield return new WaitForSeconds (1.0f);
				mProcessResult = ProcessResult.ProcessSucceed;
			}
			else {
				#if UNITY_IPHONE
				EtceteraManager.saveImageToPhotoAlbumSucceededEvent += PhotoAlbumSaveImageSucceed;
				EtceteraManager.saveImageToPhotoAlbumFailedEvent += PhotoAlbumSaveImageFailed;

				EtceteraBinding.saveImageToPhotoAlbum (ImagePath);

				if (Application.platform == RuntimePlatform.IPhonePlayer) {
				while (mProcessResult == ProcessResult.None)
				yield return new WaitForEndOfFrame ();
				} else {
				yield return new WaitForSeconds (1.0f);
				mProcessResult = ProcessResult.ProcessSucceed;
				}

				EtceteraManager.saveImageToPhotoAlbumSucceededEvent -= PhotoAlbumSaveImageSucceed;
				EtceteraManager.saveImageToPhotoAlbumFailedEvent -= PhotoAlbumSaveImageFailed;


				#elif UNITY_ANDROID
				CocoPluginManager pluginManager = CocoMainController.PluginManager;
				bool res = false;
				yield return pluginManager.StartCoroutine (pluginManager.RequestPermission (
					CocoPluginManager.PERMISSION_WRITE_EXTERNAL_STORAGE,
					GameLocalization.Get ("txt_permission_rationale_save_to_gallery"),
					(passed) => {
						if (passed) {
							res = passed;
						}
					}
				));
				if (res) {
					res = EtceteraAndroid.saveImageToGallery (ImagePath, Path.GetFileNameWithoutExtension (ImagePath));
				}
				mProcessResult = res ? ProcessResult.ProcessSucceed : ProcessResult.ProcessFailed;
				#endif
			}
			
			//StopActivityIndicator ();
			
			string msg = string.Empty;
			if (mProcessResult == ProcessResult.ProcessSucceed)
				msg = CocoLocalization.Get ("txt_prompt_info_share_photo_album_succeed");
			else
				msg = CocoLocalization.Get ("txt_prompt_info_share_photo_album_failed") + "\n" + mProcessResultInfo;

			ShowPromptMessage (msg, 1.5f);
			System.IO.File.Delete(ImagePath);
			Release();

			if (endAction != null) {
				endAction (mProcessResult == ProcessResult.ProcessSucceed);
			}
		}

		private void PhotoAlbumSaveImageSucceed ()
		{
			Debug.Log ("GamePluginManager->PhotoAlbumSaveImageSucceed. ");
			mProcessResult = ProcessResult.ProcessSucceed;
		}
		
		private void PhotoAlbumSaveImageFailed (string pError)
		{
			Debug.Log ("GamePluginManager->PhotoAlbumSaveImageFailed: " + pError);
			mProcessResult = ProcessResult.ProcessFailed;
			mProcessResultInfo = pError;
		}

		public void ShowPromptMessage (string pMsg, float pDuration)
		{
			#if UNITY_IPHONE
			CocoCommonBinding.ShowPromptMessage (pMsg, pDuration);
			#elif UNITY_ANDROID
			EtceteraAndroid.showToast (pMsg, true);
			#else
			Debug.Log ("GamePluginManager->ShowPromptMessage: " + pMsg + ", " + pDuration);
			#endif
		}
	}

}


