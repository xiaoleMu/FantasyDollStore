//
//  SKStoreProductViewControllerWrapper.m
//  PublishingSDKCore
//
//  Created by Ariel Vardy on 3/16/15.
//  Copyright (c) 2015 TabTale. All rights reserved.
//

#if TARGET_OS_IPHONE


#import "TTUSKStoreProductViewControllerWrapper.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@interface TTUSKStoreProductViewControllerWrapper()

@property (nonatomic, weak) id<PSDKBannersDelegate> bannerDelegate;

@end


@implementation TTUSKStoreProductViewControllerWrapper

- (id)initWithDelegate:(id<PSDKBannersDelegate>)delegate
{
    self = [super init];
    if (self) {
        _bannerDelegate = delegate;
        if (_bannerDelegate != nil){
            [_bannerDelegate onBannerWillDisplay];
        }
    }
    return self;
}

- (BOOL)shouldAutorotate
{
    return YES;
}

- (UIInterfaceOrientationMask)supportedInterfaceOrientations
{
    if([[[PSDKServiceManager instance] orientation] isEqualToString:PSDK_LANDSCAPE]){
        return (UIInterfaceOrientationMaskLandscape);
    }
    return (UIInterfaceOrientationMaskPortrait | UIInterfaceOrientationMaskPortraitUpsideDown);
}

-(void)dealloc
{
    if (_bannerDelegate != nil)
        [_bannerDelegate onBannerClose];
}

@end


#endif