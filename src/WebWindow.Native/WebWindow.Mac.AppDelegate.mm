#import "WebWindow.Mac.AppDelegate.h"
#import "DDHotKeyUtilities.h"
#import <Carbon/Carbon.h>
#import <FinderSync/FinderSync.h>

@implementation MyApplicationDelegate : NSObject
- (id)init {
    if (self = [super init]) {
        // allocate and initialize window and stuff here ..
    }

    return self;
}

- (void)applicationDidFinishLaunching:(NSNotification *)notification {
    [window makeKeyAndOrderFront:nil];
    [NSApp activateIgnoringOtherApps:YES];

    // Show extensions, if FinderUtilities is not approved
    if (!FIFinderSyncController.isExtensionEnabled) {
        [FIFinderSyncController showExtensionManagementInterface];
    }
}

- (BOOL)applicationShouldTerminateAfterLastWindowClosed:(NSApplication *)sender {
    NSLog(@"When last windows Closed");
    return true;
}

- (void)applicationWillTerminate:(NSNotification *)notification {
    NSLog(@"Will terminate....");
    return ;
}

- (BOOL)windowShouldClose:(NSWindow *)sender {
    NSLog(@"windowShouldClose....");
	if (_bTrayUse == false)
	{
   		((WebWindow *)SelfThis)->ProgramExit();
        return true;
	}
	else
	{
   		((WebWindow *)SelfThis)->MoveWebWindowToTray();
        return false;
	}

    return true;
}

- (void)windowWillMiniaturize:(NSNotification *)notification {
    NSLog(@"Will window minimize....");
    ((WebWindow *)SelfThis)->MinimizeWebWindow();
    return ;
}

- (void)dealloc {
    [window release];
    [super dealloc];
}

- (BOOL)userNotificationCenter:(NSUserNotificationCenter *)center
     shouldPresentNotification:(NSUserNotification *)notification
{
    NSLog(@"Notification Presented");
    return YES;
}

- (void)userNotificationCenter:(NSUserNotificationCenter *)center
     didActivateNotification:(NSUserNotification *)notification
{
    NSLog(@"Button Actioned");
    NSDictionary *userInfo = notification.userInfo;

	const char *navURI = [userInfo[@"NAVIGATE_URI"] UTF8String];
	NTLog(SelfThis, Info, "Called : Action Navigate URI->(%s)", (AutoString)navURI);
	((WebWindow*)SelfThis)->InvokeRequestedNavigateURL((AutoString)navURI);
    return ;
}

- (void) hotkeyClipBoardWithEvent:(NSEvent *)hkEvent object:(id)anObject
{
    NSNumber *nsNumbGuId = (NSNumber *)anObject;

    NSLog(@"Global HotKey Event Callback!, GUID:%d", [nsNumbGuId intValue]);

    dispatch_async(dispatch_get_main_queue(), ^{
        NSPasteboard *pasteBoard = NSPasteboard.generalPasteboard;
        NSArray *array = [pasteBoard readObjectsForClasses:@[[NSImage class], [NSString class]] options:nil];
        NSImage *img = nil;
        for (NSInteger i=0; i<array.count; i++) {
            if ([[array objectAtIndex:i] isKindOfClass:[NSImage class]]) {
                img = [array objectAtIndex:i];
                break;
            }
        }
        
        NSData *pClipData;
        std::string strClipText;
        int nDataType = D_CLIP_TEXT;
        __block BOOL bClipSend=YES;

        if (img != nil) {   // 이미지 복사
            [img lockFocus];
            NSBitmapImageRep *rep = [[NSBitmapImageRep alloc] initWithFocusedViewRect:NSMakeRect(0, 0, img.size.width, img.size.height)];
            [img unlockFocus];
            
            [img addRepresentation:rep];
            
            pClipData = [rep representationUsingType:NSBitmapImageFileTypeBMP properties:@{NSImageInterlaced: @NO}];
            // set image type
            nDataType = D_CLIP_IMAGE;
            NSLog(@"copy image\n");
        }
        else
        {
            if(array.count>0)
            {             // 문자열 복사
                NSString *NSClipStr = nil;
                NSClipStr=[array objectAtIndex:0];
                NSLog(@"copy string\n%@", NSClipStr);

                strClipText = NSClipStr.UTF8String;
                nDataType = D_CLIP_TEXT;
            }
            else
            {
                NTLog(SelfThis, Warning, "클립보드는 이미지와 텍스트만 전송이 가능합니다. (Not Supported Clipboard Type)");
                bClipSend=NO;
            }
        }

        if(bClipSend)
        {
            if( nDataType == D_CLIP_TEXT) ((WebWindow*)(SelfThis))->InvokeClipBoard([nsNumbGuId intValue], nDataType, strClipText.length(), strClipText.data());
            else ((WebWindow*)(SelfThis))->InvokeClipBoard([nsNumbGuId intValue], nDataType, pClipData.length, (const char*)[pClipData bytes]);
        }
    });
}

- (void) hotkeyQuitWithEvent:(NSEvent *)hkEvent object:(id)anObject
{
    NSLog(@"Global HotKey Event Quit Callback!");
    ((WebWindow*)(SelfThis))->ProgramExit();
}

- (void) hotkeyGenerate:(char)chVKCode alt:(bool)bAlt control:(bool)bControl shift:(bool)bShift win:(bool)bWin
{
    NSLog(@"generate hotkey, alt:%d control:%d shift:%d win:%d keycode:%c", bAlt, bControl, bShift, bWin, chVKCode);
    CGEventSourceRef src = CGEventSourceCreate(kCGEventSourceStateHIDSystemState);
    CGEventRef keyCodeDown, keyCodeUp;

    // Init
    unsigned short usKeyCode = GetCharKeyCode([NSString stringWithFormat:@"%c" , chVKCode]);
    keyCodeDown = CGEventCreateKeyboardEvent(src, (CGKeyCode)usKeyCode, true);
    keyCodeUp   = CGEventCreateKeyboardEvent(src, (CGKeyCode)usKeyCode, false);
    
    CGEventFlags eventFlags = 0;
    if(bAlt)        eventFlags |= kCGEventFlagMaskAlternate;
    if(bControl)    eventFlags |= kCGEventFlagMaskControl;
    if(bShift)      eventFlags |= kCGEventFlagMaskShift;
    if(bWin)        eventFlags |= kCGEventFlagMaskCommand;

    CGEventSetFlags(keyCodeDown, eventFlags);
    CGEventSetFlags(keyCodeUp, eventFlags);

    CGEventTapLocation loc = kCGHIDEventTap; // kCGSessionEventTap also works

    // Key Action 
    CGEventPost(loc, keyCodeDown);
    CGEventPost(loc, keyCodeUp);

    // Releases 
    CFRelease(keyCodeDown);
    CFRelease(keyCodeUp);
    CFRelease(src);

    return ;
}
@end
