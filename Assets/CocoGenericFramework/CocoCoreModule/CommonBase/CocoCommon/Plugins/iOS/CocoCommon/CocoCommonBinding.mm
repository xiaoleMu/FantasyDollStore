//
//  CocoCommonBinding.mm
//  Unity-iPhone
//
//  Created by Wei Kehui on 13-2-28.
//
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "CocoCommon.h"
#import <AVFoundation/AVFoundation.h>

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

UIViewController * UnityGetGLViewController();
UIView * UnityGetGLView();
void UnityPause(bool pause);

#pragma mark ---- Common APIs ----

void _cocoCommonInitialize ()
{
#ifdef ENABLE_FEATURE_SHARE
    [ShareSDK registerApp:@"e0fe734695d"];
    [ShareSDK convertUrlEnabled:NO];
    [ShareSDK connectSinaWeiboWithAppKey:@"341006842"
                               appSecret:@"bf0db12a3cd42b311c2700dfd9720040"
                             redirectUri:@"http://weibo.com"];
    [ShareSDK connectFacebookWithAppKey:@"692004924143770"
                              appSecret:@"0b86cddde137b500c443ba157f269c36"];
    [ShareSDK connectMail];
#endif
    
#ifdef ENABLE_FEATURE_GOOGLE_CONVERSION_PING
    // Admob
    [GoogleConversionPing pingWithConversionId:@"981061405" label:@"jskgCIum9QYQnZ7n0wM" value:@"0.12" isRepeatable:NO];
#endif
}

bool _cocoCommonApplicationHandleOpenURL (UIApplication * application, NSURL *url)
{
#ifdef ENABLE_FEATURE_SHARE
    return [ShareSDK handleOpenURL:url wxDelegate:application];
#else
    return false;
#endif
}

bool _cocoCommonApplicationOpenURLWithSourceApplicationAndAnnotation (UIApplication * application, NSURL *url, NSString *sourceApplication, id annotation)
{
#ifdef ENABLE_FEATURE_SHARE
    return [ShareSDK handleOpenURL:url sourceApplication:sourceApplication annotation:annotation wxDelegate:application];
#else
    return false;
#endif
}


#ifdef __cplusplus
extern "C" {
#endif
    
bool _cocoCommonCameraDeviceIsAvailable ()
{
    if (_ios70orNewer) {
        __block bool ret = false;
        
        AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
        if (status == AVAuthorizationStatusAuthorized) {
            // authorized
            ret = true;
        } else if (status == AVAuthorizationStatusDenied) {
            // denied
            ret = false;
        } else if (status == AVAuthorizationStatusRestricted) {
            // restricted
            ret = false;
        } else if (status == AVAuthorizationStatusNotDetermined) {
            // not determined
            __block bool closed = false;
            
            [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
                ret = granted;
                closed = true;
            }];
            
            while (!closed) {
                usleep(100000);
            }
        }
        
        return ret;
    }
    
    return false;
//    return [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
}

char * _getCountryCode(){
    NSLocale *locale = [NSLocale currentLocale];
    NSString *countrycode = [locale localeIdentifier];
    NSLog(@"COUNTRY CODE：%@",countrycode);

//    const char *country = [countrycode UTF8String];
//    char *back = (char *)malloc(countrycode.length + 1);
//    for (int i = 0;i<countrycode.length; i++) {
//         back[i] = country[i];
//     }
//     return back;

    char *country = MakeStringCopy(countrycode);
    return country;
}

float _cocoCommonScreenScaleFactor()
{
    return UnityGetGLView().contentScaleFactor;
}

bool _cocoCommonShowAppStoreInternally (long appId)
{
   return [[CocoCommonManager sharedInstance] showAppStoreInternallyForID:[NSNumber numberWithLong:appId]inViewController:UnityGetGLViewController()];
}

void _cocoCommonShowPromptMessage (const char * msg, float duration)
{
    UIView * unityView = UnityGetGLView();
    
    MBProgressHUD * hud = [[MBProgressHUD alloc] initWithView:unityView];
    [unityView addSubview:hud];
    hud.removeFromSuperViewOnHide = YES;
    hud.mode = MBProgressHUDModeText;
    
    hud.labelText = GetStringParam(msg);
    [hud show:YES];
    [hud hide:YES afterDelay:duration];
    
//    [hud release];
}

