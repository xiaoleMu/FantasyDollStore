//
//  TTUPsdkAnalyticsDelegate.m
//  PSDKAnalyticsTestApp
//
//  Created by Shmulik Armon on 10/01/2018.
//  Copyright © 2018 Tabtale. All rights reserved.
//

#import "TTUPsdkAnalyticsDelegate.h"

extern void UnitySendMessage(const char *, const char *, const char *);

@implementation TTUPsdkAnalyticsDelegate 

-(void) onRequestEngagementComplete:(NSString*)decisionPoint params:(NSDictionary*)params
{
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:params options:NSJSONWritingPrettyPrinted error:nil];
    
    NSString *str = [NSString stringWithFormat:@"{ \"decisionPoint\" : \"%@\", \"parameters\" : %@ }", decisionPoint, [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding]];
    UnitySendMessage("PsdkEventSystem","OnRequestEngagementComplete", [str UTF8String]);
}

@end
