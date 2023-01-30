#ifdef OS_MAC
#include "WebWindow.h"
#import "WebWindow.Mac.AppDelegate.h"
#import "WebWindow.Mac.UiDelegate.h"
#import "WebWindow.Mac.UrlSchemeHandler.h"
#import "DDHotKeyCenter.h"
#import "DDHotKeyUtilities.h"
#import "FinderSync.h"
#include <cstdio>
#include <thread>
#include <map>
#import <Cocoa/Cocoa.h>
#import <WebKit/WebKit.h>
#import <Carbon/Carbon.h>

void *SelfThis = nullptr;

#include "NativeLog.h"
#include "TrayFunc.h"

#include "TextEncDetect.h"

#include <sys/socket.h>
#include <sys/un.h>
//#include <sys/types.h>
#include <sys/stat.h>
#include <sys/socket.h>
#include <sys/un.h>
//#include <unistd.h>
//#include <stdio.h>
//#include <stdlib.h>
//#include <string.h>
//#include <fcntl.h>

using namespace std;

bool g_bStartTray = true;
bool g_bDoExit2TrayUse = false;
bool g_bClipCopyNsend = false;
static id _appDelegate;

map<NSWindow*, WebWindow*> nsWindowToWebWindow;

void WebWindow::Register()
{
    [NSAutoreleasePool new];
    [NSApplication sharedApplication];
    [NSApp setActivationPolicy:NSApplicationActivationPolicyRegular];
    id menubar = [[NSMenu new] autorelease];
    id appMenuItem = [[NSMenuItem new] autorelease];
    [menubar addItem:appMenuItem];
    [NSApp setMainMenu:menubar];
    id appMenu = [[NSMenu new] autorelease];
    id appName = [[NSProcessInfo processInfo] processName];
    id quitTitle = [@"Quit " stringByAppendingString:appName];
    id quitMenuItem = [[[NSMenuItem alloc] initWithTitle:quitTitle
        action:@selector(terminate:) keyEquivalent:@"q"] autorelease];
    [appMenu addItem:quitMenuItem];
    [appMenuItem setSubmenu:appMenu];

    NSBundle *nsBundle = [NSBundle mainBundle];
    NSString *strBundle = [nsBundle bundlePath];
    NSString *strResource = [nsBundle resourcePath];
    NSString *strExecutable = [nsBundle executablePath];

    NSString *infoPath = [nsBundle pathForResource:@"info" ofType:@"plist"];
    NSString *bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
    NSLog(@"info.plist: %@", infoPath);
    NSLog(@"BundlePath: %@", strBundle );
    NSLog(@"ResourcePath: %@", strResource );
    NSLog(@"ExecPath: %@", strExecutable );
    NSLog(@"BundleIdentifier: %@", bundleIdentifier );

    MyApplicationDelegate * appDelegate = [[[MyApplicationDelegate alloc] init] autorelease];
    NSApplication * application = [NSApplication sharedApplication];
    [application setDelegate:appDelegate];

    [[NSUserNotificationCenter defaultUserNotificationCenter] setDelegate:appDelegate];
    _appDelegate = appDelegate;
}

/*
static struct tray defTray = {
    .icon = (char *)TRAY_ICON1,
    .dark_icon = (char *)TRAY_ICON3,
    .menu = (struct tray_menu[]) {
            {.text = (char *)"Hide",    .disabled = 0, .checked = 0, .usedCheck = 0, .cb = toggle_show, .context = NULL, .submenu = NULL},
            {.text = (char *)"-",       .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL, .submenu = NULL},
            {.text = (char *)"Quit",    .disabled = 0, .checked = 0, .usedCheck = 0, .cb = quit_cb, .context = NULL, .submenu = NULL},
            {.text = NULL,              .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL, .submenu = NULL}
    }
};
*/

WebWindow::WebWindow(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback)
{
	SelfThis = this;
	_webMessageReceivedCallback = webMessageReceivedCallback;
	g_bDoExit2TrayUse = false;
    
    NSRect frame = NSMakeRect(0, 0, WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT);
    NSWindow *window = [[NSWindow alloc]
        initWithContentRect:frame
        styleMask:NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskResizable | NSWindowStyleMaskMiniaturizable
        backing: NSBackingStoreBuffered
        defer: false];
    _window = window;
	_g_window = window;
    [window setDelegate:_appDelegate];

    [window cascadeTopLeftFromPoint:NSMakePoint(20,20)];
    SetTitle(title);

    WKWebViewConfiguration *webViewConfiguration = [[WKWebViewConfiguration alloc] init];
    [webViewConfiguration.preferences setValue:@YES forKey:@"developerExtrasEnabled"];
    // Await contents data(JS and so on...) still load to memory. --> Experimental
    // webViewConfiguration.suppressesIncrementalRendering = YES;
    _webviewConfiguration = webViewConfiguration;
    _webview = nil;
}

