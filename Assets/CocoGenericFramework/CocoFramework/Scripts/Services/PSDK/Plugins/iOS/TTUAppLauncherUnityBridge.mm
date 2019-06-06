#if TARGET_OS_IPHONE

#import "TTUAppLauncher.h"
#import <Foundation/Foundation.h>
#import <PublishingSDKCore/PublishingSDKCore.h>
#import <syslog.h>

#pragma mark Unity bridge

	NSString* CreateNSString (const char* string)
	 {
	   if (string)
		 return [NSString stringWithUTF8String: string];
	   else
			 return [NSString stringWithUTF8String: ""];
	 }
 
extern "C" {
    
	void openAppLauncher(const char *link, const char *appId, const char *storeId)
	{
	NSMutableDictionary *appToOpen = [NSMutableDictionary dictionary];
	[appToOpen setValue:CreateNSString(appId) forKey:@"appName"];
	[appToOpen setValue:CreateNSString(appId) forKey:@"appId"];
	[appToOpen setValue:CreateNSString(link) forKey:@"link"];
	[appToOpen setValue:CreateNSString(storeId) forKey:@"storeId"];
		
	[PSDKAppLauncherNew OpenApp:[appToOpen objectForKey:@"appName"] appUrl:[appToOpen objectForKey:@"link"] appId:[appToOpen objectForKey:@"appId"] storeId:[appToOpen objectForKey:@"storeId"]delegate:nil];
	
	NSLog(@"OpenAppLauncherNew call for appID: ",CreateNSString(appId));
	}
}
#endif