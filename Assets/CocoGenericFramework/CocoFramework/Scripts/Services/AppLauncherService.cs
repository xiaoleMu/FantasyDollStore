using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace TabTale
{
    public class AppLauncherService
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void openAppLauncher(string appUrl, string appId, string storeID);
#endif
        public void OpenAppLauncher(string appUrl, string appId, string storeID)
        {
#if UNITY_IOS
            openAppLauncher(appUrl, appId, storeID);
#endif
        }

    }

}
