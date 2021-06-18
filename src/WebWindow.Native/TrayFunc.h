#include "Tray.h"
#include "NativeLog.h"

#if TRAY_APPINDICATOR
GtkWidget* _g_window = nullptr;
#elif TRAY_APPKIT
id _g_window = nullptr;
#endif

static void toggle_cb(struct tray_menu *item);
static void toggle_show(struct tray_menu *item);
static void toggle_minimize(struct tray_menu *item);
static void hello_cb(struct tray_menu *item);
static void quit_cb(struct tray_menu *item);
static void submenu_cb(struct tray_menu *item);

// Test tray init
#if defined(TRAY_APPINDICATOR) || defined(TRAY_APPKIT)
static struct tray tray = {
    .icon 		= (char *)TRAY_ICON1,
    .dark_icon	= (char *)TRAY_ICON3,
    .menu 		= (struct tray_menu[]) {
          		  {.text = (char *)"About",   .disabled = 0, .checked = 0, .usedCheck = 0, .cb = hello_cb,	.context = NULL, .submenu = NULL},
          		  {.text = (char *)"-",       .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, 		.context = NULL, .submenu = NULL},
          		  {.text = (char *)"Hide",    .disabled = 0, .checked = 0, .usedCheck = 0, .cb = toggle_show, .context = NULL, .submenu = NULL},
          		  {.text = (char *)"-",       .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL,		.context = NULL, .submenu = NULL},
          		  {.text = (char *)"Quit",    .disabled = 0, .checked = 0, .usedCheck = 0, .cb = quit_cb,		.context = NULL, .submenu = NULL},
          		  {.text = NULL,              .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL,		.context = NULL, .submenu = NULL}
    }
};
#else
static struct tray tray;
#endif

static void toggle_show(struct tray_menu *item) {
	if(!item->checked) {
		NTLog(SelfThis, Info, "Called : OpenNetLink Hide (value: %s)", item->text);
		item->text = (char*)"Show";
#if TRAY_APPINDICATOR
		gtk_widget_hide(_g_window);
#elif TRAY_APPKIT
		[_g_window orderOut:(id)SelfThis];
#elif TRAY_WINAPI
		::ShowWindow(messageLoopRootWindowHandle, SW_HIDE);
#endif
	}
	else if(item->checked) {
		NTLog(SelfThis, Info, "Called : OpenNetLink Show (value: %s)", item->text);
		item->text = (char*)"Hide";
#if TRAY_APPINDICATOR
		gtk_widget_show_all(_g_window);
#elif TRAY_APPKIT
		[_g_window makeKeyAndOrderFront:(id)SelfThis];
		[NSApp activateIgnoringOtherApps:YES];
#elif TRAY_WINAPI
		::ShowWindow(messageLoopRootWindowHandle, SW_RESTORE);
		::ShowWindow(messageLoopRootWindowHandle, SW_SHOW);
#endif
	}
	item->checked = !item->checked;
	tray_update(&tray);
}

static void toggle_minimize(struct tray_menu *item) {
	if(!item->checked) {
		NTLog(SelfThis, Info, "Called : OpenNetLink Minimize Change State Hide -> Show (value: %s)", item->text);
		item->text = (char*)"Show";
	}
	else if(item->checked) {
		NTLog(SelfThis, Info, "Called : OpenNetLink Minimize Change State Show -> Hide (value: %s)", item->text);
		item->text = (char*)"Hide";
	}
	item->checked = !item->checked;
	tray_update(&tray);
}

static void toggle_cb(struct tray_menu *item) {
	NTLog(SelfThis, Info, "Called : OpenNetLink Toggle CB (value: %s)", item->text);
	item->checked = !item->checked;
	tray_update(&tray);
}

static void hello_cb(struct tray_menu *item) {
	(void)item;
	NTLog(SelfThis, Info, "Called : OpenNetLink Hello CB (value: %s)", item->text);
	if (strcmp(tray.icon, TRAY_ICON1) == 0) {
		tray.icon = (char*)TRAY_ICON2;
	} else {
		tray.icon = (char*)TRAY_ICON1;
	}
	tray_update(&tray);
}

static void quit_cb(struct tray_menu *item) {
	(void)item;
	NTLog(SelfThis, Info, "Called : OpenNetLink Exit CB (value: %s)", item->text);
	tray_exit();
}

static void submenu_cb(struct tray_menu *item) {
	(void)item;
	NTLog(SelfThis, Info, "Called : OpenNetLink SubMenu CB (value: %s)", item->text);
	tray_update(&tray);
}