//
//  TTUPsdkInAppDelegate.m
//  PSDKAnalyticsTestApp
//
//  Created by Shmulik Armon on 01/12/2016.
//  Copyright © 2016 Tabtale. All rights reserved.
//

#import "TTUPsdkInAppDelegate.h"

extern void UnitySendMessage(const char *, const char *, const char *);

@implementation TTUPsdkInAppDelegate

typedef char * (*PsdkEventSystemCallbackFromNativeWithReturn_callback_t) (const char* message, const char* param);

static PsdkEventSystemCallbackFromNativeWithReturn_callback_t mPsdkEventSystemCallbackFromNativeWithReturn;


extern "C" void RegisterSendDirectMessageToUnityCallbackWithReturn(PsdkEventSystemCallbackFromNativeWithReturn_callback_t cb){
    mPsdkEventSystemCallbackFromNativeWithReturn = cb;
}

-(NSDictionary *)callUnityMethod:(NSString *)method params:(NSDictionary *)params
{
    NSString *paramsString = @"";
    if (params != nil) {
        NSError *e = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:params
                                                       options:0
                                                         error:&e];
        if(e == nil){
            paramsString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        }
    }
    char *r = (*mPsdkEventSystemCallbackFromNativeWithReturn) ([method UTF8String], [paramsString UTF8String]);
    if (r == NULL) {
        return nil;
    }
    NSString *result = [NSString stringWithUTF8String:strdup(r)];
    
    NSError *e = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:[result dataUsingEncoding:NSUTF8StringEncoding]
                                                         options:NSJSONReadingMutableContainers
                                                           error:&e];
    if(e == nil){
        return dict;
    }
    return nil;
}

-(NSString *)getSceneName
{
    return [[PSDKServiceManager instance] getSceneName];
    
}

-(int)getCurrencyBalance:(NSString *)currencyId
{
    return 0;
}

-(BOOL)isReadyForSale:(NSString *)productId
{
    return [[[PSDKServiceManager instance] inAppPurchase] isItemAvailable:productId];
    
}

-(BOOL)isReadyForSale:(NSString *)productId amount:(int)amount currencyId:(NSString *)currencyId
{
    NSDictionary *dict = [self callUnityMethod:@"OnIsReadyForSale" params:[NSDictionary dictionaryWithObjectsAndKeys:
                                                                           productId,                                  @"itemId",
                                                                           [NSString stringWithFormat:@"%d",amount],   @"amount",
                                                                           currencyId,                                 @"currencyId"
                                                                           , nil]];
    if(dict != nil){
        NSNumber *result = [dict objectForKey:@"result"];
        if(result != nil){
            return [result boolValue];
        }
    }
    
    return NO;
    
}

-(NSString *)getPriceString:(NSString *)productId
{
    return [[[PSDKServiceManager instance] inAppPurchase] getPrice:productId];
}

-(void)purchase:(NSString *)productId
{
    NSError *e = nil;
    NSData *data = [NSJSONSerialization dataWithJSONObject:[NSDictionary dictionaryWithObjectsAndKeys:
                                                            productId,                          @"productId",
                                                            nil]
                                                   options:NSJSONReadingMutableContainers
                                                     error:&e];
    if (e == nil) {
        UnitySendMessage("PsdkEventSystem", "OnPurchase", [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] UTF8String]);
    }
}

-(void)purchase:(NSString *)productId amount:(int)amount currencyId:(NSString *)currencyId
{
    NSError *e = nil;
    NSData *data = [NSJSONSerialization dataWithJSONObject:[NSDictionary dictionaryWithObjectsAndKeys:
                                                            productId,                                  @"itemId",
                                                            [NSString stringWithFormat:@"%d",amount],   @"amount",
                                                            currencyId,                                 @"currencyId"
                                                            , nil]
                                                   options:NSJSONReadingMutableContainers
                                                     error:&e];
    if (e == nil) {
        UnitySendMessage("PsdkEventSystem", "OnPurchase", [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] UTF8String]);
    }
}

@end
