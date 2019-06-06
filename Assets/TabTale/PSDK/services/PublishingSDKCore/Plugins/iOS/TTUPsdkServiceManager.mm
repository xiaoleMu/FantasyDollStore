#import <Foundation/Foundation.h>
#import "TTUPsdkServiceManager.h"
#import "TTUPsdkConfigurationDelegate.h"
#import <PublishingSDKCore/ServiceManager.h>
#import "TTUPsdkStartupDelegate.h"
#import "TTUPsdkUnityRootViewControllerImpl.h"
#import "TTUPsdkInAppDelegate.h"

extern "C" UIViewController*	UnityGetGLViewController();

@implementation TTUPsdkServiceManager

#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    

    bool psdkSetup(const char *configJson, const char *language)
    {
        
        NSData *data = [[[NSString alloc] initWithUTF8String:configJson] dataUsingEncoding:NSUnicodeStringEncoding];
        NSDictionary *configDict = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:nil];
        
        NSString *languageStr = nil;
        
        if (language)
            languageStr = [[NSString alloc] initWithUTF8String:language];
        
        [PSDKServiceManager setInAppDelegate:[[TTUPsdkInAppDelegate alloc] init]];
        
        id<PSDKStartupDelegate> startupDelegate = [[TTUPsdkStartupDelegate alloc] init];

        PSDKServiceManager *sm = [PSDKServiceManager setup:configDict rootViewController:[TTUPsdkUnityRootViewControllerImpl alloc] language:languageStr startupDelegate:startupDelegate];
       
        if (sm == nil) {
            NSLog(@"Failure in PSDK setup !!!");
            return false;
        }
        
        return true;
    }
    
    void psdkSetLanguage(const char *language)
    {
        [[PSDKServiceManager instance] setLanguage:[[NSString alloc] initWithUTF8String:language]];
    }

    void psdkServiceMgr_PurchaseAd()
    {
        [[PSDKServiceManager instance] purchaseAd];
    }
    
    void psdkServiceManager_reportLevel(int level)
    {
        [[PSDKServiceManager instance] reportLevel:level];
    }

    const char *psdkServiceManager_getAppID() {
        NSString * appId =[[PSDKServiceManager instance] getAppID];
        if (appId != nil)
            return strdup([appId UTF8String]);
        return nil;
    }
    
    void psdkValidatePurchase(const char * price, const char * currency, const char * productId)
    {
        NSString *sPrice =[[NSString alloc] initWithUTF8String: price != NULL ? price : ""];
        NSString *sCurrency =[[NSString alloc] initWithUTF8String:currency != NULL ? currency : ""];
        NSString *sProductId =[[NSString alloc] initWithUTF8String:productId != NULL ? productId : ""];
        [[[PSDKServiceManager instance] purchaseValidation] validateReceiptAndReport:sPrice currency:sCurrency productID:sProductId completionHandler:^(BOOL verified) {
            NSLog(@"TTUPsdkServiceManager::OnValidateResponse");
            NSString *str = [NSString stringWithFormat:@"{ \"price\" : \"%@\", \"currency\" : \"%@\", \"productId\" : \"%@\", \"valid\" : %s  }", sPrice, sCurrency, sProductId, verified ? "true" : "false" ];
            UnitySendMessage("PsdkEventSystem","OnValidateResponse", [str UTF8String] );
        }];
    }
    
    void psdkSetSceneName(const char *sceneName)
    {
        [[PSDKServiceManager instance] setSceneName:[[NSString alloc] initWithUTF8String:sceneName]];
    }
    
}

@end
