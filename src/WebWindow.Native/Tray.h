#ifndef TRAY_H
#define TRAY_H

#if defined (_WIN32) || defined (_WIN64)
#define TRAY_WINAPI 1
#elif defined (__linux__) || defined (linux) || defined (__linux) || defined (OS_LINUX)
#define TRAY_APPINDICATOR 1
#elif defined (__APPLE__) || defined (__MACH__) || defined (OS_MAC)
#define TRAY_APPKIT 1
#endif

#if TRAY_APPINDICATOR
//#define TRAY_ICON1 "indicator-messages"
//#define TRAY_ICON2 "indicator-messages-new"
#define TRAY_ICON1 "hanssak.webwindow.open.netlink"
#define TRAY_ICON2 "hanssak.webwindow.open.netlink"
#define TRAY_ICON3 "hanssak.webwindow.open.netlink"
// Must use command :  sudo xdg-icon-resource install --novendor --size 16 wwwroot/images/adminlte/ico.png hanssak.webwindow.open.netlink
#elif TRAY_APPKIT
#define TRAY_ICON1 "wwwroot/MacTray.png"
#define TRAY_ICON2 "wwwroot/MacTray.png"
#define TRAY_ICON3 "wwwroot/MacTray.png"
#elif TRAY_WINAPI
#define TRAY_ICON1 "wwwroot/SecureGate.ico"
#define TRAY_ICON2 "wwwroot/SecureGate.ico"
#define TRAY_ICON3 "wwwroot/SecureGate.ico"
#endif

struct tray_menu;

struct tray {
  char *icon;
  char *dark_icon;
  struct tray_menu *menu;
};

struct tray_menu {
  char *text;
  int disabled;
  int checked;
  int usedCheck;

  void (*cb)(struct tray_menu *);
  void *context;

  struct tray_menu *submenu;
};

static void tray_update(struct tray *tray);

#if defined(TRAY_APPINDICATOR)

#include <gtk/gtk.h>
#include <libappindicator/app-indicator.h>

#define TRAY_APPINDICATOR_ID "tray-id"

static AppIndicator *indicator = NULL;
static int loop_result = 0;

static void _tray_menu_cb(GtkMenuItem *item, gpointer data) {
  (void)item;
  struct tray_menu *m = (struct tray_menu *)data;
  m->cb(m);
}

static GtkMenuShell *_tray_menu(struct tray_menu *m) {
  GtkMenuShell *menu = (GtkMenuShell *)gtk_menu_new();
  for (; m != NULL && m->text != NULL; m++) {
    GtkWidget *item;
    if (strcmp(m->text, "-") == 0) {
      item = gtk_separator_menu_item_new();
    } else {
      if (m->submenu != NULL) {
        item = gtk_menu_item_new_with_label(m->text);
        gtk_menu_item_set_submenu(GTK_MENU_ITEM(item),
                                  GTK_WIDGET(_tray_menu(m->submenu)));
      } else {
        if(m->usedCheck) {
          item = gtk_check_menu_item_new_with_label(m->text);
          gtk_check_menu_item_set_active(GTK_CHECK_MENU_ITEM(item), !!m->checked);
        } else {
          item = gtk_menu_item_new_with_label(m->text);
        }
      }
      gtk_widget_set_sensitive(item, !m->disabled);
      if (m->cb != NULL) {
        g_signal_connect(item, "activate", G_CALLBACK(_tray_menu_cb), m);
      }
    }
    gtk_widget_show(item);
    gtk_menu_shell_append(menu, item);
  }
  return menu;
}

static int tray_init(struct tray *tray) {
  if (gtk_init_check(0, NULL) == FALSE) {
    return -1;
  }
  indicator = app_indicator_new(TRAY_APPINDICATOR_ID, tray->icon,
                                APP_INDICATOR_CATEGORY_APPLICATION_STATUS);
  app_indicator_set_status(indicator, APP_INDICATOR_STATUS_ACTIVE);
  tray_update(tray);
  return 0;
}

