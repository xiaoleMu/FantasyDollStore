using System;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TabTale;

namespace TabTale 
{
    public class OpenShopCommand : Command
    {   
        [Inject]
        public IModalityManager modalityManager { get; set; }
        
        public override void Execute()
        {
            CoreLogger.LogDebug("OpenShopCommand","Execute");
            
            modalityManager.Add(new AppModalHandle("GamePopups/ShopModal"),true);
            
        }
        
        
    }
}