void _cocoCommonNSLog (const char * msg) {
    NSLog(@"%@", GetStringParam(msg));
}

#pragma mark ---- Share Features ----
    
#ifdef ENABLE_FEATURE_SHARE
void _cocoCommonShowISSContentForShareType ( ShareType shareType, id<ISSContent> shareContent, NSString * title)
{
    UnityPause(true);
    
    id<ISSShareOptions> shareOptions = [ShareSDK defaultShareOptionsWithTitle:title
                                                              oneKeyShareList:nil
                                                               qqButtonHidden:YES
                                                        wxSessionButtonHidden:YES
                                                       wxTimelineButtonHidden:YES
                                                         showKeyboardOnAppear:YES
                                                            shareViewDelegate:nil
                                                          friendsViewDelegate:nil
                                                        picViewerViewDelegate:nil];
    
    
    id<ISSContainer> container = [ShareSDK container];
    [container setIPhoneContainerWithViewController:UnityGetGLViewController()];
    [container setIPadContainerWithView:UnityGetGLView() arrowDirect:UIPopoverArrowDirectionUp];
    
    
    [ShareSDK showShareViewWithType:shareType
                          container:container
                            content:shareContent
                      statusBarTips:YES
                        authOptions:nil
                       shareOptions:shareOptions
                             result:^(ShareType type, SSResponseState state, id<ISSPlatformShareInfo> statusInfo, id<ICMErrorInfo> error, BOOL end) {
                                 switch (state) {
                                     case SSResponseStateSuccess:
                                         NSLog(@"Share Success");
                                         UnitySendMessage("CocoCommonBinding", "ShareContentSucceeded", "");
                                         break;
                                     case SSResponseStateCancel:
                                         NSLog(@"Share Cancelled");
                                         UnitySendMessage("CocoCommonBinding", "ShareContentCancelled", "");
                                         break;
                                         
                                     case SSResponseStateBegan:
                                         NSLog(@"Share Began");
                                         break;
                                         
                                     default:
                                         NSLog(@"Share Failed, error code:%d, error description:%@", [error errorCode],
                                               [error errorDescription]);
                                         NSString *errorResult = [NSString stringWithFormat:@"%d : %@", [error errorCode], [error errorDescription]];
                                         UnitySendMessage("CocoCommonBinding", "ShareContentFailed", MakeStringCopy(errorResult));
                                         break;
                                 }
                                 
                                 UnityPause(false);
                             }];
}

void _cocoCommonEnableSSO(bool isEnabled)
{
    [ShareSDK ssoEnabled:isEnabled];
}


void _cocoCommonShowContentForShareType( ShareType shareType, const char * imagePath, const char * title, const char * content )
{
    NSString * nsImagePath = GetStringParam( imagePath );
    NSString * nsTitle = GetStringParam( title );
    NSString * nsContent = GetStringParam( content );
    
    id<ISSContent> shareContent = [ShareSDK content:nsContent
                                defaultContent:@""
                                         image:[ShareSDK imageWithPath:nsImagePath]
                                         title:nsTitle
                                           url:nil
                                   description:nil
                                     mediaType:SSPublishContentMediaTypeNews];
    
    
    _cocoCommonShowISSContentForShareType(shareType, shareContent, nsTitle);
}

void _cocoCommonShowContentForShareMail( const char * imagePath,const char * title,  const char * content, bool isHTML , const char * toAddress )
{
    NSString * nsImagePath = GetStringParam( imagePath );
    NSString * nsTitle = GetStringParam( title );
    NSString * nsContent = GetStringParam( content );
    NSString * nsToAddress = GetStringParam (toAddress);
    NSArray * toAddresses = toAddress != NULL ? [nsToAddress componentsSeparatedByString:@","] : INHERIT_VALUE;

    id<ISSContent> shareContent = [ShareSDK content:nsContent
                                     defaultContent:@""
                                              image:[ShareSDK imageWithPath:nsImagePath]
                                              title:nsTitle
                                                url:nil
                                        description:nil
                                          mediaType:SSPublishContentMediaTypeNews];
    
    [shareContent addMailUnitWithSubject:nsTitle
                                  content:nsContent
                                    isHTML:[NSNumber numberWithBool:isHTML]
                               attachments:INHERIT_VALUE
                                        to:toAddresses
                                        cc:INHERIT_VALUE
                                       bcc:INHERIT_VALUE];
    
    _cocoCommonShowISSContentForShareType(ShareTypeMail, shareContent, nsTitle);
}
#endif
    