static int tray_loop(int blocking) {
  gtk_main_iteration_do(blocking);
  return loop_result;
}

static void tray_update(struct tray *tray) {
  app_indicator_set_icon(indicator, tray->icon);
  // GTK is all about reference counting, so previous menu should be destroyed
  // here
  app_indicator_set_menu(indicator, GTK_MENU(_tray_menu(tray->menu)));
}

static void tray_exit() { loop_result = -1; }

#elif defined(TRAY_APPKIT)

#include <objc/objc-runtime.h>
#include <limits.h>
#include <availability.h>
#include <string.h>

static id app;
static id icon;
static id pool;
static id statusBar;
static id statusItem;
static id statusBarButton;

static id _tray_menu(struct tray_menu *m) {
    id menu = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSMenu"), sel_registerName("new"));
    ((void(*)(id, SEL))objc_msgSend)(menu, sel_registerName("autorelease"));
    ((void(*)(id, SEL, bool))objc_msgSend)(menu, sel_registerName("setAutoenablesItems:"), false);

    for (; m != NULL && m->text != NULL; m++) {
      if (strcmp(m->text, "-") == 0) {
        ((void(*)(id, SEL, id))objc_msgSend)(menu, sel_registerName("addItem:"), 
          ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSMenuItem"), sel_registerName("separatorItem")));
      } else {
        id menuItem = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSMenuItem"), sel_registerName("alloc"));
        ((void(*)(id, SEL))objc_msgSend)(menuItem, sel_registerName("autorelease"));
        ((void(*)(id, SEL, id, SEL, id))objc_msgSend)(menuItem, sel_registerName("initWithTitle:action:keyEquivalent:"),
                  ((id(*)(id, SEL, char *))objc_msgSend)((id)objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), m->text),
                  sel_registerName("menuCallback:"),
                  ((id(*)(id, SEL, const char *))objc_msgSend)((id)objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), ""));
  
        ((void(*)(id, SEL, bool))objc_msgSend)(menuItem, sel_registerName("setEnabled:"), (m->disabled ? false : true));
          ((void(*)(id, SEL, int))objc_msgSend)(menuItem, sel_registerName("setState:"), (m->checked ? 1 : 0));
          ((void(*)(id, SEL, id))objc_msgSend)(menuItem, sel_registerName("setRepresentedObject:"),
            ((id(*)(id, SEL, struct tray_menu *))objc_msgSend)((id)objc_getClass("NSValue"), sel_registerName("valueWithPointer:"), m));
  
          ((void(*)(id, SEL, id))objc_msgSend)(menu, sel_registerName("addItem:"), menuItem);
  
          if (m->submenu != NULL) {
            ((void(*)(id, SEL, id, id))objc_msgSend)(menu, sel_registerName("setSubmenu:forItem:"), _tray_menu(m->submenu), menuItem);
      }
    }
  }

  return menu;
}

static void menu_callback(id self, SEL cmd, id sender) {
    id repObj = ((id(*)(id, SEL))objc_msgSend)(sender, sel_registerName("representedObject"));
    struct tray_menu *m = ((struct tray_menu*(*)(id, SEL))objc_msgSend)(repObj, sel_registerName("pointerValue"));
    if (m != NULL && m->cb != NULL) {
      m->cb(m);
    }
}

