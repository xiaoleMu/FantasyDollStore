#import <Foundation/Foundation.h>
#import "TTUPsdkLocationMgrUnityBridge.h"
#import <PublishingSDKCore/ServiceManager.h>
#import "TTUPsdkLocationMgrDelegate.h"
#import "TTUPsdkWebViewDelegate.h"
#import <PublishingSDKCore/LocationsMgrDelegate.h>


@implementation TTUPsdkLocationMgrUnityBridge


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    void psdkLocationMgrReportLocation(const char *location)
    {
        [[[PSDKServiceManager instance] locationsMgr] reportLocation: [[NSString alloc] initWithUTF8String:location]];
    }

    int64_t psdkLocationMgrShow(const char *location)
    {
        return [[[PSDKServiceManager instance] locationsMgr] show: [[NSString alloc] initWithUTF8String:location]];
        
    }
    
    int64_t psdkLocationMgrIsLocationReady(const char *location)
    {
        return [[[PSDKServiceManager instance] locationsMgr] isLocationReady: [[NSString alloc] initWithUTF8String:location]];
        
    }

    bool psdkLocationMgrIsViewVisible() {
        
        if ([PSDKServiceManager instance]  &&
            [[PSDKServiceManager instance] locationsMgr])
            return [[[PSDKServiceManager instance] locationsMgr] isViewVisible];
        
        return false;
    }
    
    bool psdkSetupLocationsManager() {
        
        id<PSDKLocationsMgrDelegate> locDelegate = [[TTUPsdkLocationMgrDelegate alloc] init];
         [PSDKServiceManager  setLocationMgrDelegate:locDelegate];

        id<PSDKWebViewDelegate> delegate = [[TTUPsdkWebViewDelegate alloc] init];
        [PSDKServiceManager  setupWebViewDelegate:delegate];
        
        NSLog(@"Setup location manager delegate !");
        return true;
    }
    const char* psdkLocationMgrGetLocations()
    {
        if ([PSDKServiceManager instance]  &&
            [[PSDKServiceManager instance] locationsMgr])
            return strdup([[[[PSDKServiceManager instance] locationsMgr] getLocations] UTF8String]);
        
        return nil;
    }
    
    void psdkLocationMgrLevelOfFirstPopupStatus (bool enabled)
    {
        [[[PSDKServiceManager instance] locationsMgr] levelOfFirstPopupStatus:enabled];
    }
 
}

@end