WebWindow::~WebWindow()
{
    WKWebViewConfiguration *webViewConfiguration = (WKWebViewConfiguration*)_webviewConfiguration;
    [webViewConfiguration release];
    WKWebView *webView = (WKWebView*)_webview;
    [webView release];
    NSWindow* window = (NSWindow*)_window;
    [window close];
}

void WebWindow::AttachWebView()
{
    MyUiDelegate *uiDelegate = [[[MyUiDelegate alloc] init] autorelease];
    uiDelegate->webWindow = this;

    NSString *initScriptSource = @"window.__receiveMessageCallbacks = [];"
			"window.__dispatchMessageCallback = function(message) {"
			"	window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });"
			"};"
			"window.external = {"
			"	sendMessage: function(message) {"
			"		window.webkit.messageHandlers.webwindowinterop.postMessage(message);"
			"	},"
			"	receiveMessage: function(callback) {"
			"		window.__receiveMessageCallbacks.push(callback);"
			"	}"
			"};";
    WKUserScript *initScript = [[WKUserScript alloc] initWithSource:initScriptSource injectionTime:WKUserScriptInjectionTimeAtDocumentStart forMainFrameOnly:YES];
    WKUserContentController *userContentController = [WKUserContentController new];
    WKWebViewConfiguration *webviewConfiguration = (WKWebViewConfiguration *)_webviewConfiguration;
    webviewConfiguration.userContentController = userContentController;
    [userContentController addUserScript:initScript];

    NSWindow *window = (NSWindow*)_window;
    WKWebView *webView = [[WKWebView alloc] initWithFrame:window.contentView.frame configuration:webviewConfiguration];
    [webView setAutoresizingMask:NSViewWidthSizable | NSViewHeightSizable];
    [window.contentView addSubview:webView];
    [window.contentView setAutoresizesSubviews:YES];

    uiDelegate->window = window;
    webView.UIDelegate = uiDelegate;
    webView.navigationDelegate = uiDelegate;

    uiDelegate->webMessageReceivedCallback = _webMessageReceivedCallback;
    [userContentController addScriptMessageHandler:uiDelegate name:@"webwindowinterop"];

    // TODO: Remove these observers when the window is closed
    [[NSNotificationCenter defaultCenter] addObserver:uiDelegate selector:@selector(windowDidResize:) name:NSWindowDidResizeNotification object:window];
    [[NSNotificationCenter defaultCenter] addObserver:uiDelegate selector:@selector(windowDidMove:) name:NSWindowDidMoveNotification object:window];

    _webview = webView;
}

void WebWindow::Show()
{
    if (_webview == nil) {
        AttachWebView();
    }

    if(!g_bStartTray)
    {
        NSWindow * window = (NSWindow*)_window;
        [window makeKeyAndOrderFront:nil];
    }

}

void WebWindow::SetTitle(AutoString title)
{
    NSWindow* window = (NSWindow*)_window;
    NSString* nstitle = [[NSString stringWithUTF8String:title] autorelease];
    [window setTitle:nstitle];
}

void WebWindow::WaitForExit()
{
    FinderSyncExtensionHelper Helper;
    Helper.reinstall(false);

    // ThrMouseRightClick: This way is Thread Run of Objective-c ARC Style
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        /*
            < For Mouse Right Click :  Add File List To Transfer >
            /Users/angellos/Library/Containers/com.hanssak.OpenNetLinkApp.SGFinderSync/Data/RList.txt
            angellos@minmyeonghun-ui-Macmini Data % cat RList.txt 
            0
            /Users/angellos/Desktop/맥오픈넷링크-다크.png
            /Users/angellos/Desktop/맥오픈넷링크-라이트.png
            /Users/angellos/Desktop/맥오픈넷링크-다이나믹.png
            /Users/angellos/WorkSpace/MacFinderSync/OpenNetLinkFinderSync(gips)/SGFinderSync/FinderSync.m
            /Users/angellos/Library/Mobile Documents/com~apple~TextEdit/Documents/무제.rtf
            /Users/angellos/WorkSpace/MacFinderSync/OpenNetLinkFinderSync(gips)/Gips/AppDelegate.m
            /Users/angellos/WorkSpace/MacFinderSync/OpenNetLinkFinderSync(gips)/Gips/ViewController.m
            /Users/angellos/WorkSpace/MacFinderSync/OpenNetLinkFinderSync(gips)/Gips/main.m
            /Users/angellos/WorkSpace/MacFinderSync/OpenNetLinkFinderSync(gips)/SGFinderSync/SGFinderSync.entitlements
            and ..
            unlink("/Users/angellos/Library/Containers/com.hanssak.OpenNetLinkApp.SGFinderSync/Data/RList.txt")
        */
        int myuid;
        passwd *mypasswd;
        myuid = getuid();
        mypasswd = getpwuid(myuid);
        struct stat buffer;   
        std::string filePath = std::string(mypasswd->pw_dir) + "/Library/Containers/com.hanssak.OpenNetLinkApp.SGFinderSync/Data/RList.txt";

        unlink(filePath.c_str());
        while(true) {
            if (stat (filePath.c_str(), &buffer) == 0) {
                int nOffset = 0;
                char *pCmdBuf = (char *)malloc(buffer.st_size);
                if (ReadFileAndSaveForContextualTransfer((AutoString)filePath.c_str(), (AutoString)pCmdBuf, buffer.st_size) != nullptr) {
                    char szCmdGuId[8];
                    for(int i_for = 0; i_for < sizeof(szCmdGuId); i_for++) {
                        if(pCmdBuf[i_for+1] == '\r' && pCmdBuf[i_for+2] == '\n') {
                            szCmdGuId[i_for] = pCmdBuf[i_for];
                            szCmdGuId[i_for+1] = '\r';
                            szCmdGuId[i_for+2] = '\n';
                            nOffset += 3;
                        } else {
                            szCmdGuId[i_for] = pCmdBuf[i_for];
                            nOffset++;
                        }
                    }
                    if(ContextualTransferClient(szCmdGuId, nOffset) == 0) unlink(filePath.c_str());
                }
                if (pCmdBuf) free(pCmdBuf);
            }
            sleep(1);
        }
    }); // ThrMouseRightClick: Thread Run
    [NSApp run];
	if (tray_init(&tray) < 0)
	{
		// printf("failed to create tray\n");
		NTLog(this, Fatal, "Failed to Create Tray\n");
		return ;
	}

    if(!g_bStartTray)
        MoveTrayToWebWindow();

    RegisterQuitHotKey();
	while (tray_loop(1) == 0)
	{
		// printf("iteration\n");
	}
}

