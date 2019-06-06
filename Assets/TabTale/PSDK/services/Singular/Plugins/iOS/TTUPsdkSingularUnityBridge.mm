//
//  TTUPsdkSingularUnityBridge.m
//  SingularTestApp
//
//  Created by Ariel Vardy on 21/02/2018.
//  Copyright © 2018 Ariel Vardy. All rights reserved.
//

#import "TTUPsdkSingularUnityBridge.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@implementation TTUPsdkSingularUnityBridge

extern "C" {
    void psdkTutorialComplete()
    {
        id<PSDKSingular> service = [[PSDKServiceManager instance] singular];
        if (nil != service) {
            [service reportTutorialComplete];
        }
    }
    
    void psdkSingularLogEvent(const char* eventName, const char* eventParamsJson)
    {
        if (eventName == NULL){
            return;
        }
        NSString* name = [NSString stringWithUTF8String:eventName];
        NSDictionary *dict = nil;
        
        if (eventParamsJson != NULL){
            
            NSData *data = [[[NSString alloc] initWithUTF8String:eventParamsJson] dataUsingEncoding:NSUnicodeStringEncoding];
            dict = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:nil];
        }
       
        id<PSDKSingular> service = [[PSDKServiceManager instance] singular];
        if (nil != service) {
            [service logEventWithName:name params:dict];
        }
    }
}

@end
