//
//  TTUPsdkSplashDelegate.h
//  RuntimeConfigTestApp
//
//  Created by Israel Papoushado on 7/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <PublishingSDKCore/PublishingSDKCore.h>

@interface TTUPsdkStartupDelegate : NSObject<PSDKStartupDelegate>

- (void) onConfigurationReady;
- (void) onPSDKReady;
- (void) onRemoteConsentModeReady:(TtConcentFormType)concentFormType;

@end
