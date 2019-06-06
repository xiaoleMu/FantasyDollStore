using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TabTale {

public class OpenSettingsButtonView : MainView 
{
    [Inject]
    public RequestOpenSettingsSignal openSettingsSignal {get; set;}

    [Inject]
    public SoundManager soundManager { get; set; }

    public void OnClick()
    {
        soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);
        openSettingsSignal.Dispatch();
    }
}

public class RequestOpenSettingsSignal : Signal
{

}
}




