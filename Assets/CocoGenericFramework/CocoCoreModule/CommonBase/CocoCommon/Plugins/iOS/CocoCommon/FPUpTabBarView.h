//
//  FPUpTabBarView.h
//  ForParents
//
//  Created by cyher on 13-11-26.
//  Copyright (c) 2013年 cocoplay. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol FPUpTabBarViewDelegate <NSObject>
@required
- (void)FPbackButtonPressed:(id)sender;
- (void)FPtabButtonPressed:(id)sender;
@end

@interface FPUpTabBarView : UIView

@property (nonatomic, weak) id <FPUpTabBarViewDelegate> delegate;
@end
