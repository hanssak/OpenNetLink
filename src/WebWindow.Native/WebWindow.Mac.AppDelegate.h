#import <Cocoa/Cocoa.h>
#include "WebWindow.h"
#include "NativeLog.h"

@interface MyApplicationDelegate : NSObject <NSApplicationDelegate, NSWindowDelegate, NSUserNotificationCenterDelegate> {
    NSWindow * window;

}

- (void) hotkeyGenerate:(char)chVKCode alt:(bool)bAlt control:(bool)bControl shift:(bool)bShift win:(bool)bWin;
@end
