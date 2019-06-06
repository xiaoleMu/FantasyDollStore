using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay.Fake
{
	#if COCO_FAKE
	public class CocoFakeAudioData : CocoAudioBaseData
	{
		#region implemented abstract members of CocoAudioBaseData

		public override void InitAudioDatas ()
		{
			AddAudioData (CocoAudioID.Button_Normal, "general_button");
		}

		#endregion
	}
	#endif
}
