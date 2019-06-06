#import <Foundation/Foundation.h>

#import "TTUPsdkRewardedAdsPlugin.h"
#import "TTUPsdkRewardedAdsDelegate.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkRewardedAdsPlugin


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {

    bool psdkSetupRewardedAds()
    {
        id<PSDKRewardedAdsDelegate> rewardedAdsDelegate = [[TTUPsdkRewardeAdsDelegate alloc] init];
        [PSDKServiceManager setRewardedAdsDelegate:rewardedAdsDelegate];
        NSLog(@"Setuped RewardedAds delegate !");
       
        return true;
    }

    bool psdkRewardedAds_ShowAd() {
        return [[[PSDKServiceManager instance]  rewardedAdsService] showAd];
    }
    
    bool psdkRewardedAds_IsAdReady() {
        return [[[PSDKServiceManager instance]  rewardedAdsService] isAdReady];
    }
    
    bool psdkRewardedAds_IsAdPlaying() {
        return [[[PSDKServiceManager instance]  rewardedAdsService] isAdPlaying];
    }
    

}

@end
