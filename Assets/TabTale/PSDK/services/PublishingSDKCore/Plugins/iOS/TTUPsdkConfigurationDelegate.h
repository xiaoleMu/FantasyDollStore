//
//  TTUPsdkConfigurationDelegate.h
//  PublishingSDKCore
//
//  Created by Shmulik Armon on 21/02/2016.
//  Copyright © 2016 TabTale. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <PublishingSDKCore/PublishingSDKCore.h>

@interface TTUPsdkConfigurationDelegate : NSObject <PSDKConfigurationDelegate>

-(void) onConfigurationLoaded;

@end
