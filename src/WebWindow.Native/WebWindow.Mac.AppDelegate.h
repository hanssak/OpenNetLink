#import <Cocoa/Cocoa.h>
#import <Foundation/Foundation.h>
#include "WebWindow.h"
#include "NativeLog.h"

@interface MyApplicationDelegate : NSObject <NSApplicationDelegate, NSWindowDelegate, NSUserNotificationCenterDelegate> {
    NSWindow * window;
    int gCopyAndSend;
    int gPasteHotKey;
    int gPasteGroupID;
    NSMutableDictionary *dicClipTypeSelect;
    NSMutableDictionary *dicClipFirstSendTypeText;
}

- (void) hotkeyClipBoardWithEvent:(NSEvent *)hkEvent object:(id)anObject; 
- (void) hotkeyGenerate:(char)chVKCode alt:(bool)bAlt control:(bool)bControl shift:(bool)bShift win:(bool)bWin;
- (void) SetCopyAndSend:(bool)value;
- (void) SetPasteHotKey:(bool)value object:(id)anObject;
- (void) ClipTypeSelect:(id)anObject;
- (void) ClipFirstSendTypeText:(id)anObject;
@end
