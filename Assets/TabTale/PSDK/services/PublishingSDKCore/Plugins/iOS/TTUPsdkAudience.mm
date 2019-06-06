#import <Foundation/Foundation.h>
#import "TTUPsdkExternalConfiguration.h"
#import "TTUPsdkConfigurationDelegate.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkAudience

#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    void psdkAudienceSetBirthYear(int birthYear)
    {
        [[[PSDKServiceManager instance] audience] setBirthYear:birthYear];
    }
    
    int psdkAudienceGetAge()
    {
        return [[[PSDKServiceManager instance] audience] age];
    }
    
    int psdkAudiencegetAudienceMode()
    {
        AudienceMode mode = [[[PSDKServiceManager instance] audience] audienceMode];
        int modeInt = -1;
        switch (mode) {
            case AUDIENCE_MODE_CHILDREN:
                modeInt = 0;
                break;
            case AUDIENCE_MODE_MIXED_UNKNOWN:
                modeInt = 1;
                break;
            case AUDIENCE_MODE_MIXED_NON_CHILDREN:
                modeInt = 2;
                break;
            case AUDIENCE_MODE_NON_CHILDREN:
                modeInt = 3;
                break;
            case AUDIENCE_MODE_MIXED_CHILDREN:
                modeInt = 4;
                break;
            default:
                break;
        }
        return modeInt;
        
    }
}

@end
