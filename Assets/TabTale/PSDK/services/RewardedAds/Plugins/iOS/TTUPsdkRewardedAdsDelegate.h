//
//  TTUPsdkRewardeAdsDelegate.h
//  RuntimeConfigTestApp
//
//  Created by Israel Papoushado on 7/20/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <PublishingSDKCore/PublishingSDKCore.h>

@interface TTUPsdkRewardeAdsDelegate : NSObject<PSDKRewardedAdsDelegate>

-(void) adIsReady;
-(void) adWillShow;
-(void) adDidClose;
-(void) adShouldReward;
-(void) adShouldNotReward;

@end
