//
//  TTUPsdkLocationMgrDelegate.m
//  Unity Psdk delegate
//
//  Created by Israel Papoushado on 10/7/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import "TTUPsdkLocationMgrDelegate.h"


extern void UnitySendMessage(const char *, const char *, const char *);
#if UNITY_VERSION < 500
extern void UnityPause(bool pause);
#endif


/////////////////////////////////////////////
////      PSDKLocationsMgrDelegate      /////
/////////////////////////////////////////////
@implementation TTUPsdkLocationMgrDelegate

NSString *_unityGameObjectListner = @"PsdkEventSystem";


-(void) onLocationLoaded:(NSString*) location attribute:(long)attribute
{
    if (location == nil) location = @"";
    NSString *str = [NSString stringWithFormat:@"{ \"location\" : \"%@\", \"attributes\" : %li }", location, attribute];
    UnitySendMessage([_unityGameObjectListner UTF8String],"OnLocationLoaded", [str UTF8String]);
}

-(void) onLocationFailed:(NSString*) location error:(PublishingSDKError*) error
{
    if (location == nil) location = @"";
    NSString *str = [NSString stringWithFormat:@"{ \"location\" : \"%@\", \"psdkError\" : \"%@\" }", location, error.description];
    UnitySendMessage([_unityGameObjectListner UTF8String],"OnLocationFailed", [str UTF8String]);
    
}

-(void) onShowFailed:(NSString *)location attribute:(long)attribute
{
    //it is unclear why we call onShowFailed and not onShownFailed. We keep it in case and old game updates psdk.
    [self onShownFailed: location attribute: attribute];
}



-(void) onShown:(NSString*) location attribute:(long)attribute
{
    if (location == nil) location = @"";
    NSString *str = [NSString stringWithFormat:@"{ \"location\" : \"%@\", \"attributes\" : %li }", location, attribute];
    UnitySendMessage([_unityGameObjectListner UTF8String],"OnShown", [str UTF8String]);
}

-(void) onShownFailed:(NSString *) location attribute:(long)attribute
{
    if (location == nil) location = @"";
    NSString *str = [NSString stringWithFormat:@"{ \"location\" : \"%@\", \"attributes\" : %li }", location, attribute];
    UnitySendMessage([_unityGameObjectListner UTF8String],"OnShownFailed", [str UTF8String]);
}


-(void) onClosed:(NSString*) location attribute:(long)attribute
{
    if (location == nil) location = @"";
    if (UnityIsPaused()) {
        UnityPause(false);
    }
    NSString *str = [NSString stringWithFormat:@"{ \"location\" : \"%@\", \"attributes\" : %li }", location, attribute];
    UnitySendMessage([_unityGameObjectListner UTF8String],"OnClosed", [str UTF8String]);
}

-(void) onConfigurationLoaded
{
    UnitySendMessage([_unityGameObjectListner UTF8String],"OnLocMgrConfigurationLoaded", "");
    
}

@end
