//
//  SKStoreProductViewControllerWrapperViewController.h
//  PublishingSDKCore
//
//  Created by Ariel Vardy on 3/16/15.
//  Copyright (c) 2015 TabTale. All rights reserved.
//

#if TARGET_OS_IPHONE

#import <StoreKit/StoreKit.h>
#import <PublishingSDKCore/BannersDelegate.h>

@interface TTUSKStoreProductViewControllerWrapper : SKStoreProductViewController

- (id)initWithDelegate:(id<PSDKBannersDelegate>)delegate;

@end

#endif
