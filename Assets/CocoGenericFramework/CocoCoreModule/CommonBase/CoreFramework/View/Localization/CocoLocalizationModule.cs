using UnityEngine;
using System.Collections;
using CocoPlay;

namespace CocoPlay.Localization
{
    public class CocoLocalizationModule : CocoModuleBase
    {
        protected override void InitSignals ()
        {
            base.InitSignals ();
            Bind<CocoLanguageSetSingal> ();
            Bind<CocoLanguageUpdateSingal> ();
            BindCommand<CocoLanguageSetSingal, CocoLanguageCommand> ();
        }

        protected override void CleanSignals ()
        {
            Unbind<CocoLanguageSetSingal> ();
            Unbind<CocoLanguageUpdateSingal> ();
            base.CleanSignals ();
        }
    }
}
