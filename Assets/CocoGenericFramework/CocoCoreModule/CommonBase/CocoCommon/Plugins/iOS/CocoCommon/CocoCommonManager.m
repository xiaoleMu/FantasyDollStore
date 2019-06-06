//
//  CocoCommonManager.m
//  Unity-iPhone
//
//  Created by Wei Kehui on 13-12-30.
//
//

#import "CocoCommonManager.h"

@interface CocoCommonManager ()

#ifdef ENABLE_FEATURE_MORE_APPS
@property (nonatomic, retain) MoreApps *moreAppsInstance;
#endif

@end

@implementation CocoCommonManager

#ifdef ENABLE_FEATURE_MORE_APPS
@synthesize moreAppsInstance;
#endif

+ (CocoCommonManager *)sharedInstance
{
    static CocoCommonManager * _sharedInstance;
    
    if (!_sharedInstance) {
        _sharedInstance = [[CocoCommonManager alloc] init];
    }
    
    return _sharedInstance;
}

- (BOOL)showAppStoreInternallyForID:(NSNumber *)appStoreID inViewController:(UIViewController *)viewController
{
    if(!NSClassFromString(@"SKStoreProductViewController")) { // Checks for iOS 6 feature.
        return NO;
    }
    
    SKStoreProductViewController *storeController = [[SKStoreProductViewController alloc] init];
    storeController.delegate = self; // productViewControllerDidFinish

    MBProgressHUD *hud = [[MBProgressHUD alloc] initWithView:viewController.view];
    hud.removeFromSuperViewOnHide = YES;

    [viewController.view addSubview:hud];
    [hud show:YES];
    
    NSDictionary *productParameters = @{ SKStoreProductParameterITunesItemIdentifier : appStoreID };
    [storeController loadProductWithParameters:productParameters completionBlock:^(BOOL result, NSError *error) {
        if (result) {
            [viewController presentViewController:storeController animated:YES completion:nil];
            [hud hide:YES afterDelay:1];
        } else {
            [hud hide:YES];
            [[[UIAlertView alloc] initWithTitle:@"Uh oh!" message:@"There was a problem displaying the app" delegate:nil cancelButtonTitle:@"Ok" otherButtonTitles: nil] show];
        }
    }];
    
//    [storeController release];
//    [hud release];
    
    return YES;
}

- (void)productViewControllerDidFinish:(SKStoreProductViewController *)viewController
{
    [viewController dismissViewControllerAnimated:YES completion:^{
    }];
}


#ifdef ENABLE_FEATURE_MORE_APPS
- (void)moreAppsShowInViewController:(UIViewController *)viewController useParentalGate:(BOOL) useParentalGate
{
    if (!self.moreAppsInstance) {
        MoreApps * moreApps = [[MoreApps alloc] initWithViewController:viewController];
        self.moreAppsInstance = moreApps;
        [moreApps release];
    }
    if (self.moreAppsInstance) {
        self.moreAppsInstance.delegate = self;
        self.moreAppsInstance.useParentalGate = useParentalGate;
        [self.moreAppsInstance show];
    }
}

- (void)moreAppsHide
{
    if (self.moreAppsInstance) {
        [self.moreAppsInstance dismiss];
    }
}

- (void)moreAppsShowViewDirectlyInViewController:(UIViewController *)viewController
{
    if (self.moreAppsInstance) {
        self.moreAppsInstance = NULL;
    }
    
    MoreApps * moreApps = [[MoreApps alloc] initWithViewController:viewController];
    self.moreAppsInstance = moreApps;
    [moreApps release];
    
    if (self.moreAppsInstance) {
        [self.moreAppsInstance showMoreApps];
    }
}

- (void)moreAppsDidShow
{
    NSLog(@"CocoCommonManager->moreAppsDidShow");
}

- (void)moreAppsDidDismiss
{
    NSLog(@"CocoCommonManager->moreAppsDidDismiss");
    self.moreAppsInstance = NULL;
}
#endif

@end
