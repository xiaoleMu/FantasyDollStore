using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace TabTale.Plugins.PSDK {
	public class PsdkBannersDebug : PsdkSingleton<PsdkBannersDebug>, IPsdkBanners {

		float _AdHeight = 60F;
		bool _active = false;

		public bool Setup() {
			return true;
		}

		public bool Show() {
			_active = true;
			PsdkEventSystem.Instance.SendMessage ("onBannerShown");
			return true;
		}

		public void Hide() {
			_active = false;
			PsdkEventSystem.Instance.SendMessage ("onBannerHidden");
		}
		public float GetAdHeight() {
			return _AdHeight;
		}

		public bool IsBlockingViewNeeded() {
			return true;
		}
		
		public bool IsActive() {
			return _active;
		}
		
		public bool IsAlignedToTop() {
			return false;
		}

		public void psdkStartedEvent() {
		}
		
		public IPsdkBanners GetImplementation() {
			return this;
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

			if (! _active)
				return;

			InitStyles();

			float x = 0f;
			float y = 0f;
			float adHeight = GetAdHeight ();

			if (! IsAlignedToTop ())
				y = Screen.height - adHeight;

			GUI.Box(new Rect(x,y,Screen.width,adHeight), " Banner !",currentStyle);
		}

		#if UNITY_EDITOR
		[MenuItem("TabTale/PSDK/DebugPlay/Banners Show")]
		static void showBanners(){
			PsdkBannersDebug.Instance.Show ();
		}
		
		[MenuItem("TabTale/PSDK/DebugPlay/Banners Hide")]
		static void hideBanners() {
			PsdkBannersDebug.Instance.Hide ();
		}
		#endif

	}
}
