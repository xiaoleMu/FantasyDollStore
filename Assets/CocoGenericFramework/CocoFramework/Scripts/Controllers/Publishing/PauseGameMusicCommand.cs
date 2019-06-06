using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using TabTale.Publishing;

namespace TabTale
{
    public class PauseGameMusicCommand : GameCommand
    {

		[Inject]
		public SoundManager soundManager { get; set; }

		[Inject]
		public bool mute { get; set; }

        public override void Execute ()
        {
            logger.Log("PauseGameMusicCommand executed!");

			if(mute)
			{
				logger.Log("PauseGameMusicCommand - Attempting to mute game sounds");
                soundManager.MuteAllLayers();
            }
			else
			{
                logger.Log("PauseGameMusicCommand - Attempting to unmute game sounds");
                soundManager.UnMuteAllLayers();
            }
        }
    }
    
}