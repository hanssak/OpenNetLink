#include "Tray.h"

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
		std::string strLog;
		strLog  = "Called : OpenNetLink Hide (value:";
		strLog += item->text;
		strLog += ")";
		NTLOG(Info, strLog.data());
		item->text = "Show";
	#if TRAY_APPINDICATOR
		gtk_widget_hide(_g_window);
	#endif
	}
	else if(item->checked) {
		std::string strLog;
		strLog  = "Called : OpenNetLink Show (value:";
		strLog += item->text;
		strLog += ")";
		NTLOG(Info, strLog.data());
		item->text = "Hide";
	#if TRAY_APPINDICATOR
		gtk_widget_show_all(_g_window);
	#endif
	}
	item->checked = !item->checked;
	tray_update(&tray);
}

static void toggle_cb(struct tray_menu *item) {
	std::string strLog;
	strLog  = "Called : OpenNetLink Toggle CB (value:";
	strLog += item->text;
	strLog += ")";
	NTLOG(Info, strLog.data());
	item->checked = !item->checked;
	tray_update(&tray);
}

static void hello_cb(struct tray_menu *item) {
	(void)item;
	std::string strLog;
	strLog  = "Called : OpenNetLink Hello CB (value:";
	strLog += item->text;
	strLog += ")";
	NTLOG(Info, strLog.data());
	if (strcmp(tray.icon, TRAY_ICON1) == 0) {
		tray.icon = TRAY_ICON2;
	} else {
		tray.icon = TRAY_ICON1;
	}
	tray_update(&tray);
}

static void quit_cb(struct tray_menu *item) {
	(void)item;
	std::string strLog;
	strLog  = "Called : OpenNetLink Exit CB (value:";
	strLog += item->text;
	strLog += ")";
	NTLOG(Info, strLog.data());
	tray_exit();
}

static void submenu_cb(struct tray_menu *item) {
	(void)item;
	std::string strLog;
	strLog  = "Called : OpenNetLink SubMenu CB (value:";
	strLog += item->text;
	strLog += ")";
	NTLOG(Info, strLog.data());
	tray_update(&tray);
}