AutoString WebWindow::ReadFileAndSaveForContextualTransfer(AutoString strPath, AutoString pCmdBuf, int nSize)
{
#define CONTEXT_LIST_FILE "/var/tmp/sgateContext.info"
    int result = 0;

    FILE *pRFile = fopen(strPath, "r");
    result = fread (pCmdBuf, 1, nSize, pRFile);
    if(result == nSize) {
        FILE *pWFile = fopen(CONTEXT_LIST_FILE, "w");
        result = fwrite (pCmdBuf, 1, nSize, pWFile);
        if (result == nSize) {
            fclose(pRFile);
            fclose(pWFile);
            return pCmdBuf;
        }
        if (pWFile) fclose(pWFile);
    }

    if (pRFile) fclose(pRFile);

    return nullptr;
}

int WebWindow::ContextualTransferClient(AutoString pCmdGuId, int nSize)
{
#define SERVER_SOCK_FILE "/var/tmp/testd.sock"

    int sockfd;
    struct sockaddr_un serveraddr;
    int  clilen;

    sockfd = socket(AF_UNIX, SOCK_STREAM, 0); 
    if (sockfd < 0)
    {
        NTLog(this, Info, "Error : Create Unix Domain Socket for Contaxtual File Transfer. (%s)", strerror(errno));
        return -1;
    }

    bzero(&serveraddr, sizeof(serveraddr));
    serveraddr.sun_family = AF_UNIX;
    strcpy(serveraddr.sun_path, SERVER_SOCK_FILE);
    clilen = sizeof(serveraddr);

    int rc = connect(sockfd, (struct sockaddr *) &serveraddr, clilen);
    if(rc == -1){
        NTLog(this, Info, "Error : Connect Unix Domain Socket for Contaxtual File Transfer. (%s)", strerror(errno));
        close(sockfd);
        return -2;
    }

    if (sendto(sockfd, (void *)pCmdGuId, nSize, 0, (struct sockaddr *)&serveraddr, clilen) < 0)
    {
        NTLog(this, Info, "Error : Send to Unix Domain Socket for Contaxtual File Transfer. (%s)", strerror(errno));
        close(sockfd);
        return -3;
    }

    close(sockfd);
    return 0;
}

void WebWindow::Invoke(ACTION callback)
{
    dispatch_sync(dispatch_get_main_queue(), ^(void){
        callback();
    });
}

void WebWindow::ShowMessage(AutoString title, AutoString body, unsigned int type)
{
    return;

    // code 검증필요
    NSString* nstitle = [[NSString stringWithUTF8String:title] autorelease];
    NSString* nsbody= [[NSString stringWithUTF8String:body] autorelease];
    NSAlert *alert = [[[NSAlert alloc] init] autorelease];
    [[alert window] setTitle:nstitle];
    [alert setMessageText:nsbody];
    [alert runModal];
}

void WebWindow::NavigateToString(AutoString content)
{
    WKWebView *webView = (WKWebView *)_webview;
    NSString* nscontent = [[NSString stringWithUTF8String:content] autorelease];
    [webView loadHTMLString:nscontent baseURL:nil];
}

