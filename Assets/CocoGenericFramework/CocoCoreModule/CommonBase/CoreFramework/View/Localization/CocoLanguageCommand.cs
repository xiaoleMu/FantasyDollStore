using UnityEngine;
using System.Collections;
using Game;
using strange.extensions.command.impl;

namespace CocoPlay.Localization
{
    public class CocoLanguageCommand : Command 
    {
        [Inject] public CocoLanguage languageType { get; set; }
        [Inject] public CocoLanguageUpdateSingal updateSignal { get; set; }
    
        public override void Execute ()
        {
            Retain ();
            SetLanguage();
        }
    
        void SetLanguage()
        {
            if(languageType == CocoLanguageSetting.Language)
                return;

            CocoLanguageSetting.SetLanguage(languageType);
            updateSignal.Dispatch();
        }
    }
}

