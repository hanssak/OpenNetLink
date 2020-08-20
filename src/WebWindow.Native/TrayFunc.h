#include "Tray.h"

GtkWidget* _g_window = nullptr;

static void toggle_cb(struct tray_menu *item);
static void toggle_show(struct tray_menu *item);
static void hello_cb(struct tray_menu *item);
static void quit_cb(struct tray_menu *item);
static void submenu_cb(struct tray_menu *item);
// Test tray init
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

static void toggle_show(struct tray_menu *item) {
  if(!item->checked) {
   	NTLOG(Info,"Called : OpenNetLink Hide (value:" + item->text + ")");
	item->text = "Show";
	gtk_widget_hide(_g_window);
  }
  else if(item->checked) {
   	NTLOG(Info,"Called : OpenNetLink Show (value:" + item->text + ")");
	item->text = "Hide";
	gtk_widget_show_all(_g_window);
  }
  item->checked = !item->checked;
  tray_update(&tray);
}

static void toggle_cb(struct tray_menu *item) {
  NTLOG(Info,"Called : OpenNetLink Toggle CB (value:" + item->text + ")");
  item->checked = !item->checked;
  tray_update(&tray);
}

static void hello_cb(struct tray_menu *item) {
  (void)item;
  NTLOG(Info,"Called : OpenNetLink Hello CB (value:" + item->text + ")");
  if (strcmp(tray.icon, TRAY_ICON1) == 0) {
    tray.icon = TRAY_ICON2;
  } else {
    tray.icon = TRAY_ICON1;
  }
  tray_update(&tray);
}

static void quit_cb(struct tray_menu *item) {
  (void)item;
  printf("quit cb\n");
  NTLOG(Info,"Called : OpenNetLink Exit CB (value:" + item->text + ")");
  tray_exit();
}

static void submenu_cb(struct tray_menu *item) {
  (void)item;
  NTLOG(Info,"Called : OpenNetLink SubMenu CB (value:" + item->text + ")");
  tray_update(&tray);
}