void WebWindow::NavigateToUrl(AutoString url)
{
    WKWebView *webView = (WKWebView *)_webview;
    NSString* nsurlstring = [[NSString stringWithUTF8String:url] autorelease];
    NSURL *nsurl= [[NSURL URLWithString:nsurlstring] autorelease];
    NSURLRequest *nsrequest= [[NSURLRequest requestWithURL:nsurl] autorelease];
    [webView loadRequest:nsrequest];
}

void WebWindow::SendMessage(AutoString message)
{
    // JSON-encode the message
    NSString* nsmessage = [NSString stringWithUTF8String:message];
    NSData* data = [NSJSONSerialization dataWithJSONObject:@[nsmessage] options:0 error:nil];
    NSString *nsmessageJson = [[[NSString alloc]
        initWithData:data
        encoding:NSUTF8StringEncoding] autorelease];
    nsmessageJson = [[nsmessageJson substringToIndex:([nsmessageJson length]-1)] substringFromIndex:1];

    WKWebView *webView = (WKWebView *)_webview;
    NSString *javaScriptToEval = [NSString stringWithFormat:@"__dispatchMessageCallback(%@)", nsmessageJson];
    [webView evaluateJavaScript:javaScriptToEval completionHandler:nil];
}

void WebWindow::ShowUserNotification(AutoString image, AutoString title, AutoString message, AutoString navURI)
{
    NSString* nsimage = [NSString stringWithUTF8String:image];
    NSString* nstitle = [NSString stringWithUTF8String:title];
    NSString* nsmessage = [NSString stringWithUTF8String:message];
    NSImage *nsImage = [[NSImage alloc] initByReferencingFile:nsimage];

    NSUserNotification *notification = [[NSUserNotification alloc] init];
    [notification setValue:nsImage forKey:@"_identityImage"];
    [notification setTitle:nstitle];
    [notification setInformativeText:nsmessage];
    [notification setSoundName:NSUserNotificationDefaultSoundName];

    if(navURI && strlen(navURI) > 0)
    {
        NTLog(this, Info, "Called : ShowUserNotification Navigate URI->(%s)", (AutoString)navURI);

        NSMutableDictionary *userInfo = [[NSMutableDictionary alloc] init];
        userInfo[@"NAVIGATE_URI"] = (navURI)? [NSString stringWithUTF8String:navURI] : [NSString stringWithUTF8String:""];
        [notification setValue:@(YES) forKey:@"_showsButtons"];
        [notification setUserInfo:userInfo];
        [userInfo autorelease];
    }

    [[NSUserNotificationCenter defaultUserNotificationCenter] deliverNotification:notification];

    [nsImage autorelease];
    [notification autorelease];
}

void WebWindow::AddCustomScheme(AutoString scheme, WebResourceRequestedCallback requestHandler)
{
    // Note that this can only be done *before* the WKWebView is instantiated, so we only let this
    // get called from the options callback in the constructor
    MyUrlSchemeHandler* schemeHandler = [[[MyUrlSchemeHandler alloc] init] autorelease];
    schemeHandler->requestHandler = requestHandler;

    WKWebViewConfiguration *webviewConfiguration = (WKWebViewConfiguration *)_webviewConfiguration;
    NSString* nsscheme = [NSString stringWithUTF8String:scheme];
    [webviewConfiguration setURLSchemeHandler:schemeHandler forURLScheme:nsscheme];
}

void WebWindow::SetResizable(bool resizable)
{
    NSWindow* window = (NSWindow*)_window;
    if (resizable) window.styleMask |= NSWindowStyleMaskResizable;
    else window.styleMask &= ~NSWindowStyleMaskResizable;
}

void WebWindow::GetSize(int* width, int* height)
{
    NSWindow* window = (NSWindow*)_window;
    NSSize size = [window frame].size;
    if (width) *width = (int)roundf(size.width);
    if (height) *height = (int)roundf(size.height);
}

void WebWindow::SetSize(int width, int height)
{
    CGFloat fw = (CGFloat)width;
    CGFloat fh = (CGFloat)height;
    NSWindow* window = (NSWindow*)_window;
    NSRect frame = [window frame];
    CGFloat oldHeight = frame.size.height;
    CGFloat heightDelta = fh - oldHeight;  
    frame.size = CGSizeMake(fw, fh);
    frame.origin.y -= heightDelta;
    [window setFrame: frame display: YES];
}

void WebWindow::GetAllMonitors(GetAllMonitorsCallback callback)
{
    if (callback)
    {
        for (NSScreen* screen in [NSScreen screens])
        {
            Monitor props = {};
            NSRect frame = [screen frame];
            props.monitor.x = (int)roundf(frame.origin.x);
            props.monitor.y = (int)roundf(frame.origin.y);
            props.monitor.width = (int)roundf(frame.size.width);
            props.monitor.height = (int)roundf(frame.size.height);
            NSRect vframe = [screen visibleFrame];
            props.work.x = (int)roundf(vframe.origin.x);
            props.work.y = (int)roundf(vframe.origin.y);
            props.work.width = (int)roundf(vframe.size.width);
            props.work.height = (int)roundf(vframe.size.height);
            callback(&props);
        }
    }
}

