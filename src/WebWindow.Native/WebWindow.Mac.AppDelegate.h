#import <Cocoa/Cocoa.h>
#include "WebWindow.h"
#include "NativeLog.h"

@interface MyApplicationDelegate : NSObject <NSApplicationDelegate, NSWindowDelegate, NSUserNotificationCenterDelegate> {
    NSWindow * window;
}
@end