static int tray_init(struct tray *tray) {
    // check if we're in dark mode
  bool dark_mode = false;
  bool auto_switch = false;
    id userdefaults = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSUserDefaults"), sel_registerName("standardUserDefaults"));
    id keystr = ((id(*)(id,SEL,id))objc_msgSend)(userdefaults, sel_registerName("stringForKey:"),
                  ((id(*)(id, SEL, const char *))objc_msgSend)((id)objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), "AppleInterfaceStyle"));
    char* ckeystr = ((char*(*)(id,SEL))objc_msgSend)(keystr, sel_registerName("UTF8String"));
    if (ckeystr != NULL && strcmp(ckeystr, "Dark") == 0)
      dark_mode = true; 
    id keystr2 = ((id(*)(id,SEL,id))objc_msgSend)(userdefaults, sel_registerName("stringForKey:"),
                  ((id(*)(id, SEL, const char *))objc_msgSend)((id)objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), "AppleInterfaceStyleSwitchesAutomatically"));
    char* ckeystr2 = ((char*(*)(id,SEL))objc_msgSend)(keystr2, sel_registerName("UTF8String"));
    if (ckeystr2 != NULL)
      auto_switch = true;
      icon = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSImage"), sel_registerName("alloc"));
    //((void(*)(id, SEL, id))objc_msgSend)(icon, sel_registerName("initWithContentsOfFile:"),
    ((void(*)(id, SEL, id))objc_msgSend)(icon, sel_registerName("initByReferencingFile:"),
      ((id(*)(id, SEL, char *))objc_msgSend)((id)objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), dark_mode ? tray->dark_icon : tray->icon));
    
    // bool out = true;
    // object_setInstanceVariable(icon, "template", &out);
    
    pool = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSAutoreleasePool"),
                          sel_registerName("new"));
  
    app = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSApplication"),
                          sel_registerName("sharedApplication"));
  
    Class trayDelegateClass = objc_allocateClassPair(objc_getClass("NSObject"), "Tray", 0);
    class_addProtocol(trayDelegateClass, objc_getProtocol("NSApplicationDelegate"));
    class_addMethod(trayDelegateClass, sel_registerName("menuCallback:"), (IMP)menu_callback, "v@:@");
    objc_registerClassPair(trayDelegateClass);

    // from https://github.com/glfw/glfw/issues/1024
    id orig_delegate = ((id(*)(id, SEL))objc_msgSend)(app, sel_registerName("delegate"));
    id origClass = ((id(*)(id, SEL))objc_msgSend)((id)orig_delegate, sel_registerName("className"));
    char* delegateType = ((char*(*)(id,SEL))objc_msgSend)(origClass, sel_registerName("UTF8String"));
    Class original_c = objc_getClass(delegateType);
    Method swizzled_m = class_getInstanceMethod(trayDelegateClass, sel_registerName("menuCallback:"));
    bool didAddMethod = class_addMethod(original_c, sel_registerName("menuCallback:"), method_getImplementation(swizzled_m), method_getTypeEncoding(swizzled_m));

    statusBar = ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSStatusBar"),
                          sel_registerName("systemStatusBar"));

    statusItem = ((id(*)(id, SEL, double))objc_msgSend)(statusBar, sel_registerName("statusItemWithLength:"), -1.0);

    ((void(*)(id, SEL))objc_msgSend)(statusItem, sel_registerName("retain"));
    ((void(*)(id, SEL, bool))objc_msgSend)(statusItem, sel_registerName("setHighlightMode:"), true);
    statusBarButton = ((id(*)(id, SEL))objc_msgSend)(statusItem, sel_registerName("button"));
    tray_update(tray);
    ((void(*)(id, SEL, bool))objc_msgSend)(app, sel_registerName("activateIgnoringOtherApps:"), true);
    return 0;
}

static int tray_loop(int blocking) {
    id until = (blocking ? 
      ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSDate"), sel_registerName("distantFuture")) : 
      ((id(*)(id, SEL))objc_msgSend)((id)objc_getClass("NSDate"), sel_registerName("distantPast")));
  
    id event = ((id(*)(id, SEL, unsigned long, id, id, bool))objc_msgSend)(app, sel_registerName("nextEventMatchingMask:untilDate:inMode:dequeue:"), 
                ULONG_MAX, 
                until, 
                ((id(*)(id, SEL, const char*))objc_msgSend)((id)objc_getClass("NSString"), 
                  sel_registerName("stringWithUTF8String:"), 
                  "kCFRunLoopDefaultMode"), 
                true);
    if (event) {
      ((void(*)(id, SEL, id))objc_msgSend)(app, sel_registerName("sendEvent:"), event);
    }
    return 0;
}