unsigned int WebWindow::GetScreenDpi()
{
	return 72;
}

void WebWindow::GetPosition(int* x, int* y)
{
    NSWindow* window = (NSWindow*)_window;
    NSRect frame = [window frame];
    if (x) *x = (int)roundf(frame.origin.x);
    if (y) *y = (int)roundf(-frame.size.height - frame.origin.y); // It will be negative, because macOS measures from bottom-left. For x-plat consistency, we want increasing this value to mean moving down.
}

void WebWindow::SetPosition(int x, int y)
{
    NSWindow* window = (NSWindow*)_window;
    NSRect frame = [window frame];
    frame.origin.x = (CGFloat)x;
    frame.origin.y = -frame.size.height - (CGFloat)y;
    [window setFrame: frame display: YES];
}

void WebWindow::SetTopmost(bool topmost)
{
    NSWindow* window = (NSWindow*)_window;
    if (topmost) [window setLevel:NSFloatingWindowLevel];
    else [window setLevel:NSNormalWindowLevel];
}

void WebWindow::SetIconFile(AutoString filename)
{
    // Crash: Occured abnormal situation that zsh: illegal hardware instruction dotnet
    // So Delete autorelease
	//NSString* path = [[NSString stringWithUTF8String:filename] autorelease];
	NSString* path = [NSString stringWithUTF8String:filename];
    NSImage* icon = [[NSImage alloc] initWithContentsOfFile:path];
    if (icon != nil)
    {
        NSWindow* window = (NSWindow*)_window;
        [[window standardWindowButton:NSWindowDocumentIconButton] setImage:icon];
    }
}

void WebWindow::OnHotKey(int groupID)
{
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];
    NSLog(@"OnHotKey Click!!!");
    [_appDelegate hotkeyClipBoardWithEvent:NULL object:numGuId];
}

void WebWindow::ClipTypeSelect(int groupID)
{
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];
    [_appDelegate ClipTypeSelect:numGuId];
}

void WebWindow::ClipMemFree(int groupID)
{
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];
    //[_appDelegate hotkeyClipBoardWithEvent:NULL object:numGuId];
}

void WebWindow::ClipFirstSendTypeText(int groupID)
{
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];
    [_appDelegate ClipFirstSendTypeText:numGuId];
}

void WebWindow::SetClipBoardSendFlag(int groupID)
{
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];
    //[_appDelegate hotkeyClipBoardWithEvent:NULL object:numGuId];
}

void WebWindow::GenerateHotKey(bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
{
    [_appDelegate hotkeyGenerate:chVKCode alt:bAlt control:bControl shift:bShift win:bWin];
}

void WebWindow::RegisterQuitHotKey()
{
	std::string strKeyCode(1, 'Q');
    NSUInteger uKeyMask = GetKeyMask(true, false, false, false);
    unsigned short usKeyCode = GetCharKeyCode([NSString stringWithUTF8String:strKeyCode.c_str()]);

    DDHotKeyCenter *HKCenter = [DDHotKeyCenter sharedHotKeyCenter];

    if (![HKCenter registerHotKeyWithKeyCode:usKeyCode modifierFlags:(uKeyMask) target:_appDelegate action:@selector(hotkeyQuitWithEvent:object:) object:0]) {
	    NTLog(this, Err, "Fail: Setting ClipBoard HotKey, \" <Command>Q \" to activate keybinding\n");
    } else {
	    NTLog(this, Info, "Setting ClipBoard HotKey, \" <Command>Q \" to activate keybinding\n");
    }
}

void WebWindow::RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
{
	std::string strModifiers = "";
	std::string strKeyCode(1, chVKCode);
	if(bAlt)
		strModifiers += "<Alt>";             // Alt 키 조합 (0x0001)
	if (bControl)
		strModifiers += "<Ctrl>";			 // Control 키 조합 (0x0002)
	if (bShift)
		strModifiers += "<Shift>";			 // Shift 키 조합 (0x0004)
	if (bWin)
		strModifiers += "<Super>";			 // Window 키 조합 (0x0008)

	strModifiers += strKeyCode; // Key Code

    NSUInteger uKeyMask = GetKeyMask(bWin, bAlt, bControl, bShift);
    unsigned short usKeyCode = GetCharKeyCode([NSString stringWithUTF8String:strKeyCode.c_str()]);

    DDHotKeyCenter *HKCenter = [DDHotKeyCenter sharedHotKeyCenter];
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];

    if (![HKCenter registerHotKeyWithKeyCode:usKeyCode modifierFlags:(uKeyMask) target:_appDelegate action:@selector(hotkeyClipBoardWithEvent:object:) object:numGuId]) {
	    NTLog(this, Err, "Fail: Setting ClipBoard HotKey, \" %s \" to activate keybinding in GUID:%d\n", strModifiers.c_str(), groupID);
    } else {
	    NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to activate keybinding in GUID:%d\n", strModifiers.c_str(), groupID);
    }
}

