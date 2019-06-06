#import <Foundation/Foundation.h>
#import "TTUPsdkExternalConfiguration.h"
#import "TTUPsdkConfigurationDelegate.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkInAppPurchase

#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    void psdkPurchaseCampaignCompleteTransaction(bool result)
    {
        [[[PSDKServiceManager instance] inAppPurchase] purchaseCampaignCompleteTransaction:result];
    }
    
    void psdkItemPurchased(const char *itemId)
    {
        if (itemId != NULL){
            [[[PSDKServiceManager instance] inAppPurchase] itemPurchased:[[NSString alloc] initWithUTF8String:itemId]];
        }
    }
    
    void psdkAddProduct(const char *productId, const char *price, bool isPurchased)
    {
        if (productId != NULL && price != NULL){
            [[[PSDKServiceManager instance] inAppPurchase] addProduct:[[NSString alloc] initWithUTF8String:productId] price:[[NSString alloc] initWithUTF8String:price] isPurchased:isPurchased];
        }
    }
    
}

@end
