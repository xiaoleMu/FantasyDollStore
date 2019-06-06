//
//  TTUPsdkUnityAppController.m
//

#import <UIKit/UIKit.h>
#import "UnityAppController.h"
#import "UI/UnityView.h"
#import "UI/UnityViewControllerBase.h"
#import <PublishingSDKCore/PublishingSDKCore.h>

@interface ttuacMsgRecord : NSObject
{
    NSString *_method;
    NSString *_param;
    BOOL _usesUnitySendMessage;
};
@end

@implementation ttuacMsgRecord


-(id) initWithParams: (BOOL) usesUnitySendMessage  method:(NSString*)method param:(NSString*) param
{
    self = [super init];
    if (self) {
        _method = method;
        _usesUnitySendMessage = usesUnitySendMessage;
        _param = param;
    }
    return self;
}

-(NSString *)method {
    return _method;
}

-(NSString *)param {
    return _param;
}

-(BOOL) usesUnitySendMessage {
    return _usesUnitySendMessage;
}

@end

@interface TTUPsdkUnityAppController : UnityAppController

@end


extern void UnitySendMessage(const char *, const char *, const char *);
extern void psdkAlmApplicationWillResignActive();



@implementation TTUPsdkUnityAppController


typedef void (*PsdkEventSystemCallbackFromNative_callback_t) (const char* message, const char* param);

static PsdkEventSystemCallbackFromNative_callback_t mPsdkEventSystemCallbackFromNative;
static NSMutableArray *_msgsQueue;

BOOL isCallbackAlreadyRegistered() {
    return (mPsdkEventSystemCallbackFromNative != nil);
}

void enqueue(BOOL usesUnitySendMessage, NSString *method, NSString *param) {
    if (_msgsQueue == nil) {
        _msgsQueue = [[NSMutableArray alloc] init];
    }
    [_msgsQueue addObject:[[ttuacMsgRecord alloc] initWithParams:usesUnitySendMessage method:method param:param]];
}

ttuacMsgRecord *dequeue() {
    if (_msgsQueue == nil) {
        return nil;
    }
    if ([_msgsQueue count] == 0) {
        return nil;
    }
    ttuacMsgRecord *item = [_msgsQueue objectAtIndex:0];
    [_msgsQueue removeObjectAtIndex:0];
    return item;
}


extern "C" void InvokeManagedPsdkEventSystemCallback (BOOL usesUnitySendMessage, const char* message, const char* param)
{
    if (message != NULL) { // when calling from the callback registration, it will be with null;
        enqueue(usesUnitySendMessage, [[NSString alloc] initWithUTF8String:message], [[NSString alloc] initWithUTF8String:param]);
    }
    if (mPsdkEventSystemCallbackFromNative == NULL){
        return;
    }
    while ([_msgsQueue count] > 0) {
        ttuacMsgRecord *item = dequeue();
        if ([item usesUnitySendMessage]) {
            UnitySendMessage("PsdkEventSystem",[[item method] UTF8String],[[item param] UTF8String]);
        }
        else {
            (*mPsdkEventSystemCallbackFromNative) ([[item method] UTF8String], [[item param] UTF8String]);
        }
    }
}



extern "C" void RegisterSendDirectMessageToUnityCallback (PsdkEventSystemCallbackFromNative_callback_t cb)
{
    mPsdkEventSystemCallbackFromNative = cb;
    InvokeManagedPsdkEventSystemCallback(NO,NULL,NULL); // for calling all enqueued messages;
}


- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions{
    
    NSLog(@"TTUPsdkUnityAppController didFinishLaunchingWithOptions");
    
    BOOL success = [super application:application didFinishLaunchingWithOptions:launchOptions];
    InvokeManagedPsdkEventSystemCallback(NO,"didFinishLaunchingWithOptions","");
    UnitySendMessage("PsdkEventSystem","didFinishLaunchingWithOptions", "");
    
    return success;
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    NSLog(@"TTUPsdkUnityAppController applicationDidEnterBackground");
    [super applicationDidEnterBackground:application];
    if (mPsdkEventSystemCallbackFromNative == NULL){ // empty the queue
        // not sending messages prior to pause.
        [_msgsQueue removeAllObjects];
    }
    else {
        InvokeManagedPsdkEventSystemCallback(NO,"applicationDidEnterBackground","");
    }
    //UnitySendMessage("PsdkEventSystem","applicationDidEnterBackground", "");
}


- (void)applicationWillEnterForeground:(UIApplication *)application {
    NSLog(@"TTUPsdkUnityAppController applicationWillEnterForeground");
    [super applicationWillEnterForeground:application];
    InvokeManagedPsdkEventSystemCallback(YES,"applicationWillEnterForeground","");
}

- (void)applicationDidFinishLaunching:(UIApplication *)application {
    NSLog(@"TTUPsdkUnityAppController applicationDidFinishLaunching");
    [super applicationDidFinishLaunching:application];
    InvokeManagedPsdkEventSystemCallback(NO,"applicationDidFinishLaunching","");
    //UnitySendMessage("PsdkEventSystem","applicationDidFinishLaunching", "");
}

- (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    NSLog(@"TTUPsdkUnityAppController willFinishLaunchingWithOptions");
    InvokeManagedPsdkEventSystemCallback(NO,"willFinishLaunchingWithOptions","");
    //UnitySendMessage("PsdkEventSystem","willFinishLaunchingWithOptions", "");
    return YES;
}

- (void)applicationDidBecomeActive:(UIApplication *)application {
    NSArray* views = [[[UIApplication sharedApplication] keyWindow] subviews];
    
    NSLog(@"TTUPsdkUnityAppController applicationDidBecomeActive");
    [super applicationDidBecomeActive:application];
    InvokeManagedPsdkEventSystemCallback(YES,"applicationDidBecomeActive","");
    //UnitySendMessage("PsdkEventSystem","applicationDidBecomeActive", "");
    
    //reaging the view back after Unity destory them
    for (UIView *view in views) {
        if ([[[[UIApplication sharedApplication] keyWindow] subviews] containsObject:view]){
            NSLog(@"Ariel moving view %@ to front", view);
            [_window bringSubviewToFront:view];
        }
    }
}

- (void)applicationWillResignActive:(UIApplication *)application {
    NSLog(@"TTUPsdkUnityAppController applicationWillResignActive");
    [super applicationWillResignActive:application];
    //UnitySendMessage("PsdkEventSystem","applicationWillResignActive", "");
    // The UnitySendMessage was Async, and we need it to be blocking here.
    InvokeManagedPsdkEventSystemCallback(NO,"applicationWillResignActive","");
}


@end



IMPL_APP_CONTROLLER_SUBCLASS(TTUPsdkUnityAppController)
