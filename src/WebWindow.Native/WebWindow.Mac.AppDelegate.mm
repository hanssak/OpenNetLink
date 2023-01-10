#import "WebWindow.Mac.AppDelegate.h"
#import "DDHotKeyUtilities.h"
#import <Carbon/Carbon.h>
#import <FinderSync/FinderSync.h>
#import <Foundation/Foundation.h>
#import "Tray.h"

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
    NSLog(@"applicationDidFinishLaunching");
    NTLog(SelfThis, Info, "!! ----- aLaucgubg");
    // Show extensions, if FinderUtilities is not approved
    // if (!FIFinderSyncController.isExtensionEnabled) {
    //    [FIFinderSyncController showExtensionManagementInterface];
    //}

    //@autoreleasepool {
    //    while(tray_loop(1))
    //    {

    //    }
    //}
    [NSApp stop:nil];
}

- (BOOL)applicationShouldTerminateAfterLastWindowClosed:(NSApplication *)sender {
    NTLog(SelfThis, Info, "!! ----- application shoud terminater after last windowclosed");
    NSLog(@"When last windows Closed");
    return true;
}

- (NSApplicationTerminateReply)applicationShouldTerminate:(NSApplication *)sender {
    NSLog(@"applicationShouldTerminate");
    return NSTerminateNow;
}

- (void)applicationWillTerminate:(NSNotification *)notification {
    NSLog(@"applicationWillTerminate.......");
    NTLog(SelfThis, Info, "will terminate");
    NSString *strSocket = @"/var/tmp/testd.sock";
    NSFileManager *FileManager;
    FileManager = [NSFileManager defaultManager];
    if ([FileManager fileExistsAtPath:strSocket] == YES) {
        [FileManager removeFileAtPath:strSocket handler:nil];
        NSLog(@"delete file %@", strSocket);
    }
    NSLog(@"Will terminate....");
    return ;
}

