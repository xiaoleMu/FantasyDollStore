using strange.extensions.command.impl;
using UnityEngine;

namespace TabTale
{
    public class OnApplicationPauseCommand : Command
    {

        public override void Execute()
        {
            Debug.Log("OnApplicationPauseCommand -------------------->");
        }
    }
}