static void tray_update(struct tray *tray) {
  ((void(*)(id, SEL, id))objc_msgSend)(statusBarButton, sel_registerName("setImage:"), icon);
  ((void(*)(id, SEL, id))objc_msgSend)(statusItem, sel_registerName("setMenu:"), _tray_menu(tray->menu));
}

static void tray_exit() { ((void(*)(id, SEL, id))objc_msgSend)(app, sel_registerName("terminate:"), app); }

#elif defined(TRAY_WINAPI)
#include <windows.h>

#include <shellapi.h>

#define WM_TRAY_CALLBACK_MESSAGE (WM_USER + 1)
#define WC_TRAY_CLASS_NAME L"TRAY"
#define ID_TRAY_FIRST 1000

static WNDCLASSEX wc;
static NOTIFYICONDATA nid;
static HWND hwnd;
static HMENU hmenu = NULL;

static LRESULT CALLBACK _tray_wnd_proc(HWND hwnd, UINT msg, WPARAM wparam,
                                       LPARAM lparam) {
  switch (msg) {
  case WM_CLOSE:
    DestroyWindow(hwnd);
    return 0;
  case WM_DESTROY:
    PostQuitMessage(0);
    return 0;
  case WM_TRAY_CALLBACK_MESSAGE:
    if (lparam == WM_LBUTTONUP || lparam == WM_RBUTTONUP) {
      POINT p;
      GetCursorPos(&p);
      SetForegroundWindow(hwnd);
      WORD cmd = TrackPopupMenu(hmenu, TPM_LEFTALIGN | TPM_RIGHTBUTTON |
                                           TPM_RETURNCMD | TPM_NONOTIFY,
                                p.x, p.y, 0, hwnd, NULL);
      SendMessage(hwnd, WM_COMMAND, cmd, 0);
      return 0;
    }
    break;
  case WM_COMMAND:
    if (wparam >= ID_TRAY_FIRST) {
        MENUITEMINFO item;
      item.cbSize = sizeof(MENUITEMINFO);
      item.fMask = MIIM_ID | MIIM_DATA;
      if (GetMenuItemInfo(hmenu, wparam, FALSE, &item)) {
        struct tray_menu *menu = (struct tray_menu *)item.dwItemData;
        if (menu != NULL && menu->cb != NULL) {
          menu->cb(menu);
        }
      }
      return 0;
    }
    break;
  }
  return DefWindowProc(hwnd, msg, wparam, lparam);
}

static HMENU _tray_menu(struct tray_menu *m, UINT *id) {
  HMENU hmenu = CreatePopupMenu();
  for (; m != NULL && m->text != NULL; m++, (*id)++) {
    if (strcmp(m->text, "-") == 0) {
      InsertMenu(hmenu, *id, MF_SEPARATOR, TRUE, L"");
    } else {
      MENUITEMINFO item;
      memset(&item, 0, sizeof(item));
      item.cbSize = sizeof(MENUITEMINFO);
      item.fMask = MIIM_ID | MIIM_TYPE | MIIM_STATE | MIIM_DATA;
      item.fType = 0;
      item.fState = 0;
      if (m->submenu != NULL) {
        item.fMask = item.fMask | MIIM_SUBMENU;
        item.hSubMenu = _tray_menu(m->submenu, id);
      }
      if (m->disabled) {
        item.fState |= MFS_DISABLED;
      }
      if (m->checked) {
        item.fState |= MFS_CHECKED;
      }
      item.wID = *id;

      wchar_t wText[MAX_PATH];
      memset(wText, 0x00, sizeof(wText));
      MultiByteToWideChar(CP_ACP, 0, m->text, -1, wText, sizeof(wText));
      item.dwTypeData = wText;
      //item.dwTypeData = m->text;
      item.dwItemData = (ULONG_PTR)m;

      InsertMenuItem(hmenu, *id, TRUE, &item);
    }
  }
  return hmenu;
}

