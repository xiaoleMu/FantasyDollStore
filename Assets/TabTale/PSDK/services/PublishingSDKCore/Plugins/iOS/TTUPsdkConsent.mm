//
//  TTUPsdkConsent.m
//  InterstitialTestApp
//
//  Created by Shmulik Armon on 10/06/2018.
//  Copyright © 2018 TabTale. All rights reserved.
//

#import "TTUPsdkConsent.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkConsent

extern "C" {
        
    void psdkSetConsent(const char *consent)
    {
        TtConcentMode mode = [[[PSDKServiceManager instance] consentInstructor] consentFromString:[[NSString alloc] initWithUTF8String:consent]];
        [[[PSDKServiceManager instance] consentInstructor] setConsent:mode];
    }
    
    void psdkForgetUser()
    {
        [[[PSDKServiceManager instance] consentInstructor] forgetUser];
    }
    
    void psdkShowPrivacySettings()
    {
        [[[PSDKServiceManager instance] consentInstructor] showPrivacySettings];
    }
    
    const char * psdkGetConsent()
    {
        TtConcentMode consentMode = [[[PSDKServiceManager instance] consentInstructor] getConsent];
        NSString *consentModeStr = [[[PSDKServiceManager instance] consentInstructor] stringFromConsentMode:consentMode];
        return strdup([consentModeStr UTF8String]);
        
    }
}
    
    
@end
