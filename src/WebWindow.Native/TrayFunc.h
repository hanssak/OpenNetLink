#include "Tray.h"
#include "NativeLog.h"

#if TRAY_APPINDICATOR
GtkWidget* _g_window = nullptr;
#endif

static void toggle_cb(struct tray_menu *item);
static void toggle_show(struct tray_menu *item);
static void hello_cb(struct tray_menu *item);
static void quit_cb(struct tray_menu *item);
static void submenu_cb(struct tray_menu *item);

// Test tray init
#if 0
static struct tray tray = {
    .icon = TRAY_ICON1,
    .menu = (struct tray_menu[]) {
            {.text = "About", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = hello_cb},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Hide", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = toggle_show},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Quit", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = quit_cb},
            {.text = NULL, .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL}}
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
#elif TRAY_WINAPI
		::ShowWindow(messageLoopRootWindowHandle, SW_SHOW);
#endif
	}
	item->checked = !item->checked;
	tray_update(&tray);
}

static void toggle_cb(struct tray_menu *item) {
	std::string strLog;
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