void WebWindow::UnRegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
{
	// have to use same parameter of RegisterClipboardHotKey.
	std::string strModifiers = "";
	std::string strKeyCode(1, chVKCode);
	if(bAlt)
		strModifiers += "<Alt>";             // Alt 키 조합 (0x0001)
	if (bControl)
		strModifiers += "<Ctrl>";			 // Control 키 조합 (0x0002)
	if (bShift)
		strModifiers += "<Shift>";			 // Shift 키 조합 (0x0004)
	if (bWin)
		strModifiers += "<Super>";			 // Window 키 조합 (0x0008)

	strModifiers += strKeyCode; // Key Code

    NSUInteger uKeyMask = GetKeyMask(bWin, bAlt, bControl, bShift);
    unsigned short usKeyCode = GetCharKeyCode([NSString stringWithUTF8String:strKeyCode.c_str()]);

    DDHotKeyCenter *HKCenter = [DDHotKeyCenter sharedHotKeyCenter];
    [HKCenter unregisterHotKeyWithKeyCode:usKeyCode modifierFlags:(uKeyMask)];
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to deactivate keybinding\n", strModifiers.c_str());
}

void WebWindow::RegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{
	std::string strModifiers = "";
	std::string strKeyCode(1, chVKCode);
	if(bAlt)
		strModifiers += "<Alt>";             // Alt 키 조합 (0x0001)
	if (bControl)
		strModifiers += "<Ctrl>";			 // Control 키 조합 (0x0002)
	if (bShift)
		strModifiers += "<Shift>";			 // Shift 키 조합 (0x0004)
	if (bWin)
		strModifiers += "<Super>";			 // Window 키 조합 (0x0008)

	strModifiers += strKeyCode; // Key Code

    NSUInteger uKeyMask = GetKeyMask(bWin, bAlt, bControl, bShift);
    unsigned short usKeyCode = GetCharKeyCode([NSString stringWithUTF8String:strKeyCode.c_str()]);

    DDHotKeyCenter *HKCenter = [DDHotKeyCenter sharedHotKeyCenter];
    NSNumber *numGuId = [NSNumber numberWithInt:groupID];

    if (![HKCenter registerHotKeyWithKeyCode:usKeyCode modifierFlags:(uKeyMask) target:_appDelegate action:@selector(hotkeyClipBoardWithEvent:object:) object:numGuId]) {
	    NTLog(this, Err, "Fail: Setting ClipBoard HotKey, \" %s \" to activate keybinding in GUID:%d\n", strModifiers.c_str(), groupID);
    } else {
	    NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to activate keybinding in GUID:%d\n", strModifiers.c_str(), groupID);
    }
}

void WebWindow::UnRegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{
	// have to use same parameter of RegisterClipboardHotKey.
	std::string strModifiers = "";
	std::string strKeyCode(1, chVKCode);
	if(bAlt)
		strModifiers += "<Alt>";             // Alt 키 조합 (0x0001)
	if (bControl)
		strModifiers += "<Ctrl>";			 // Control 키 조합 (0x0002)
	if (bShift)
		strModifiers += "<Shift>";			 // Shift 키 조합 (0x0004)
	if (bWin)
		strModifiers += "<Super>";			 // Window 키 조합 (0x0008)

	strModifiers += strKeyCode; // Key Code

    NSUInteger uKeyMask = GetKeyMask(bWin, bAlt, bControl, bShift);
    unsigned short usKeyCode = GetCharKeyCode([NSString stringWithUTF8String:strKeyCode.c_str()]);

    DDHotKeyCenter *HKCenter = [DDHotKeyCenter sharedHotKeyCenter];
    [HKCenter unregisterHotKeyWithKeyCode:usKeyCode modifierFlags:(uKeyMask)];
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to deactivate keybinding\n", strModifiers.c_str());
}

void WebWindow::FolderOpen(AutoString strDownPath)
{
	// 탐색기 Open 하는 로직 필요
    BOOL bRet = [[NSWorkspace sharedWorkspace] openFile:[NSString stringWithUTF8String:strDownPath]];
    if( bRet ) NTLog(this, Info, "Finder Open Folder: %s", strDownPath);
    else NTLog(this, Info, "Can't Finder Open Folder: %s", strDownPath);
}

