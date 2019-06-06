//
//  AppLauncher.m
//  PublishingSDKCore
//
//  Created by Gal Briner on 3/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#if TARGET_OS_IPHONE

#import "TTUAppLauncher.h"
#import <PublishingSDKCore/PublishingSDKCore.h>
#import <Foundation/Foundation.h>
#import "TTUSKStoreProductViewControllerWrapper.h"


@interface PSDKAppLauncherNew()
@property (nonatomic, strong) PSDKAppLauncherNew* appLauncher;
@property (nonatomic, strong) SKStoreProductViewController* store;
@property (nonatomic, weak) id<PSDKBannersDelegate> delegate;
@end

@implementation PSDKAppLauncherNew

+ (void) detachReportPromoViewToURL:(NSDictionary*)params
{
    @autoreleasepool {
        NSString* url  = [params objectForKey:@"url"];
        NSLog(@"PSDKAppLauncherNew::appsflyer Url - %@", url);
        
        NSString* userAgent = [params objectForKey:@"userAgent"];
        
        if ([[PSDKServiceManager instance] appsFlyer] == nil || [[[PSDKServiceManager instance] appsFlyer] sendIDFA]){
            NSString* idfa = [PSDKUtilities identifierForAdvertising];
            
            if (idfa != nil){
                url = [NSString stringWithFormat:@"%@&idfa=%@", url, idfa];
            }
        }
        
        NSMutableURLRequest * request =[NSMutableURLRequest requestWithURL:[NSURL URLWithString:url] cachePolicy:NSURLRequestReloadIgnoringCacheData timeoutInterval:20.0];
        [request setHTTPMethod:@"GET"];
        [request setValue:@"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" forHTTPHeaderField:@"Accept"];
        
        if (userAgent == nil){
            NSString * deviceName = [UIDevice currentDevice].model;
            userAgent = [NSString stringWithFormat:@"Mozilla/5.0 (%@Modal; CPU iPhone OS 6_0_1 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Mobile/10A525", deviceName];
        }
        
        [request setValue:userAgent forHTTPHeaderField:@"User-Agent"];
        [NSURLConnection sendSynchronousRequest:request returningResponse:nil error:nil];
    }
}

+ (NSString *)userAgent
{
    UIWebView* webView = [[UIWebView alloc] initWithFrame:CGRectZero];
    return [webView stringByEvaluatingJavaScriptFromString:@"navigator.userAgent"];
}

+ (PSDKAppLauncherNew*) OpenApp:(NSString *)appName appUrl:(NSString *)appUrl appId:(NSString *)appId storeId:(NSString*)storeId delegate:(id<PSDKBannersDelegate>)delegate
{
    PSDKAppLauncherNew* appLauncher = nil;
    if (appId != nil && [appId length] > 0 && YES) {
        NSString *bundleUrl = [NSString stringWithFormat:@"%@://", appId];
        NSString *shortAppName = [self getAppName:appId];
        
        if ([[UIApplication sharedApplication] openURL: [NSURL URLWithString: bundleUrl]]){
            return appLauncher;
        }
        else{
            ExtendNSLog(@"PSDKAppLauncherNew::OpenApp - openURL failed bundleUrl: %@", bundleUrl);
        }
        
        
        bundleUrl = [NSString stringWithFormat:@"%@://",shortAppName];
        
        if ([[UIApplication sharedApplication] openURL: [NSURL URLWithString: bundleUrl]]){
            return appLauncher;
        }
        else{
            ExtendNSLog(@"PSDKAppLauncherNew::OpenApp - openURL failed bundleUrl: %@", bundleUrl);
        }
    }
    
    appLauncher = [[PSDKAppLauncherNew alloc] initWithDelegate:delegate];
    [appLauncher OpenApp:appUrl withID:appId storeId:storeId];
    return appLauncher;
}

