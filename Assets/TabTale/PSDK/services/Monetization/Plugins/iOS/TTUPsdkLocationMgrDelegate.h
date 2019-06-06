//
//  TTUPsdkLocationMgrDelegate.h
//  Psdk Psdk Monetization
//
//  Created by Israel Papoushado on 10/7/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import <Monetization/Monetization.h>
#import <PublishingSDKCore/PublishingSDKCore.h>


@interface TTUPsdkLocationMgrDelegate : NSObject<PSDKLocationsMgrDelegate>

-(void) onLocationLoaded:(NSString*) location attribute:(long)attribute;
-(void) onLocationFailed:(NSString*) location error:(PublishingSDKError*) error;
-(void) onShown:(NSString*) location attribute:(long)attribute;
-(void) onClosed:(NSString*) location attribute:(long)attribute;
-(void) onConfigurationLoaded;

@end