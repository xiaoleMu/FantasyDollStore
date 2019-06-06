using UnityEngine;
using TabTale;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
#else
using CocoUIButtonID = Game.CocoUIButtonID;
using CocoAudioID = Game.CocoAudioID;
#endif


namespace CocoPlay
{
	[System.Serializable]
	public class CocoUIButtonIDProperty : CocoAlternativeProperty<CocoUIButtonID, string>
	{
		public CocoUIButtonIDProperty () : base ()
		{
		}

		public CocoUIButtonIDProperty (CocoUIButtonID id) : base (id)
		{
		}

		public CocoUIButtonIDProperty (string id) : base (id)
		{
		}
	}

	[System.Serializable]
	public class CocoUIButtonAudioProperty : CocoAlternativeProperty<CocoAudioID, string>
	{
		public CocoUIButtonAudioProperty () : base (CocoAudioID.Button_Click_01)
		{
		}

		public CocoUIButtonAudioProperty (CocoAudioID id) : base (id)
		{
		}

		public CocoUIButtonAudioProperty (string id) : base (id)
		{
		}
	}
}
