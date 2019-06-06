//
//  PSDKUnityDialog.m
//  UnityDialog
//
//  Created by Shmulik Armon on 26/04/2017.
//  Copyright © 2017 Tabtalett. All rights reserved.
//

#import "PSDKUnityDialog.h"

@implementation PSDKUnityDialog

extern "C" {
    void showDialog(const char * title, const char * message)
    {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:[[NSString alloc] initWithUTF8String:title]
                                                        message:[[NSString alloc] initWithUTF8String:message]
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        [alert show];
    }
}

@end
