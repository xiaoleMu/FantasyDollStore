using UnityEngine;
using System.Collections.Generic;

#if COCO_FAKE
using CocoAudioID = CocoPlay.Fake.CocoAudioID;

#else
using CocoAudioID = Game.CocoAudioID;
#endif


namespace CocoPlay
{
	public interface ICocoAudioData
	{
		void InitAudioDatas ();

		string GetAudioName (CocoAudioID audioId);
	}

	public abstract class CocoAudioBaseData : ICocoAudioData
	{
		public CocoAudioBaseData ()
		{
			InitAudioDatas ();
		}

		protected Dictionary<CocoAudioID, List<string>> m_AudioDataDic = new Dictionary<CocoAudioID, List<string>> ();

		protected void AddAudioData (CocoAudioID audioId,  params string[] str)
		{
			m_AudioDataDic.Add (audioId, new List<string>(str));
		}

		#region ICocoAudioData implementation

		public abstract void InitAudioDatas ();

		public virtual string GetAudioName (CocoAudioID audioId)
		{
			List<string> audioSou;
			if (!m_AudioDataDic.ContainsKey (audioId)) {
				return string.Empty;
			}

			var lsit =  m_AudioDataDic [audioId];

			return lsit[Random.Range(0, lsit.Count)];
		}

		#endregion
	}
}
