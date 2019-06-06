//
//  TTUPsdkAppShelfDelegate.m
//  Unity Psdk delegate
//
//  Created by Israel Papoushado on 10/7/14.
//  Copyright (c) 2014 TabTale. All rights reserved.
//

#import "TTUPsdkWebViewDelegate.h"


extern void UnitySendMessage(const char *, const char *, const char *);


/////////////////////////////////////////////
////      PSDKWebViewDelegate      /////
/////////////////////////////////////////////
@implementation TTUPsdkWebViewDelegate

-(void) onPlaySound:(NSString*) sourceFilePath
{
    UnitySendMessage("PsdkEventSystem","onPlaySound", [sourceFilePath UTF8String]);
}

-(void) onStartAnimationEnded
{
    UnitySendMessage("PsdkEventSystem","onStartAnimationEnded", "");
    
}

@end