static int tray_init(struct tray *tray) {
  memset(&wc, 0, sizeof(wc));
  wc.cbSize = sizeof(WNDCLASSEX);
  wc.lpfnWndProc = _tray_wnd_proc;
  wc.hInstance = GetModuleHandle(NULL);
  wc.lpszClassName = WC_TRAY_CLASS_NAME;
  if (!RegisterClassEx(&wc)) {
    return -1;
  }

  hwnd = CreateWindowEx(0, WC_TRAY_CLASS_NAME, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0);
  if (hwnd == NULL) {
    return -1;
  }
  UpdateWindow(hwnd);

  memset(&nid, 0, sizeof(nid));
  nid.cbSize = sizeof(NOTIFYICONDATA);

  //HICON icon;
  //wchar_t wchIcon[MAX_PATH];
  //memset(wchIcon, 0x00, sizeof(wchIcon));
  //MultiByteToWideChar(CP_ACP, 0, tray->icon, -1, wchIcon, sizeof(wchIcon));
  //ExtractIconEx(wchIcon, 0, NULL, &icon, 1);

  //wchar_t wchIcon[MAX_PATH];
  //memset(wchIcon, 0x00, sizeof(wchIcon));
  //MultiByteToWideChar(CP_ACP, 0, tray->icon, -1, wchIcon, sizeof(wchIcon));
 // HICON icon = (HICON)LoadImage(NULL, wchIcon, IMAGE_ICON, 0, 0, LR_LOADFROMFILE | LR_DEFAULTSIZE | LR_SHARED);

 // nid.hIcon = icon;
  nid.hWnd = hwnd;
  nid.uID = 0;
  nid.uFlags = NIF_ICON | NIF_MESSAGE;
  nid.uCallbackMessage = WM_TRAY_CALLBACK_MESSAGE;
  Shell_NotifyIcon(NIM_ADD, &nid);

  tray_update(tray);
  return 0;
}

static int tray_loop(int blocking) {
  MSG msg;
  if (blocking) {
    GetMessage(&msg, NULL, 0, 0);
  } else {
    PeekMessage(&msg, NULL, 0, 0, PM_REMOVE);
  }
  if (msg.message == WM_QUIT) {
    return -1;
  }
  TranslateMessage(&msg);
  DispatchMessage(&msg);
  return 0;
}

static void tray_update(struct tray *tray) {
  HMENU prevmenu = hmenu;
  UINT id = ID_TRAY_FIRST;
  hmenu = _tray_menu(tray->menu, &id);
  SendMessage(hwnd, WM_INITMENUPOPUP, (WPARAM)hmenu, 0);
  HICON icon;
  
  wchar_t wchIcon[MAX_PATH];
  memset(wchIcon, 0x00, sizeof(wchIcon));
  MultiByteToWideChar(CP_ACP, 0, tray->icon, -1, wchIcon, sizeof(wchIcon));
  ExtractIconEx(wchIcon, 0, NULL, &icon, 1);
  if (nid.hIcon) {
    DestroyIcon(nid.hIcon);
  }
  nid.hIcon = icon;
  Shell_NotifyIcon(NIM_MODIFY, &nid);

  if (prevmenu != NULL) {
    DestroyMenu(prevmenu);
  }
}

static void tray_exit() {
  Shell_NotifyIcon(NIM_DELETE, &nid);
  if (nid.hIcon != 0) {
    DestroyIcon(nid.hIcon);
  }
  if (hmenu != 0) {
    DestroyMenu(hmenu);
  }
  PostQuitMessage(0);
  UnregisterClass(WC_TRAY_CLASS_NAME, GetModuleHandle(NULL));
}
#else
static int tray_init(struct tray *tray) { return -1; }
static int tray_loop(int blocking) { return -1; }
static void tray_update(struct tray *tray) {}
static void tray_exit();
#endif

#endif /* TRAY_H */