+ (BOOL) isLocalApp:(NSString*)bundleId;
{
    return [[[PSDKServiceManager instance] userData] isLocalApp:bundleId];
}

+ (NSString*) getAppName:(NSString*)bundleId
{
    NSRange loc = [bundleId rangeOfString:@"." options:NSBackwardsSearch];
    if(loc.location != NSNotFound){
        return [bundleId substringFromIndex:loc.location+1];
    }
    return bundleId;
}

- (id)initWithDelegate:(id<PSDKBannersDelegate>)delegate
{
    self = [super init];
    if (self) {
        _delegate = delegate;
    }
    return self;
}

- (bool) isOSVersion6orGreater
{
    NSString *reqSysVer = @"6.0";
    NSString *currSysVer = [[UIDevice currentDevice] systemVersion];
    return ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending);
}

- (bool) isValidAppID: (NSString *)appID
{
    return (appID != nil && ([appID length] > 0) && ![appID isEqualToString:@"0"]);
}

- (void) OpenApp: (NSString *)appUrl withID:(NSString *)appID storeId:(NSString*)storeId
{
    _appLauncher = self;
    if ([self isOSVersion6orGreater] && [self isValidAppID:storeId]){
        // prepare & present the store view controller
        
        _store = [[TTUSKStoreProductViewControllerWrapper alloc] initWithDelegate:_delegate];
        _store.delegate = self;
        UIViewController* rootController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [rootController presentViewController:_store animated:YES completion: ^(void){
            if(_store == nil) {  //already tried to dismiss while animating
                dispatch_after(0, dispatch_get_main_queue(), ^{
                    [rootController dismissViewControllerAnimated:NO completion:nil];
                });
            }
        }];
        
        NSDictionary * storeParams = [NSDictionary dictionaryWithObject:[NSNumber numberWithInt:[storeId intValue]] forKey:SKStoreProductParameterITunesItemIdentifier];
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(closeModalDueToBackground) name:UIApplicationWillResignActiveNotification object:nil];
        [_store loadProductWithParameters:storeParams completionBlock:^(BOOL result, NSError *error) {
            if (result){
                ExtendNSLog(@"modal of store successfully loaded");
                //report view
                
                NSDictionary* dic = nil;
                if ([NSThread isMainThread]){
                    UIWebView* webView = [[UIWebView alloc] initWithFrame:CGRectZero];
                    NSString* userAgent = [webView stringByEvaluatingJavaScriptFromString:@"navigator.userAgent"];
                    dic = [[NSDictionary alloc] initWithObjectsAndKeys:appUrl, @"url", userAgent, @"userAgent", nil];
                }
                else{
                    dic = [[NSDictionary alloc] initWithObjectsAndKeys:appUrl, @"url", nil];
                }
                
                [NSThread detachNewThreadSelector:@selector(detachReportPromoViewToURL:) toTarget:self.class withObject:dic];
            }
            else { //move to app store - maybe there - we'll strike...
                ExtendNSLog(@"modal of store failed to load it will jump to the shortcut link");
                // in case the info is nil - it means that the user canceled the store modal
                if (error.code != 0){
                    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:appUrl]];
                }
                //_appLauncher = nil;
            }
        }];
    }
    else {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:appUrl]];
        _appLauncher = nil;
    }
    
}

-(void)closeModalDueToBackground
{
    [self closeModalView:NO];
}

- (void)closeModalView: (BOOL) animated
{
    UIViewController* rootController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
    [rootController dismissViewControllerAnimated:animated completion:nil];
    [[UIApplication sharedApplication] setStatusBarHidden: YES];
    _store.delegate = nil;
    _store = nil;
    _appLauncher = nil;
    
    [[NSNotificationCenter defaultCenter] removeObserver:self name:UIApplicationWillResignActiveNotification object:nil];
}

- (void)productViewControllerDidFinish:(SKStoreProductViewController *)viewController
{
    [self closeModalView:YES];
}

@end

#endif
