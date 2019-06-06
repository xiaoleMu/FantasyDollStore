
#import "TTUPsdkCrashToolUnityBridge.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkCrashToolUnityBridge


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
	void psdkCrashTool_addBreadCrumb(const char* crumb){ 
		id<PSDKCrashTool> service = [[PSDKServiceManager instance] crashToolService];
        	if (nil != service) {
            		[service addBreadCrumb:[[NSString alloc] initWithUTF8String:crumb]];
        	}
	}

	void psdkCrashTool_clearAllBreadCrumbs (){ 
        	id<PSDKCrashTool> service = [[PSDKServiceManager instance] crashToolService];
        	if (nil != service) {
            	[service clearAllBreadCrumbs];
        	}
	}
}

@end