#pragma mark ---- ForParents Features ----
    
#ifdef ENABLE_FEATURE_FOR_PARENTS
void _cocoCommonShowForParents(long appId)
{
    FPMainViewController *forParentViewController = [[FPMainViewController alloc] initWithNibName:nil bundle:nil appStoreId:appId];
    UIViewController *viewController = UnityGetGLViewController();
    if (viewController) {
        [viewController presentViewController:forParentViewController animated:YES completion:^{
        }];
    }
    [forParentViewController release];
}
#endif
    
    
#pragma mark ---- MoreApps Features ----
    
#ifdef ENABLE_FEATURE_MORE_APPS
void _cocoCommonShowMoreApps(bool useParentalGate)
{
    [[CocoCommonManager sharedInstance] moreAppsShowInViewController:UnityGetGLViewController() useParentalGate:useParentalGate];
}
    
void _cocoCommonHideMoreApps()
{
    [[CocoCommonManager sharedInstance] moreAppsHide];
}
    
void _cocoCommonShowMoreAppsView()
{
    [[CocoCommonManager sharedInstance] moreAppsShowViewDirectlyInViewController:UnityGetGLViewController()];
}
#endif
    

#pragma mark ---- Promotion Features ----
    
#ifdef ENABLE_FEATURE_PROMOTION
bool _cocoCommonShowPromotion( const char * url, bool needCallback)
{
    CPPromotion * promotion = [CPPromotion sharedInstance];
    if (!promotion) {
        NSLog(@"Promotion: instantiation failed.");
        return false;
    }
    
    UIView * view = UnityGetGLView();
    if (!view) {
        NSLog(@"Promotion: UnityGetGLView NULL.");
        return false;
    }
    
    NSString * nsUrl = GetStringParam(url);
    
    if (!needCallback) {
        [promotion promotionShowInView:view withIndexUrl:nsUrl];
    } else {
        [promotion promotionShowInView:view withIndexUrl:nsUrl completionBlock:^{
            NSLog(@"Promotion: view closed.");
            UnitySendMessage("CocoCommonBinding", "PromotionClosed", "");
        }];
    }
    
    return true;
}
#endif
    
//Check photo library.  
#import <Photos/Photos.h>

bool _cocoPhotoLibraryIsAvailable ()
{
    if (_ios80orNewer) {
        PHAuthorizationStatus curStatus = [PHPhotoLibrary authorizationStatus];
        __block bool ret = false;

        if (curStatus == PHAuthorizationStatusAuthorized){
            ret = true;
        }
        else if (curStatus == PHAuthorizationStatusRestricted ||
                 curStatus == PHAuthorizationStatusDenied) {
            ret = false;
        }
        else if(curStatus == PHAuthorizationStatusNotDetermined) {
            __block bool closed = false;

            [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                if (status == PHAuthorizationStatusAuthorized) {
                    ret = true;
                }

                closed = true;
            }];

            while (!closed) {
                usleep(100000);
            }
        }

        return  ret;
    }

    return false;
}

#pragma mark ---- ParentalGate Features ----
    
#ifdef ENABLE_FEATURE_PARENTAL_GATE
    void _cocoCommonShowParentalGate()
    {
        ParentalGate * parentalGate = [ParentalGate sharedInstance];
        [parentalGate paretalGateShowInView:UnityGetGLView()
                                  completed:^(BOOL passed) {
                                      NSLog(@"Parental Gate Verify Finished, passed:%d", (passed ? 1 : 0));
                                      UnitySendMessage("CocoCommonBinding", "ParentalGateVerifyFinished", passed ? "1" : "0");
                                  }];
        
    }
#endif
    
#ifdef __cplusplus
}
#endif

