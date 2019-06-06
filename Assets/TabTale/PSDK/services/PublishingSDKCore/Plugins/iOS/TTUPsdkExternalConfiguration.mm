#import <Foundation/Foundation.h>
#import "TTUPsdkExternalConfiguration.h"
#import "TTUPsdkConfigurationDelegate.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkExternalConfiguration

#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    void setupDelegate()
    {
        id<PSDKConfigurationDelegate> configurationDelegate = [[TTUPsdkConfigurationDelegate alloc] init];
        [PSDKServiceManager setConfigurationDelegate:configurationDelegate];
    }
    
    const char* psdkExtConfigurationGetExperimentGroup ()
    {
        NSString *group = [[[PSDKServiceManager instance] extConfiguration] getExperimentGroup];
        return strdup([group UTF8String]);
    }
}

@end
