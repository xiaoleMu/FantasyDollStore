//
//  TTUPsdkConfigurationDelegate.m
//  PublishingSDKCore
//
//  Created by Shmulik Armon on 21/02/2016.
//  Copyright © 2016 TabTale. All rights reserved.
//

#import "TTUPsdkConfigurationDelegate.h"

extern void UnitySendMessage(const char *, const char *, const char *);

@implementation TTUPsdkConfigurationDelegate

-(void) onConfigurationLoaded
{
    ExtendNSLog(@"TTUPsdkConfigurationDelegate::onConfigurationLoaded");
    UnitySendMessage("PsdkEventSystem","OnConfigurationLoaded", "");
}

@end
