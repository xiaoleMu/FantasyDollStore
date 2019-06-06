//
//  FPMainViewController.h
//  ForParents
//
//  Created by cyher on 13-11-25.
//  Copyright (c) 2013年 cocoplay. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "FPUpTabBarView.h"

@interface FPMainViewController : UIViewController <FPUpTabBarViewDelegate>
- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil appStoreId:(NSInteger)appId;
@end
