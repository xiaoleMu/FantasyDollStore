#import <Foundation/Foundation.h>
#import "TTUPsdkUtils.h"
#import <syslog.h>



#import <PublishingSDKCore/PublishingSDKCore.h>


@implementation TTUPsdkUtils

#pragma mark Unity bridge

// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.

extern "C" {
    
    
    void psdkRemoveUserDefaultValue(const char *key) {
        [[NSUserDefaults standardUserDefaults] removeObjectForKey:[[NSString alloc] initWithUTF8String:key] ];
    }
    
    
    BOOL psdkUnityUnzip(char *zippedFilePath, char *destinationPath) {
        
        PSDKZipArchive *zipArchive = [[PSDKZipArchive alloc] init];
        NSString *srcZip =[[NSString alloc] initWithUTF8String:zippedFilePath];
        NSString *destFolder = [[NSString alloc] initWithUTF8String:destinationPath];
        if ([zipArchive UnzipOpenFile: srcZip]) {
            if(![zipArchive UnzipFileTo: destFolder overWrite: YES]){
                NSLog(@"psdkUnityUnzip:: Failed unzipping %@ to %@", srcZip, destFolder);
                return NO;
            }
        }
        else {
            NSLog(@"psdkUnityUnzip:: Failed openning %@", srcZip);
            return NO;
        }
        return YES;
    }
   
    const char* psdkUtilsGetCacheDir() {
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES);
        NSString *cachePath = [paths objectAtIndex:0];
        BOOL isDir = NO;
        NSError *error;
        if (! [[NSFileManager defaultManager] fileExistsAtPath:cachePath isDirectory:&isDir] && isDir == NO) {
            [[NSFileManager defaultManager] createDirectoryAtPath:cachePath withIntermediateDirectories:NO attributes:nil error:&error];
        }
        return strdup([cachePath UTF8String]);
    }
    
    const char * psdkUtilsGetBundleVersion() {
        //NSString *version = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"];
        NSString *build = [[[NSBundle mainBundle] infoDictionary] objectForKey:(NSString *)kCFBundleVersionKey];
        NSLog(@"native bundle version:%@",build);
        return strdup([build UTF8String]);
    }
    
    const char * psdkUtilsGetBundleIdentifier() {
        NSString *identifier = [[NSBundle mainBundle] bundleIdentifier];
        NSLog(@"native bundle identifier:%@",identifier);
        return strdup([identifier UTF8String]);
    }
    
   

    void psdkUtilsSysLog(const char* logLine) {
        openlog("UPTAUI", (LOG_CONS|LOG_PERROR|LOG_PID), LOG_USER);
        syslog(4, "%s", logLine);
        closelog();
    }

    void psdkUtilsNativeLog(const char* logLine) {
        psdkUtilsSysLog(logLine);
        NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];

        if ([[vComp objectAtIndex:0] intValue] == 7) {
            NSLog(@"%@",[[NSString alloc] initWithUTF8String:logLine]);
        } else {
        }
    }


    
}

@end

