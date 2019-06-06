
#import "TTUPsdkAnalyticsUnityBridge.h"
#import <PublishingSDKCore/PublishingSDKCore.h>
#import "TTUPsdkAnalyticsDelegate.h"

@implementation TTUPsdkAnalyticsUnityBridge


#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    void psdkRemoveExtras(const char* extrasStr) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        NSArray *keys = [[[NSString alloc] initWithUTF8String:extrasStr] componentsSeparatedByString:@";"];
        if (nil != service && nullptr != extrasStr) {
           [service removeExtras:keys];
        }
    }
    
    void psdkAddExtras(const char* extrasStr) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service && nullptr != extrasStr) {
            [service addExtras:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:extrasStr]];
        }
    }
    
    void psdkAnalyticsLogEvent(int64_t targets, const char* eventName, const char* eventParamsJsonStr, BOOL timed) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service logEvent:(AnalyticsType)targets
                         name:[[NSString alloc] initWithUTF8String:eventName]
                       params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]
                        timed: timed];
        }
    }
    
    void psdkAnalyticsLogEventInternal(int64_t targets, const char* eventName, const char* eventParamsJsonStr, BOOL timed, BOOL psdkEvent)
    {
        [[[PSDKServiceManager instance] analytics] logEvent:(AnalyticsType)targets
                                                       name:[[NSString alloc] initWithUTF8String:eventName]
                                                     params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]
                                                      timed:timed
                                                   internal:psdkEvent];
    }
    
    void psdkAnalyticsEndLogEvent(const char* eventName, const char* eventParamsJsonStr) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service endTimedEvent:[[NSString alloc] initWithUTF8String:eventName]
                            params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]];
        }
    }
    
    void psdkAnalyticsLogComplexEvent(const char* eventName, const char* eventParamsJsonStr) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service logComplexEvent:[[NSString alloc] initWithUTF8String:eventName]
                              params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:eventParamsJsonStr]
             ];
        }
    }
    
    void psdkAnalyticsReportPurchase(const char* price, const char* currency, const char* productId){
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            [service reportPurchase:[[NSString alloc] initWithUTF8String:price]
                           currency:[[NSString alloc] initWithUTF8String:currency]
                          productID:[[NSString alloc] initWithUTF8String:productId]];
        }
        
        
    }
    
    bool psdkRequestEngagement(const char* decisionPoint, const char* parameters) {
        id<PSDKAnalytics> service = [[PSDKServiceManager instance] analytics];
        if (nil != service) {
            return [service requestEngagement:[[NSString alloc] initWithUTF8String:decisionPoint] params:[TTUPsdkAnalyticsUnityBridge psdkAnalyticsDictionaryFromJsonStr:parameters]];
        }
        return false;
    }
    
    void psdkSetupAnalytics() {
        TTUPsdkAnalyticsDelegate *delegate = [[TTUPsdkAnalyticsDelegate alloc] init];
        [PSDKServiceManager setAnalyticsDelegate:delegate];
    }
}

+ (NSDictionary *) psdkAnalyticsDictionaryFromJsonStr: (const char*) json {
    if (json == nullptr) return nil;
    NSData *data = [[[NSString alloc] initWithUTF8String:json] dataUsingEncoding:NSUnicodeStringEncoding];
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:nil];
    return dict;
}

@end
