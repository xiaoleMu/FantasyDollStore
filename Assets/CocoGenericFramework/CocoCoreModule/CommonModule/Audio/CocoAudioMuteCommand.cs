using UnityEngine;
using System.Collections;
using TabTale;

namespace CocoPlay
{
	public class CocoAudioMuteCommand : GameCommand
	{
		[Inject]
		public bool mute { get; set; }

		public override void Execute ()
		{
			//Debug.LogErrorFormat ("SoundManager->CocoAudioMuteCommand.Execute:  mute {1}", GetType ().Name, mute);
			CocoAudio.MuteAll (mute);
		}
	}
}