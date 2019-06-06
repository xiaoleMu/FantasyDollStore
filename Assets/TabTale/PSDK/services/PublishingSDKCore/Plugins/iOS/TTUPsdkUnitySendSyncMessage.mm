//
//  TTUPsdkUnitySendSyncMessage.mm
//  Unity-iPhone
//
//  Created by israel Papoushado on 4/5/15.
//
//

#include <stdio.h>
#include "TTUPsdkUnitySendSyncMessage.h"


extern "C" {
    
    void InvokeManagedPsdkEventSystemCallback (BOOL usesUnitySendMessage, const char* message, const char* param);
    
    void PsdkUnitySendSyncMessage (const char* message, const char* param) {
        dispatch_async(dispatch_get_main_queue(), ^{
            InvokeManagedPsdkEventSystemCallback (NO, message, param);
        });
    }
}