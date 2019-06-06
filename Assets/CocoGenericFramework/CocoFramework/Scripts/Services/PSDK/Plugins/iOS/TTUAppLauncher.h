//
//  AppLauncher.h
//  PublishingSDKCore
//
//  Created by Gal Briner on 3/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#if TARGET_OS_IPHONE

#import "StoreKit/SKStoreProductViewController.h"
#import <Foundation/Foundation.h>
#import <PublishingSDKCore/BannersDelegate.h>

@interface PSDKAppLauncherNew : NSObject<SKStoreProductViewControllerDelegate>
+ (PSDKAppLauncherNew*) OpenApp:(NSString *)appName appUrl:(NSString *)appUrl appId:(NSString *)appId storeId:(NSString*)storeId delegate:(id<PSDKBannersDelegate>)delegate;
+ (BOOL) isLocalApp:(NSString*)bundleId;
@end

#endif
