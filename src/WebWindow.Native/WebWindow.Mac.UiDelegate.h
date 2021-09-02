#import <Cocoa/Cocoa.h>
#import <WebKit/WebKit.h>
#include "WebWindow.h"
#include "NativeLog.h"

typedef void (*WebMessageReceivedCallback) (char* message);

@interface MyUiDelegate : NSObject <WKUIDelegate, WKScriptMessageHandler, WKNavigationDelegate> {
    @public
    NSWindow * window;
    WebWindow * webWindow;
    WebMessageReceivedCallback webMessageReceivedCallback;
}
@end
