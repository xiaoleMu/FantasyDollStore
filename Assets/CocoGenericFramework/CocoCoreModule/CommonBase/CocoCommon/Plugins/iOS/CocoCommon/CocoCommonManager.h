//
//  CocoCommonManager.h
//  Unity-iPhone
//
//  Created by Wei Kehui on 13-12-30.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

#import "CocoCommon.h"

#ifdef ENABLE_FEATURE_MORE_APPS
@interface CocoCommonManager : NSObject<SKStoreProductViewControllerDelegate, MAMoreAppsDelegate>
#else
@interface CocoCommonManager : NSObject<SKStoreProductViewControllerDelegate>
#endif

+ (CocoCommonManager *)sharedInstance;

- (BOOL)showAppStoreInternallyForID:(NSNumber *)appStoreID inViewController:(UIViewController *)viewController;

#ifdef ENABLE_FEATURE_MORE_APPS
- (void)moreAppsShowInViewController:(UIViewController *)viewController useParentalGate:(BOOL) useParentalGate;
- (void)moreAppsHide;
// directly show more apps view
- (void)moreAppsShowViewDirectlyInViewController:(UIViewController *)viewController;
#endif

@end
