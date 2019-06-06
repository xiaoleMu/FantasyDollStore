#import <Foundation/Foundation.h>

#import "TTUPsdkAppLifeCycleMgr.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkAppLifeCycleMgr


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {

    int psdkAlmOnResume()
    {
        return [[[PSDKServiceManager instance] appLifeCycleMgr] onResume];
    }
    
    void psdkAlmApplicationWillResignActive() {
        [[[PSDKServiceManager instance] appLifeCycleMgr] applicationWillResignActive];
    }

    void psdkAlmOnPaused()
    {
        [[[PSDKServiceManager instance] appLifeCycleMgr] onPaused];
    }
    
    void psdkAlmOnStart()
    {
    }

    void psdkAlmOnStop()
    {
    }
    
    void psdkAlmOnDestroy()
    {
        [[[PSDKServiceManager instance] appLifeCycleMgr] onPaused];
    }
    
    void psdkSetConfigParams(int64_t sessionTime, int64_t restartTime, int64_t psdkReadyTimeout)
    {
        [[[PSDKServiceManager instance] appLifeCycleMgr] setConfigParams:sessionTime restartTime:restartTime psdkReadyTimeout:psdkReadyTimeout];
    }

    void psdkAlmAppIsReady() {
        [[[PSDKServiceManager instance] appLifeCycleMgr]  appIsReady];
    }
    
    void psdkAlmDidFinishLaunchingWithOptions() {;}
    void psdkAlmApplicationDidEnterBackground(){;}
    void psdkAlmApplicationWillEnterForeground(){;}
    void psdkAlmApplicationDidFinishLaunching(){;}
    void psdkAlmWillFinishLaunchingWithOptions(){;}
    void psdkAlmApplicationDidBecomeActive(){;}

}

@end
