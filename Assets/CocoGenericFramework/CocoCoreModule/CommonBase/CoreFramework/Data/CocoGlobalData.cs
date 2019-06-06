using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using TabTale;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
#else
using CocoUIButtonID = Game.CocoUIButtonID;
using CocoAudioID = Game.CocoAudioID;
using CocoLanguage = Game.CocoLanguage;
using CocoSceneID = Game.CocoSceneID;
#endif

namespace CocoPlay
{
	public partial class CocoGlobalData
	{
		[Inject]
		public CocoGlobalRecordModel globalRecordModel {get; set;}

		public CocoGlobalData ()
		{
			Init ();
		}

		#region Common

	    public static CocoSceneID EnterSceneID;
		CocoSceneID curSceneID;
		CocoSceneID frontSceneID;

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			curSceneID = CocoSceneID.None;
			frontSceneID = CocoSceneID.None;
		}

		/// <summary>
		/// Gets or sets the current scene ID.
		/// </summary>
		/// <value>The current scene I.</value>
		public CocoSceneID CurSceneID {
			get{
				return curSceneID;
			}
			set{
				frontSceneID = curSceneID;
				curSceneID = value;
			}
		}

		/// <summary>
		/// Gets the front scene ID.
		/// </summary>
		/// <value>The front scene I.</value>
		public CocoSceneID FrontSceneID {
			get{
				return frontSceneID;
			}
		}

		CocoSceneID m_BackToSceneID = CocoSceneID.None;
		public CocoSceneID BackToSceneID {
			get {
				return m_BackToSceneID;
			}
			set {
				m_BackToSceneID = value;
			}
		}

		public virtual bool CanShowInterstitialBetweenScene (CocoSceneID prevSceneId, CocoSceneID nextSceneId)
		{
			if (prevSceneId == CocoSceneID.CoverPage) {
				return false;
			}

			return true;
		}

		public virtual void ClearRvRelease (){
			
		}

		#endregion

		public static bool m_GamePlayIsStarted = false;
//		#region CB Session
//
//		bool m_GamePlayIsStarted = false;
//
//		public bool GamePlayStarted {
//			get {
//				return m_GamePlayIsStarted;
//			}
//			set {
//				m_GamePlayIsStarted = value;
//			}
//		}
//
//		#endregion

	}
}

