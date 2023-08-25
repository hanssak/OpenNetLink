#import "WebWindow.Mac.UiDelegate.h"

@implementation MyUiDelegate : NSObject

- (void)userContentController:(WKUserContentController *)userContentController didReceiveScriptMessage:(WKScriptMessage *)message
{
    char *messageUtf8 = (char *)[message.body UTF8String];
    webMessageReceivedCallback(messageUtf8);
}

- (void)viewWillDisappear: (BOOL)animated {
    WKWebView *theWebview = (WKWebView *)webWindow->_webview;
    [theWebview.configuration.userContentController removeScriptMessageHandlerForName:@"webwindowinterop"];   
    NTLog(SelfThis, Info, "!! ----- Removed Script Message Handler : webwindowinterop");
}

- (void)webView:(WKWebView *)webView runJavaScriptAlertPanelWithMessage:(NSString *)message initiatedByFrame:(WKFrameInfo *)frame completionHandler:(void (^)(void))completionHandler
{
    NSAlert* alert = [[NSAlert alloc] init];

    [alert setMessageText:[NSString stringWithFormat:@"Alert: %@.", [frame.request.URL absoluteString]]];
    [alert setInformativeText:message];
    [alert addButtonWithTitle:@"OK"];

    [alert beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        completionHandler();
        [alert release];
    }];
}

- (void)webView:(WKWebView *)webView runJavaScriptConfirmPanelWithMessage:(NSString *)message initiatedByFrame:(WKFrameInfo *)frame completionHandler:(void (^)(BOOL result))completionHandler
{
    NSAlert* alert = [[NSAlert alloc] init];

    [alert setMessageText:[NSString stringWithFormat:@"Confirm: %@.", [frame.request.URL  absoluteString]]];
    [alert setInformativeText:message];
    
    [alert addButtonWithTitle:@"OK"];
    [alert addButtonWithTitle:@"Cancel"];

    [alert beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        completionHandler(response == NSAlertFirstButtonReturn);
        [alert release];
    }];
}

- (void)webView:(WKWebView *)webView runJavaScriptTextInputPanelWithPrompt:(NSString *)prompt defaultText:(NSString *)defaultText initiatedByFrame:(WKFrameInfo *)frame completionHandler:(void (^)(NSString *result))completionHandler
{
    NSAlert* alert = [[NSAlert alloc] init];

    [alert setMessageText:[NSString stringWithFormat:@"Prompt: %@.", [frame.request.URL absoluteString]]];
    [alert setInformativeText:prompt];
    
    [alert addButtonWithTitle:@"OK"];
    [alert addButtonWithTitle:@"Cancel"];
    
    NSTextField* input = [[NSTextField alloc] initWithFrame:NSMakeRect(0, 0, 200, 24)];
    [input setStringValue:defaultText];
    [alert setAccessoryView:input];
    
    [alert beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        [input validateEditing];
        completionHandler(response == NSAlertFirstButtonReturn ? [input stringValue] : nil);
        [alert release];
    }];
}

-(void)webView:(WKWebView *)webView didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge completionHandler:(void (^)(NSURLSessionAuthChallengeDisposition, NSURLCredential * _Nullable))completionHandler
{
    if ([[[challenge protectionSpace]authenticationMethod] isEqualToString: @"NSURLAuthenticationMethodServerTrust"]) {
        SecTrustRef serverTrust = challenge.protectionSpace.serverTrust;
        CFDataRef exceptions = SecTrustCopyExceptions(serverTrust);
        SecTrustSetExceptions(serverTrust, exceptions);
        CFRelease(exceptions);
        NSURLCredential * newCredential = [NSURLCredential credentialForTrust:serverTrust];
        completionHandler(NSURLSessionAuthChallengeUseCredential, newCredential);
    } else {
        NSURLCredential * newCredential = [[NSURLCredential alloc] initWithTrust:[challenge protectionSpace].serverTrust];
        completionHandler(NSURLSessionAuthChallengePerformDefaultHandling, newCredential);
        [newCredential release];
    }
}

- (void)windowDidResize:(NSNotification *)notification {
    int width, height;
    webWindow->GetSize(&width, &height);
    webWindow->InvokeResized(width, height);
}

- (void)windowDidMove:(NSNotification *)notification {
    int x, y;
    webWindow->GetPosition(&x, &y);
    webWindow->InvokeMoved(x, y);
}
- (void) webView: (WKWebView *) webView didStartProvisionalNavigation: (WKNavigation *) navigation {
    NSString *myString = webView.URL.absoluteString;
    std::string strUriText;
    strUriText = myString.UTF8String;
    
    ((WebWindow*)(SelfThis))->InvokeURLChangedCallback(strUriText.data(), strUriText.length());
}
//InputTrans에 DragNDrop을 할 수 있어서 url이 변경될 때 페이지 변환 차단
- (void)webView:(WKWebView *)webView decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler {
	NSString *myString = navigationAction.request.URL.absoluteString;
    NSRange rangeString = [myString rangeOfString:@"file://"];
    NSInteger index = rangeString.location;
    if((int)index == 0)
    {
        //std::string strUriText;
        //strUriText = myString.UTF8String;
        //((WebWindow*)(SelfThis))->InvokeDragNDropChangedCallback(strUriText.data(), strUriText.length());

        decisionHandler(WKNavigationActionPolicyCancel);
    }
    else
    {
        decisionHandler(WKNavigationActionPolicyAllow);
    }
    NSLog(@"%@\n", myString);
}
@end