void WebWindow::SetClipBoard(int groupID, int nType, int nClipSize, void* data)
{
	/* TEXT = 1, IMAGE = 2, OBJECT = 3 */
	NTLog(this, Info, "Called SetClipBoard, Type=%d(%s) Size(%ld)", nType, nType==D_CLIP_TEXT?"TEXT":nType==D_CLIP_IMAGE?"IMAGE":"OBJECT", nClipSize);
    __block BOOL bRecvClipboard=NO;
    NSPasteboard *pasteBoard = NSPasteboard.generalPasteboard;
    
    dispatch_async(dispatch_get_main_queue(), ^{
        switch(nType)
        {
            case D_CLIP_TEXT: // 텍스트 수신
            {
                /* Set clipboard text */
                NSString *NSContents = nil;
                NSContents=[NSString stringWithUTF8String:(const char *)data];
                if(NSContents==nil)
                {
                    NSLog(@"NSContents NULL!!");
                }
                else
                {
                    NSLog(@"NSContents : %@", NSContents);
                }
                // set string sample
                [pasteBoard declareTypes:[NSArray arrayWithObject:NSStringPboardType] owner:nil];
                if ([pasteBoard setString:NSContents forType:NSStringPboardType])
                {
                    NSLog(@"Text Clipboard MAC OS Set succeed");
                    bRecvClipboard=YES;
                }
                else {
                    NSLog(@"Text Clipboard MAC OS Set failed");
                }
            } break;
            case D_CLIP_IMAGE: // 이미지 수신
            {
                NSData *pData = nil;
                pData = [NSData dataWithBytes:data length:nClipSize];
                
                [pasteBoard declareTypes:[NSArray arrayWithObject:NSPasteboardTypePNG] owner:nil];
                if ([pasteBoard setData:pData forType:NSPasteboardTypePNG])
                {
                    NSLog(@"Image Clipboard MAC OS Set succeed");
                    bRecvClipboard=YES;
                }else
                {
                    NSLog(@"Image Clipboard MAC OS Set failed");
                }
            } break;
            case D_CLIP_OBJECT:
            default:
            {

            } break;
        }
        
        if(bRecvClipboard)
        {
            if (_recvclipboardCallback != NULL)
                _recvclipboardCallback(groupID);
        }
    });
}

void WebWindow::ProgramExit()
{
	NTLog(this, Info, "Called : OpenNetLink Exit");
	tray_exit();
}

bool WebWindow::GetTrayUse()
{
	NTLog(this, Info, "Called : OpenNetLink Tray Status");
	return g_bDoExit2TrayUse;
}

void WebWindow::MoveWebWindowToTray()
{
	NTLog(this, Info, "Called : OpenNetLink Move To Tray");
    dispatch_async(dispatch_get_main_queue(), ^{
        struct tray_menu *item = tray.menu;
        do
        {
            if (strcmp(item->text, "Hide") == 0) {
                toggle_show(item);
                break;
            }
        } while ((++item)->text != NULL);
    });
}

void WebWindow::MoveTrayToWebWindow()
{
	NTLog(this, Info, "Called : OpenNetLink Move To WebWindow");

    dispatch_async(dispatch_get_main_queue(), ^{
        struct tray_menu* item = tray.menu;
        do
	    {
		if (strcmp(item->text, "Show") == 0) {
			toggle_show(item);
			break;
		}
	    } while ((++item)->text != NULL);
    });
}

void WebWindow::MinimizeWebWindow()
{
	NTLog(this, Info, "Called : OpenNetLink Minimize WebWindow");
	struct tray_menu *item = tray.menu;
	do
	{
		if (strcmp(item->text, "Hide") == 0) {
			toggle_minimize(item);
			break;
		}
	} while ((++item)->text != NULL);
}

