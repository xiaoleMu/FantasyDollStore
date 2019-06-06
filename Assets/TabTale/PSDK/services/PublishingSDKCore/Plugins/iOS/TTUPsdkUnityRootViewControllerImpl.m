//
//  RootViewController.h
//  PublishingSDKCore
//
//  Created by Gal Briner on 12/31/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import "TTUPsdkUnityRootViewControllerImpl.h"

extern UIViewController*    UnityGetGLViewController();


@interface TTUPsdkUnityRootViewControllerImpl()
@end

@implementation TTUPsdkUnityRootViewControllerImpl

- (UIViewController*) get
{
    return UnityGetGLViewController();
}

@end