- (BOOL)windowShouldClose:(NSWindow *)sender {

    NTLog(SelfThis, Info, "!! ----- windowshoudClose");
    
    NSLog(@"windowShouldClose....g_bDoExit2TrayUse(1):%s", (g_bDoExit2TrayUse?"Yes":"No"));
	if (g_bDoExit2TrayUse == false)
	{
        NSString *strSocket = @"/var/tmp/testd.sock";
        NSFileManager *FileManager;

        FileManager = [NSFileManager defaultManager];
        if ([FileManager fileExistsAtPath:strSocket] == YES) {
            [FileManager removeFileAtPath:strSocket handler:nil];
            NSLog(@"delete file %@", strSocket);
        }

        tray_exit();
        
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

-(void) ClipTypeSelect:(id)anObject
{
    NSNumber *nsNumbGuId = (NSNumber *)anObject;
    if(dicClipTypeSelect == nil)
    {
        //NSLog(@"dic nil!!!");
        dicClipTypeSelect = [[NSMutableDictionary alloc] init];
    }
    [dicClipTypeSelect setObject:@"1" forKey:nsNumbGuId];
    //NSLog(@"dicValue : %@", dicClipTypeSelect);
}

-(void) ClipFirstSendTypeText:(id)anObject
{
    NSNumber *nsNumbGuId = (NSNumber *)anObject;
    if(dicClipFirstSendTypeText == nil)
    {
        //NSLog(@"FirstSendTypeDic nil!!!");
        dicClipFirstSendTypeText = [[NSMutableDictionary alloc] init];
    }
    [dicClipFirstSendTypeText setObject:@"1" forKey:nsNumbGuId];
    //NSLog(@"dicValue : %@", dicClipFirstSendTypeText);
}

- (void) hotkeyClipBoardWithEvent:(NSEvent *)hkEvent object:(id)anObject
{
    NSNumber *nsNumbGuId = (NSNumber *)anObject;
    
    //NSLog(@"gCopyAndSend Value : %d", gCopyAndSend);
    if(gCopyAndSend == 1)
    {
        NSLog(@"Command + C Press!");
       [self hotkeyGenerate:'c' alt:false control:false shift:false win:true];
        usleep(1000000);
    }

    if(dicClipTypeSelect == nil)
    {
        dicClipTypeSelect = [[NSMutableDictionary alloc] init];
    }

    if(dicClipFirstSendTypeText == nil)
    {
        dicClipFirstSendTypeText = [[NSMutableDictionary alloc] init];
    }
    

    NSLog(@"Global HotKey Event Callback!, GUID:%d", [nsNumbGuId intValue]);
    dispatch_async(dispatch_get_main_queue(), ^{
        NSPasteboard *pasteBoard = NSPasteboard.generalPasteboard;
        BOOL isImage = [pasteBoard canReadObjectForClasses:@[[NSImage class]] options:nil];
        BOOL isText = [pasteBoard canReadObjectForClasses:@[[NSString class]] options:nil];
        int nDataType = D_CLIP_TEXT; //초기값 셋팅
        NSData *pClipData;
        if(isImage && isText)
        {
            if(dicClipTypeSelect == nil || [dicClipTypeSelect objectForKey:nsNumbGuId] == nil)
            {       
                if(dicClipFirstSendTypeText == nil || [dicClipFirstSendTypeText objectForKey:nsNumbGuId] == nil)
                {
                    nDataType = D_CLIP_IMAGE;
                }
                else
                {
                    nDataType = D_CLIP_TEXT;
                }
            }
            else
            {
                nDataType = D_CLIP_OBJECT;    
            }
        }
        else if(isImage)
        {
            nDataType = D_CLIP_IMAGE;
        }
        else if(isText)
        {
            nDataType = D_CLIP_TEXT;
        }
        else
        {
            nDataType = 0;
        }
        switch(nDataType)
        {
            case D_CLIP_IMAGE :
            {
                NSArray *array = [pasteBoard readObjectsForClasses:@[[NSImage class]] options:nil];
                NSImage *img = [array objectAtIndex:0];
                //[img lockFocus];
                //NSBitmapImageRep *rep = [[NSBitmapImageRep alloc] initWithFocusedViewRect:NSMakeRect(0, 0, img.size.width, img.size.height)];
                //[img unlockFocus];
                
                //[img addRepresentation:rep];
                //pClipData = [rep representationUsingType:NSBitmapImageFileTypeBMP properties:@{NSImageInterlaced: @NO}];
                //pClipData = [rep representationUsingType:NSBitmapImageFileTypeBMP properties:@{NSImageInterlaced: @NO}];
                pClipData = [img TIFFRepresentation];
                NSLog(@"copy image\n");
                ((WebWindow*)(SelfThis))->InvokeClipBoard([nsNumbGuId intValue], nDataType, pClipData.length, (const char*)[pClipData bytes], 0, nullptr); 
            }
            break;
            case D_CLIP_TEXT:
            {
                std::string strClipText;
                NSString *NSClipStr = nil;
                NSArray *array = [pasteBoard readObjectsForClasses:@[[NSString class]] options:nil];
                NSClipStr=[array objectAtIndex:0];
                NSLog(@"copy string\n%@", NSClipStr);

                strClipText = NSClipStr.UTF8String;
                
                ((WebWindow*)(SelfThis))->InvokeClipBoard([nsNumbGuId intValue], nDataType, strClipText.length(), strClipText.data(),0,nullptr);
            }
            break;
            case D_CLIP_OBJECT:
            {
                NSLog(@"image or text, object");                
                //TEXT IMAGE 둘 다 가능할 때 처리
                NSArray *array = [pasteBoard readObjectsForClasses:@[[NSImage class]] options:nil];
                NSImage *img = [array objectAtIndex:0];

                //[img lockFocus];
                //NSBitmapImageRep *rep = [[NSBitmapImageRep alloc] initWithFocusedViewRect:NSMakeRect(0, 0, img.size.width, img.size.height)];
                //[img unlockFocus];
                
                //[img addRepresentation:rep];
                //pClipData = [rep representationUsingType:NSBitmapImageFileType properties:@{NSImageInterlaced: @NO}];
                pClipData = [img TIFFRepresentation];
                NSLog(@"copy image\n");

                std::string strClipText;
                NSString *NSClipStr = nil;
                NSArray *arrayEx = [pasteBoard readObjectsForClasses:@[[NSString class]] options:nil];
                NSClipStr=[arrayEx objectAtIndex:0];
                NSLog(@"copy string\n%@", NSClipStr);

                strClipText = NSClipStr.UTF8String;

                ((WebWindow*)(SelfThis))->InvokeClipBoard([nsNumbGuId intValue], nDataType, pClipData.length, (const char*)[pClipData bytes], strClipText.length(), strClipText.data()); 
                //((WebWindow*)(SelfThis))->InvokeClipBoard([nsNumbGuId intValue], D_CLIP_TEXT, 0, nullptr, strClipText.length(), strClipText.data()); 
            }
            break;
            default:
            {
                NTLog(SelfThis, Warning, "클립보드는 이미지와 텍스트만 전송이 가능합니다. (Not Supported Clipboard Type)");
            }
            break;
        }
    });
}

- (void) hotkeyQuitWithEvent:(NSEvent *)hkEvent object:(id)anObject
{
    NSLog(@"Global HotKey Event Quit Callback!");
    ((WebWindow*)(SelfThis))->ProgramExit();
}

- (void) SetCopyAndSend:(bool)value
{
    NSLog(@"SetCopyAndSend");
    if(value)
        gCopyAndSend = 1;
    else
        gCopyAndSend = 0;

    //NSLog(@"gCopyAndSend Value Change : %d", gCopyAndSend);
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
