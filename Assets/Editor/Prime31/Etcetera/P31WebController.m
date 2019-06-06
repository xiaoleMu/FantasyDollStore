    //
//  P31WebController.m
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "P31WebController.h"
#import "EtceteraManager.h"


@implementation P31WebController

@synthesize webView = _webView, url = _url, backButton, forwardButton;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

- (id)initWithUrl:(NSString*)url showControls:(BOOL)showControls
{
	if( ( self = [super initWithNibName:nil bundle:nil] ) )
	{
		self.url = url;
		_showToolbar = showControls;
	}
	return self;
}



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - Private

- (void)onTouchBack
{
	[_webView goBack];
}


- (void)onTouchForward
{
	[_webView goForward];
}


- (void)onTouchAction
{
	UIActionSheet *sheet = [[UIActionSheet alloc] initWithTitle:[_webView.request.mainDocumentURL absoluteString]
														delegate:self
											   cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
										  destructiveButtonTitle:nil
											   otherButtonTitles:NSLocalizedString( @"Open in Safari", nil ), NSLocalizedString( @"Copy URL", nil ), NSLocalizedString( @"Print", nil ), nil];
	[sheet showFromToolbar:self.navigationController.toolbar];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - UIActionSheetDelegate

- (void)actionSheet:(UIActionSheet*)actionSheet clickedButtonAtIndex:(NSInteger)buttonIndex
{
	if( buttonIndex == 0 )
	{
		[[UIApplication sharedApplication] openURL:_webView.request.mainDocumentURL];
	}
	else if( buttonIndex == 1 )
	{
		[[UIPasteboard generalPasteboard] setURL:_webView.request.mainDocumentURL];
	}
	else if( buttonIndex == 2 )
	{
		if( [UIPrintInteractionController isPrintingAvailable] )
		{
			UIPrintInteractionController* pic = [UIPrintInteractionController sharedPrintController];
			
			// setup print info
			UIPrintInfo* printInfo = [UIPrintInfo printInfo];
			printInfo.outputType = UIPrintInfoOutputGeneral;
			pic.printInfo = printInfo;
			
			pic.printFormatter = self.webView.viewPrintFormatter;
			[pic presentAnimated:NO completionHandler:nil];
		}
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIViewController

- (BOOL)shouldAutorotate
{
	return YES;
}


- (UIInterfaceOrientationMask)supportedInterfaceOrientations
{
	return (UIInterfaceOrientationMask)[[UIApplication sharedApplication].keyWindow.rootViewController supportedInterfaceOrientations];
}


- (void)loadView
{
	[super loadView];
	
	// setup the toolbar if needed
	if( _showToolbar )
	{
		UIImage *backImage = [UIImage imageNamed:@"back"];
		backButton = [[UIBarButtonItem alloc] initWithImage:backImage
													  style:UIBarButtonItemStylePlain
													 target:self
													 action:@selector(onTouchBack)];
		
		UIImage *forwardImage = [UIImage imageNamed:@"forward"];
		forwardButton = [[UIBarButtonItem alloc] initWithImage:forwardImage
														 style:UIBarButtonItemStylePlain
														target:self
														action:@selector(onTouchForward)];
		UIBarButtonItem *action = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemAction
																				  target:self
																				  action:@selector(onTouchAction)];
		
		
		UIBarButtonItem *fixedSpace = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFixedSpace
																						target:nil
																						action:nil];
		fixedSpace.width = 45;
		
		UIBarButtonItem *flexibleSpace = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace
																				  target:nil
																				  action:nil];
		
		self.toolbarItems = [NSArray arrayWithObjects:backButton, fixedSpace, forwardButton, flexibleSpace, action, nil];
		self.navigationController.toolbarHidden = NO;
	}
	
	
	// add a done button
	self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone
																							target:self
																							action:@selector(onTouchDone)];
	
	// create the UIWebView and show the url
	CGRect frame = [UIApplication sharedApplication].keyWindow.bounds;
	_webView = [[UIWebView alloc] initWithFrame:frame];
	_webView.delegate = self;
	_webView.autoresizingMask = UIViewAutoresizingFlexibleWidth	| UIViewAutoresizingFlexibleHeight;
	_webView.scalesPageToFit = YES;
	[self.view addSubview:_webView];
	
	// the url could be absolute or local so check and load appropriately
	NSURL *url;
	if( ![_url hasPrefix:@"http:"] && [[NSFileManager defaultManager] fileExistsAtPath:_url] )
	{
		url = [NSURL fileURLWithPath:_url];
	}
	else
	{
		url = [NSURL URLWithString:_url];
	}
	
	NSURLRequest *request = [NSURLRequest requestWithURL:url];
	[_webView loadRequest:request];
}


- (void)onTouchDone
{
	// dismiss
	[[EtceteraManager sharedManager] dismissWrappedController];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - UIWebViewDelegate

- (BOOL)webView:(UIWebView*)webView shouldStartLoadWithRequest:(NSURLRequest*)request navigationType:(UIWebViewNavigationType)navigationType
{
	if( _showToolbar )
	{
		backButton.enabled = [_webView canGoBack];
		forwardButton.enabled = [_webView canGoForward];
	}
	
    // we only handle http, https and file with the webview
    NSURL *url = request.URL;
    if( ![url.scheme isEqual:@"http"] && ![url.scheme isEqual:@"https"] && ![url.scheme isEqual:@"file"] )
	{
        if( [[UIApplication sharedApplication]canOpenURL:url] )
		{
			[webView stopLoading];
            [[UIApplication sharedApplication] openURL:url];
			[self onTouchDone];
			
            return NO;
        }
    }
	return YES;
}


- (void)webViewDidStartLoad:(UIWebView*)webView
{
	self.title = NSLocalizedString( @"Loading...", @"" );
	
	if( _showToolbar )
	{
		backButton.enabled = [_webView canGoBack];
		forwardButton.enabled = [_webView canGoForward];
	}
}


- (void)webViewDidFinishLoad:(UIWebView*)webView
{
	if( webView.request.URL )
		[EtceteraManager sendMessage:@"webViewDidLoadURL" param:webView.request.URL.absoluteString];
	self.title = [_webView stringByEvaluatingJavaScriptFromString:@"document.title"];
	
	if( _showToolbar )
	{
		backButton.enabled = [_webView canGoBack];
		forwardButton.enabled = [_webView canGoForward];
	}
}


- (void)webView:(UIWebView*)webView didFailLoadWithError:(NSError*)error
{
	[self webViewDidFinishLoad:webView];
}

@end
