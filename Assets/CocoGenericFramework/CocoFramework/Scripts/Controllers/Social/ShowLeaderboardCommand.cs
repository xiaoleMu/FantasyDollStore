using System;
using UnityEngine;
using strange.extensions.command.impl;
using TabTale;

namespace TabTale {

    public class ShowLeaderboardButtonCommand : Command
    {
        [Inject]
        public ISocialService socialService { get; set; }

        public override void Execute()
        {
            CoreLogger.LogDebug("ShowLeaderboardCommand","Execute");

            socialService.ShowLeaderboard();
        }
    }
}
