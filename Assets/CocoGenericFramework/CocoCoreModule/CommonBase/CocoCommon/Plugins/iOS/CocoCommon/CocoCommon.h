//
//  CocoCommonConfiguration.h
//  Unity-iPhone
//
//  Created by Wei Kehui on 14-4-12.
//
//

#ifndef Unity_iPhone_CocoCommon_h
#define Unity_iPhone_CocoCommon_h

//#define ENABLE_FEATURE_SHARE
//#define ENABLE_FEATURE_FOR_PARENTS
//#define ENABLE_FEATURE_MORE_APPS
//#define ENABLE_FEATURE_PROMOTION
//#define ENABLE_FEATURE_PARENTAL_GATE
//#define ENABLE_FEATURE_GOOGLE_CONVERSION_PING

#ifdef ENABLE_FEATURE_SHARE
#import <ShareSDK/ShareSDK.h>
#endif

#ifdef ENABLE_FEATURE_FOR_PARENTS
#import "FPMainViewController.h"
#endif

#ifdef ENABLE_FEATURE_MORE_APPS
#import "MoreApps.h"
#endif

#ifdef ENABLE_FEATURE_PROMOTION
#import "CPPromotion.h"
#endif

#ifdef ENABLE_FEATURE_PARENTAL_GATE
#import "ParentalGate.h"
#endif

#ifdef ENABLE_FEATURE_GOOGLE_CONVERSION_PING
#import "GoogleConversionPing.h"
#endif

#import "CocoCommonManager.h"
#import "MBProgressHUD.h"

void _cocoCommonInitialize ();
bool _cocoCommonApplicationHandleOpenURL (UIApplication * application, NSURL *url);
bool _cocoCommonApplicationOpenURLWithSourceApplicationAndAnnotation (UIApplication * application, NSURL *url, NSString *sourceApplication, id annotation);

#endif
