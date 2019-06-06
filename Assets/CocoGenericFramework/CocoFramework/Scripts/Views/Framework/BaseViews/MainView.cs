using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace TabTale
{
    public class MainView : GameView 
    {
        protected override void RegisterView()
        {
            RegisterWithContextType(ViewType.MainView);
        }
    }
}