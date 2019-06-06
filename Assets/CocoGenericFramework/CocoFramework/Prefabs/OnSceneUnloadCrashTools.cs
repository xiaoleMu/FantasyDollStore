using UnityEngine;
using System.Collections;
using TabTale;

public class OnSceneUnloadCrashTools : GameView {


    [Inject]
    public ICrashTools crashTools { get; set; }


    void OnDestroy()
    {
        crashTools.AddBreadCrumb("On scene closing: " + Application.loadedLevelName);
    }
}
