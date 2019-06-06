using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class PsdkLocationDebugButtons : PsdkSingleton<PsdkLocationDebugButtons> , IPsdkLocationManagerService{

		private bool _visible = false;
		private string _location;
		private bool _bannersWereActive = false;

		public bool Setup() {
			return true;
		}

		public void psdkStartedEvent() {
		}

		public void ReportLocation(string location){
		}
		
		public IPsdkLocationManagerService GetImplementation() {
			return this;
		}

		void UpdatePosition() {
			transform.position = Camera.main.transform.position;
		}

		public long Show(string location) {
			UpdatePosition ();
			_location = location;
			_visible = true;
			if (PSDKMgr.Instance.GetBannersService() != null) {
				_bannersWereActive = PSDKMgr.Instance.GetBannersService().IsActive();
				PSDKMgr.Instance.GetBannersService().Hide();
			}
			string jsonMessage = "{ \"location\":\"" + location + "\", \"attributes\": 2 }";
			PsdkEventSystem.Instance.transform.gameObject.SendMessage ("OnShown",jsonMessage);
			return LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC;
		}
		
		public void Hide() {
			_visible = false;
			if (_bannersWereActive && PSDKMgr.Instance.GetBannersService() != null)
					PSDKMgr.Instance.GetBannersService().Show();
			_bannersWereActive = false;
		}

		public bool IsViewVisible (){
			return _visible;
		}

		public bool IsLocationReady (string location){
			 return true;
		}

		public long LocationStatus(string location){
			return LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC;
		}

		void ScaleScreenForOnGUI() {
			float screenScale = Screen.width / 480.0f;
			GUI.matrix = Matrix4x4.Scale(new Vector3(screenScale,screenScale,screenScale));
		}

		private GUIStyle currentStyle = null;
		
		private void InitStyles()
		{
			if( currentStyle == null )
			{
				currentStyle = new GUIStyle( GUI.skin.box );
				currentStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 0.5f ) );
			}
		}
		
		private Texture2D MakeTex( int width, int height, Color col )
		{
			Color[] pix = new Color[width * height];
			for( int i = 0; i < pix.Length; ++i )
			{
				pix[ i ] = col;
			}
			Texture2D result = new Texture2D( width, height );
			result.SetPixels( pix );
			result.Apply();
			return result;
		}
		
		void OnGUI() {
			
			if (! _visible)
				return;
			
			InitStyles();
			
			float x = 50f;
			float y = 50f;

			if (GUI.Button (new Rect (x, y, Screen.width - 100, Screen.height - 100), _location, currentStyle)) {
				_visible = false;
				Close();
			}
		}


		public void Close() {
			string jsonMessage = "{ \"location\":\"" + _location + "\", \"attributes\": 2 }";
			PsdkEventSystem.Instance.transform.gameObject.SendMessage("OnClosed",jsonMessage);
		}

		public string GetLocations()
		{
			return "";
		}

		public void LevelOfFirstPopupStatus(bool enabled)
		{
			
		}

	}
}
