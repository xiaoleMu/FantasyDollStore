using UnityEngine;
using System.Collections;
using TabTale;

namespace CocoPlay.Fake
{
	#if COCO_FAKE
	public class CocoFakeStartModule : CocoStartModule
	{
		protected override void InitDatas ()
		{
			base.InitDatas ();

			BindType<ICocoAudioData, CocoFakeAudioData> ();
			Bind <CocoGlobalData> ();
		}

		protected override void CleanDatas ()
		{
			Unbind<ICocoAudioData> ();
			Unbind <CocoGlobalData> ();

			base.CleanDatas ();
		}

		protected override void InitSceneModuleDatas ()
		{
			AddSceneModuleData (new CocoSceneModuleData (CocoSceneID.CoverPage, "Test"));
		}
	}
	#endif
}
