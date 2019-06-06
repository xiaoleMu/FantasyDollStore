using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if COCO_FAKE
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
#else
using CocoLanguage = Game.CocoLanguage;
#endif


namespace CocoPlay
{
	[System.Serializable]
	public class CocoOptionalLanguage : CocoOptionalProperty<CocoLanguage>
	{
		public CocoOptionalLanguage () : base ()
		{
		}

		public CocoOptionalLanguage (CocoLanguage value) : base (value)
		{
		}
	}

}