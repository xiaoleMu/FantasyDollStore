using System;
using UnityEngine;
using strange.extensions.command.impl;
using TabTale;

namespace TabTale {
    public class ShowAchievementsCommand : Command
    {   
        [Inject]
        public ISocialService socialService { get; set; }

         public override void Execute()
        {
            CoreLogger.LogDebug("AchievementsButtonCommand","Execute");
 
            socialService.ShowAchievements();

            //socialNetworkService.Init();
            
            //          GeneralDialogData data = new GeneralDialogData();
            //          data.buttons.Add(new BasicDialogButtonData("button1"));
            //          data.isCloseOnOutSideTap=false;
            //          generalDialog.Show(data);
            
        }
    }
}