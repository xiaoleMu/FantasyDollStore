//
//  TTUPsdkSplashDelegate.m
//  RuntimeConfigTestApp
//
//  Created by Israel Papoushado on 7/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import "TTUPsdkStartupDelegate.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

extern void UnitySendMessage(const char *, const char *, const char *);

@implementation TTUPsdkStartupDelegate

- (void) onConfigurationReady
{
    ExtendNSLog(@"TTUPsdkStartupDelegate::onConfigurationReady");
    UnitySendMessage("PsdkEventSystem","OnConfigurationReady", "");
}

- (void) onPSDKReady
{
    ExtendNSLog(@"TTUPsdkStartupDelegate::onPSDKReady");
    UnitySendMessage("PsdkEventSystem","OnPSDKReady", "");
}

- (void) onRemoteConsentModeReady:(TtConcentFormType)concentFormType
{
    NSString *consentFormTypeStr = [[[PSDKServiceManager instance] consentInstructor] stringFromConsentFormType: concentFormType];
    ExtendNSLog(@"TTUPsdkStartupDelegate::onRemoteConsentModeReady:concentFormType: %@", consentFormTypeStr);
    UnitySendMessage("PsdkEventSystem","OnRemoteConsentModeReady", [[NSString stringWithFormat:@"{\"consentFormType\":\"%@\"}", consentFormTypeStr] UTF8String]);
}

@end