/*
- Create launch Plist File
    ~/Library/LaunchAgents/com.hanssak.OpenNetLinkApp.plist
- Register
  launchctl load -w ~/Library/LaunchAgents/com.hanssak.OpenNetLinkApp.plist
- Unregister
  launchctl unload -w ~/Library/LaunchAgents/com.hanssak.OpenNetLinkApp.plist
*/
void WebWindow::RegisterStartProgram()
{
	int myuid;

	passwd *mypasswd;
	myuid = getuid();
	mypasswd = getpwuid(myuid);

	std::string filePath = std::string(mypasswd->pw_dir) + "/Library/LaunchAgents/com.hanssak.OpenNetLinkApp.plist";
    //파일의 존재 여부를 체크해서 있으면 작업을 하지 않고 그냥 넘김
    std::ifstream f(filePath.data());
    if(f.good())
    {
        return;
    }
    else
    {
        // write File
        std::ofstream writeFile(filePath.data());
        if( writeFile.is_open() ){
            writeFile << "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            writeFile << "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n";
            writeFile << "<plist version=\"1.0\">\n";
            writeFile << "<dict>\n";
            writeFile << "    <key>Label</key>\n";
            writeFile << "    <string>com.hanssak.OpenNetLinkApp</string>\n";
            writeFile << "    <key>ProgramArguments</key>\n";
            writeFile << "    <array>\n";
            //writeFile << "        <string>/Applications/OpenNetLinkApp.app/Contents/MacOS/OpenNetLinkApp.sh</string>\n";
            writeFile << "        <string>/usr/bin/open</string>\n";
            writeFile << "        <string>-a</string>\n";
            writeFile << "        <string>/Applications/OpenNetLinkApp.app</string>\n";
            writeFile << "    </array>\n";
            writeFile << "    <key>ProcessType</key>\n";
            writeFile << "    <string>Interactive</string>\n";
            writeFile << "    <key>RunAtLoad</key>\n";
            writeFile << "    <false/>\n";
            writeFile << "    <key>KeepAlive</key>\n";
            writeFile << "    <false/>\n";
            writeFile << "</dict>\n";
            writeFile << "</plist>\n";
            writeFile.close();
            NTLog(this, Info, "Called : RegisterStartProgram, Success: Create File [%s]", filePath.data());
        } else {
            NTLog(this, Err, "Called : RegisterStartProgram, Fail: Create File [%s] Err[%s]", filePath.data(), strerror(errno));
        }

        NSString *theCMD = [@"launchctl load -w " stringByAppendingString:[NSString stringWithUTF8String:filePath.c_str()]];
        system(theCMD.UTF8String);

        
        // 먼저 RunAtLoad 를 false로 두고 등록한 다음에 true 변경하여 다시 저장
        // write File
        
        writeFile.open(filePath.data());
        if( writeFile.is_open() ){
            writeFile << "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            writeFile << "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n";
            writeFile << "<plist version=\"1.0\">\n";
            writeFile << "<dict>\n";
            writeFile << "    <key>Label</key>\n";
            writeFile << "    <string>com.hanssak.OpenNetLinkApp</string>\n";
            writeFile << "    <key>ProgramArguments</key>\n";
            writeFile << "    <array>\n";
            //writeFile << "        <string>/Applications/OpenNetLinkApp.app/Contents/MacOS/OpenNetLinkApp.sh</string>\n";
            writeFile << "        <string>/usr/bin/open</string>\n";
            writeFile << "        <string>-a</string>\n";
            writeFile << "        <string>/Applications/OpenNetLinkApp.app</string>\n";
            writeFile << "    </array>\n";
            writeFile << "    <key>ProcessType</key>\n";
            writeFile << "    <string>Interactive</string>\n";
            writeFile << "    <key>RunAtLoad</key>\n";
            writeFile << "    <true/>\n";
            writeFile << "    <key>KeepAlive</key>\n";
            writeFile << "    <false/>\n";
            writeFile << "</dict>\n";
            writeFile << "</plist>\n";
            writeFile.close();
            NTLog(this, Info, "Called : RegisterStartProgram, Success: Create File [%s]", filePath.data());
        } else {
            NTLog(this, Err, "Called : RegisterStartProgram, Fail: Create File [%s] Err[%s]", filePath.data(), strerror(errno));
        }
    }
}

void WebWindow::UnRegisterStartProgram()
{
	int myuid;

	passwd *mypasswd;
	myuid = getuid();
	mypasswd = getpwuid(myuid);

	std::string filePath = std::string(mypasswd->pw_dir) + "/Library/LaunchAgents/com.hanssak.OpenNetLinkApp.plist";
    NSString *theCMD = [@"launchctl unload -w " stringByAppendingString:[NSString stringWithUTF8String:filePath.c_str()]];
    system(theCMD.UTF8String);

	if (std::remove(filePath.data()) == 0) // delete file
		NTLog(this, Info, "Called : UnRegisterStartProgram, Success: Remove File [%s]", filePath.data());
	else
		NTLog(this, Err, "Called : UnRegisterStartProgram, Fail: Remove File [%s] Err[%s]", filePath.data(), strerror(errno));
}

void WebWindow::SetTrayUse(bool useTray)
{
    g_bDoExit2TrayUse = useTray;
    NTLog(this, Info, "Called : SetTrayUse(@@@@@@@@@@) : %s", (AutoString)(g_bDoExit2TrayUse ? "Yes": "No") );
}	


void WebWindow::SetTrayStartUse(bool bUseStartTray)
{
    g_bStartTray = bUseStartTray; 
    NTLog(this, Info, "Called : OpenNetLink SetTrayStartUse : %s", (AutoString)bUseStartTray ? "Yes": "No");
}

//void WebWindow::SetCopyAndSend(bool bUseCopyAndSend)
//{
//    [_appDelegate SetCopyAndSend:bUseCopyAndSend];
//}

void WebWindow::SetUseClipCopyNsend(bool bUse)
{
	g_bClipCopyNsend = bUse;
    [_appDelegate SetCopyAndSend:g_bClipCopyNsend];
	//NTLog(this, Info, "Called : SetUseClipCopyNsend(@@@@@@@@@@) : %s", (AutoString)(bUse ? "Yes" : "No"));
}

void WebWindow::SetNativeClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{

}

#endif
