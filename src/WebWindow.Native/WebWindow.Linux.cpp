// For this to build on WSL (Ubuntu 18.04) you need to:
//  sudo apt-get install libgtk-3-dev libwebkit2gtk-4.0-dev
#ifdef OS_LINUX
#include "WebWindow.h"
#include <mutex>
#include <condition_variable>
#include <X11/Xlib.h>
#include <webkit2/webkit2.h>
#include <JavaScriptCore/JavaScript.h>
#include <gdk-pixbuf/gdk-pixdata.h>
#include <keybinder.h>
#include <sstream>
#include <iomanip>
#include <vector>
#include <iconv.h>

void *SelfThis = nullptr;

#include "NativeLog.h"
#include "TrayFunc.h"

#include "TextEncDetect.h"
using namespace AutoIt::Common;

bool g_bDoExit2TrayUse = false;
bool g_bStartTray = true;
bool g_bClipCopyNsend = false;

std::map<int, wstring> mapHotKey;


std::mutex invokeLockMutex;



/*
		enum Encoding
		{
			NONE,				// Unknown or binary
			ANSI,				// 0-255
			ASCII,				// 0-127
			UTF8_BOM,			// UTF8 with BOM
			UTF8_NOBOM,			// UTF8 without BOM
			UTF16_LE_BOM,		// UTF16 LE with BOM
			UTF16_LE_NOBOM,		// UTF16 LE without BOM
			UTF16_BE_BOM,		// UTF16-BE with BOM
			UTF16_BE_NOBOM,		// UTF16-BE without BOM
		};
*/
char *GetCharSet(int nCharId)
{
	switch(nCharId)
	{
		case TextEncodingDetect::ANSI			:	// 0-255
			return "EUC-KR";
		case TextEncodingDetect::ASCII			:	// 0-127
			return "EUC-KR";
		case TextEncodingDetect::UTF8_BOM		:	// UTF8 with BOM
			return "UTF-8";
		case TextEncodingDetect::UTF8_NOBOM		:	// UTF8 without BOM
			return "UTF-8";
		case TextEncodingDetect::UTF16_LE_BOM	:	// UTF16 LE with BOM
			return "UTF-16LE";
		case TextEncodingDetect::UTF16_LE_NOBOM	:	// UTF16 LE without BOM
			return "UTF-16LE";
		case TextEncodingDetect::UTF16_BE_BOM	:	// UTF16-BE with BOM
			return "UTF-16BE";
		case TextEncodingDetect::UTF16_BE_NOBOM	:	// UTF16-BE without BOM
			return "UTF-16BE";
		case TextEncodingDetect::NONE           :    // Unknown or binary
		default :
			return "ENC_NONE";
	}
}

int GetEncoding(char *pString)
{
	// Detect the encoding
	TextEncodingDetect textDetect;
	TextEncodingDetect::Encoding encoding = textDetect.DetectEncoding((const unsigned char*)pString, strlen(pString));
	return encoding;
} 

bool ChangeCharset(char *szSrcCharset, char *szDstCharset, char *szSrc, int nSrcLength, char *szDst, int nDstLength)
{
	iconv_t it = iconv_open(szDstCharset, szSrcCharset);
	if (it == (iconv_t)(-1)) 
		return false;

	bool result = true;
	size_t nSrcStrLen = nSrcLength;
	size_t nDstStrLen = nDstLength;
	size_t cc = iconv(it, &szSrc, &nSrcStrLen, &szDst, &nDstStrLen);
	if (cc == (size_t)(-1))
		result = false;

	if (iconv_close(it) == -1)
		result = false;

	return result;
}

struct InvokeWaitInfo
{
	ACTION callback;
	std::condition_variable completionNotifier;
	bool isCompleted;
};

struct InvokeJSWaitInfo
{
	bool isCompleted;
};

gboolean on_widget_deleted(GtkWidget *widget, GdkEvent *event, gpointer self);
void on_size_allocate(GtkWidget* widget, GdkRectangle* allocation, gpointer self);
gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, gpointer self);
gboolean on_window_state_event(GtkWidget *widget, GdkEventWindowState *event, gpointer self);
static gboolean webViewLoadFailed(WebKitWebView *webView, 
			   WebKitLoadEvent loadEvent, 
			   const char *failingURI, 
			   GError *error, void *window);
gboolean loadFailedWithTLSerrors (WebKitWebView       *web_view,
               gchar               *failingURI,
               GTlsCertificate     *certificate,
               GTlsCertificateFlags errors,
               gpointer             user_data);
static GtkWidget *createInfoBarQuestionMessage(const char *title, const char *text);
static void tlsErrorsDialogResponse(GtkWidget *dialog, gint response, gpointer user_data);
static void activate_navigate_uri(GSimpleAction *simple, GVariant *parameter, gpointer user_data);

const GActionEntry action_entries[] = {
	{ "navigate-uri", activate_navigate_uri, "s", NULL, NULL}
};

WebWindow::WebWindow(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback) : _webview(nullptr)
{
	SelfThis = this;
	_webMessageReceivedCallback = webMessageReceivedCallback;
	g_bDoExit2TrayUse = false;

	// It makes xlib thread safe.
	// Needed for get_position.
	XInitThreads();

	gtk_init(0, NULL);
 	keybinder_init();

	_app = gtk_application_new("hanssak.webwindow.open.netlink", G_APPLICATION_FLAGS_NONE);
	g_application_register(G_APPLICATION(_app), NULL, NULL);
	_window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
	_g_window = _window;
	gtk_window_set_default_size(GTK_WINDOW(_window), WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT);
	SetTitle(title);

	if (parent == NULL)
	{
		g_signal_connect(G_OBJECT(_window), "destroy",
			G_CALLBACK(+[](GtkWidget* w, gpointer arg) { gtk_main_quit(); }),
			this);
		g_signal_connect(G_OBJECT(_window), "delete-event", 
			G_CALLBACK(on_widget_deleted), 
			this);
		g_signal_connect(G_OBJECT(_window), "size-allocate",
			G_CALLBACK(on_size_allocate),
			this);
		g_signal_connect(G_OBJECT(_window), "configure-event",
			G_CALLBACK(on_configure_event),
			this);
		g_signal_connect(G_OBJECT(_window), "window-state-event", 
			G_CALLBACK(on_window_state_event), 
			this);
	}

	/*
	tray.icon = TRAY_ICON1;
	tray.menu = (struct tray_menu *)malloc(sizeof(struct tray_menu)*8);
    tray.menu[0] = {"About",0,0,0,hello_cb,NULL,NULL};
    tray.menu[1] = {"-",0,0,0,NULL,NULL,NULL};
    tray.menu[2] = {"Hide",0,0,0,toggle_show,NULL,NULL};
    tray.menu[3] = {"-",0,0,0,NULL,NULL,NULL};
    tray.menu[4] = {"Quit",0,0,0,quit_cb,NULL,NULL};
    tray.menu[5] = {NULL,0,0,0,NULL,NULL,NULL};
	*/
	/*
            {.text = "About", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = hello_cb},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Hide", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = toggle_show},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Quit", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = quit_cb},
            {.text = NULL, .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL}}
	*/

	// It is used in the notification button. (ShowUserNotification Method)
	g_action_map_add_action_entries (G_ACTION_MAP (_app), action_entries, G_N_ELEMENTS (action_entries), this);
}

gboolean on_widget_deleted(GtkWidget *widget, GdkEvent *event, gpointer self)
{
	if (g_bDoExit2TrayUse == false)
	{
		NTLog(self, Info, "Called : OpenNetLink Exit");
		tray_exit();
	}
	else
	{
   		((WebWindow *)self)->MoveWebWindowToTray();
	}
	
    return TRUE;
}

static void
activate_navigate_uri(GSimpleAction *simple, GVariant *parameter, gpointer self)
{
	const gchar *navURI = g_variant_get_string(parameter, NULL);
	NTLog(self, Info, "Called : Action Navigate URI->(%s)", (AutoString)navURI);
	((WebWindow*)self)->InvokeRequestedNavigateURL((AutoString)navURI);
}

WebWindow::~WebWindow()
{
	gtk_widget_destroy(_window);
	if(tray.menu) free(tray.menu);
}

void HandleWebMessage(WebKitUserContentManager* contentManager, WebKitJavascriptResult* jsResult, gpointer arg)
{
	JSCValue* jsValue = webkit_javascript_result_get_js_value(jsResult);
	if (jsc_value_is_string(jsValue)) {
		AutoString str_value = jsc_value_to_string(jsValue);

		WebMessageReceivedCallback callback = (WebMessageReceivedCallback)arg;
		callback(str_value);
		g_free(str_value);
	}

	webkit_javascript_result_unref(jsResult);
}

void WebWindow::Show()
{
	if (!_webview)
	{
		WebKitUserContentManager* contentManager = webkit_user_content_manager_new();
		_webview = webkit_web_view_new_with_user_content_manager(contentManager);
		gtk_container_add(GTK_CONTAINER(_window), _webview);

		WebKitWebContext *webContext = webkit_web_view_get_context(WEBKIT_WEB_VIEW(_webview));
		webkit_web_context_set_tls_errors_policy(webContext, WEBKIT_TLS_ERRORS_POLICY_IGNORE);
		g_signal_connect(WEBKIT_WEB_VIEW(_webview), "load-failed", G_CALLBACK(webViewLoadFailed), this);
		g_signal_connect(WEBKIT_WEB_VIEW(_webview), "load-failed-with-tls-errors", G_CALLBACK (loadFailedWithTLSerrors), this);

		WebKitUserScript* script = webkit_user_script_new(
			"window.__receiveMessageCallbacks = [];"
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
			"};", WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES, WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START, NULL, NULL);
		webkit_user_content_manager_add_script(contentManager, script);
		webkit_user_script_unref(script);

		g_signal_connect(contentManager, "script-message-received::webwindowinterop",
			G_CALLBACK(HandleWebMessage), (void*)_webMessageReceivedCallback);
		webkit_user_content_manager_register_script_message_handler(contentManager, "webwindowinterop");
	}

	if (g_bStartTray == false)
	gtk_widget_show_all(_window);

	/* Enable the developer extras */
	WebKitSettings *settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(_webview));
    webkit_settings_set_enable_webgl(settings, TRUE);
    webkit_settings_set_enable_media_stream(settings, TRUE);
	webkit_settings_set_default_font_size (settings, webkit_settings_get_default_font_size (settings)-3);

	/* Load some data or reload to be able to inspect the page*/
	if (!access("./debug-inspector", F_OK))
	{
		webkit_settings_set_enable_developer_extras(settings, TRUE);
		webkit_web_view_load_uri(WEBKIT_WEB_VIEW(_webview), "http://www.gnome.org");
	}

	/* Show the inspector */
	WebKitWebInspector* inspector = webkit_web_view_get_inspector(WEBKIT_WEB_VIEW(_webview));
	webkit_web_inspector_show(WEBKIT_WEB_INSPECTOR(inspector));
}

static gboolean webViewLoadFailed(WebKitWebView *webView, WebKitLoadEvent loadEvent, 
								const char *failingURI, GError *error, void *window)
{
    //gtk_entry_set_progress_fraction(GTK_ENTRY(window->uriEntry), 0.);
    return FALSE;
}

gboolean loadFailedWithTLSerrors (WebKitWebView       *web_view,
               gchar               *failingURI,
               GTlsCertificate     *certificate,
               GTlsCertificateFlags errors,
               gpointer             user_data) {

    WebWindow *window = (WebWindow *) user_data;

    gchar *text = g_strdup_printf("Failed to load %s: Do you want to continue ignoring the TLS errors?", failingURI);
    GtkWidget *dialog = createInfoBarQuestionMessage("Invalid TLS Certificate", text);
    g_free(text);
    g_object_set_data_full(G_OBJECT(dialog), "failingURI", g_strdup(failingURI), g_free);
    g_object_set_data_full(G_OBJECT(dialog), "certificate", g_object_ref(certificate), g_object_unref);

    g_signal_connect(dialog, "response", G_CALLBACK(tlsErrorsDialogResponse), web_view);

    gtk_box_pack_start(GTK_BOX(window), dialog, FALSE, FALSE, 0);
    gtk_box_reorder_child(GTK_BOX(window), dialog, 0);
    gtk_widget_show(dialog);

    return TRUE;
}

static GtkWidget *createInfoBarQuestionMessage(const char *title, const char *text)
{
    GtkWidget *dialog = gtk_info_bar_new_with_buttons("No", GTK_RESPONSE_NO, "Yes", GTK_RESPONSE_YES, NULL);
    gtk_info_bar_set_message_type(GTK_INFO_BAR(dialog), GTK_MESSAGE_QUESTION);

    GtkWidget *contentBox = gtk_info_bar_get_content_area(GTK_INFO_BAR(dialog));
    gtk_orientable_set_orientation(GTK_ORIENTABLE(contentBox), GTK_ORIENTATION_VERTICAL);
    gtk_box_set_spacing(GTK_BOX(contentBox), 0);

    GtkWidget *label = gtk_label_new(NULL);
    gchar *markup = g_strdup_printf("<span size='xx-large' weight='bold'>%s</span>", title);
    gtk_label_set_markup(GTK_LABEL(label), markup);
    g_free(markup);
    gtk_label_set_line_wrap(GTK_LABEL(label), TRUE);
    gtk_label_set_selectable(GTK_LABEL(label), TRUE);
    //gtk_misc_set_alignment(GTK_MISC(label), 0., 0.5);
    gtk_label_set_xalign(GTK_LABEL(label), 0.);
    gtk_label_set_yalign(GTK_LABEL(label), 0.5);
    gtk_box_pack_start(GTK_BOX(contentBox), label, FALSE, FALSE, 2);
    gtk_widget_show(label);

    label = gtk_label_new(text);
    gtk_label_set_line_wrap(GTK_LABEL(label), TRUE);
    gtk_label_set_selectable(GTK_LABEL(label), TRUE);
    //gtk_misc_set_alignment(GTK_MISC(label), 0., 0.5);
    gtk_label_set_xalign(GTK_LABEL(label), 0.);
    gtk_label_set_yalign(GTK_LABEL(label), 0.5);
    gtk_box_pack_start(GTK_BOX(contentBox), label, FALSE, FALSE, 0);
    gtk_widget_show(label);

    return dialog;
}

static void tlsErrorsDialogResponse(GtkWidget *dialog, gint response, gpointer user_data)
{
	WebKitWebView       *web_view = (WebKitWebView *) user_data;

    if (response == GTK_RESPONSE_YES) {
        const char *failingURI = (const char *)g_object_get_data(G_OBJECT(dialog), "failingURI");
        GTlsCertificate *certificate = (GTlsCertificate *)g_object_get_data(G_OBJECT(dialog), "certificate");
        SoupURI *uri = soup_uri_new(failingURI);
        webkit_web_context_allow_tls_certificate_for_host(webkit_web_view_get_context(web_view), certificate, uri->host);
        soup_uri_free(uri);
        webkit_web_view_load_uri(web_view, failingURI);
    }
    gtk_widget_destroy(dialog);
}

void WebWindow::SetTitle(AutoString title)
{
	gtk_window_set_title(GTK_WINDOW(_window), title);
}

void WebWindow::SetTrayStartUse(bool bUseStartTray)
{
	g_bStartTray = bUseStartTray;
	NTLog(this, Info, "Called : OpenNetLink SetTrayStartUse : %s", (AutoString)bUseStartTray ? "Yes" : "No");
}

void WebWindow::SetTrayUse(bool useTray)
{
    g_bDoExit2TrayUse = useTray;
    NTLog(this, Info, "Called : SetTrayUse(@@@@@@@@@@) : %s", (AutoString)(g_bDoExit2TrayUse ? "Yes": "No") );
}	

void WebWindow::ClipTypeSelect(int groupID)
{
	
}

void WebWindow::ClipFirstSendTypeText(int groupID)
{

}

void WebWindow::WaitForExit()
{
	//gtk_main();

	if (tray_init(&tray) < 0)
	{
		// printf("failed to create tray\n");
		NTLog(this, Fatal, "Failed to Create Tray\n");
		return ;
	}

	if (!g_bStartTray)
		MoveTrayToWebWindow();

	while (tray_loop(1) == 0)
	{
		// printf("iteration\n");
	}

	/* Self Force Kill */
	kill(getpid(), SIGTERM); /* because of do not exit */
}

static gboolean invokeCallback(gpointer data)
{
	InvokeWaitInfo* waitInfo = (InvokeWaitInfo*)data;
	waitInfo->callback();
	{
		std::lock_guard<std::mutex> guard(invokeLockMutex);
		waitInfo->isCompleted = true;
	}
	waitInfo->completionNotifier.notify_one();
	return false;
}

void WebWindow::Invoke(ACTION callback)
{
	InvokeWaitInfo waitInfo = { };
	waitInfo.callback = callback;
	gdk_threads_add_idle(invokeCallback, &waitInfo);

	// Block until the callback is actually executed and completed
	// TODO: Add return values, exception handling, etc.
	std::unique_lock<std::mutex> uLock(invokeLockMutex);
	waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}

void WebWindow::ShowMessage(AutoString title, AutoString body, unsigned int type)
{
	GtkWidget* dialog = gtk_message_dialog_new(GTK_WINDOW(_window),
		GTK_DIALOG_DESTROY_WITH_PARENT,
		GTK_MESSAGE_OTHER,
		GTK_BUTTONS_OK,
		"%s",
		body);
	gtk_window_set_title((GtkWindow*)dialog, title);
	gtk_dialog_run(GTK_DIALOG(dialog));
	gtk_widget_destroy(dialog);
}

void WebWindow::NavigateToUrl(AutoString url)
{
	webkit_web_view_load_uri(WEBKIT_WEB_VIEW(_webview), url);
}

void WebWindow::NavigateToString(AutoString content)
{
	webkit_web_view_load_html(WEBKIT_WEB_VIEW(_webview), content, NULL);
}

// From https://stackoverflow.com/a/33799784
std::string escape_json(const std::string& s) {
	std::ostringstream o;
	for (auto c = s.cbegin(); c != s.cend(); c++) {
		switch (*c) {
		case '"': o << "\\\""; break;
		case '\\': o << "\\\\"; break;
		case '\b': o << "\\b"; break;
		case '\f': o << "\\f"; break;
		case '\n': o << "\\n"; break;
		case '\r': o << "\\r"; break;
		case '\t': o << "\\t"; break;
		default:
			if ('\x00' <= *c && *c <= '\x1f') {
				o << "\\u"
					<< std::hex << std::setw(4) << std::setfill('0') << (int)*c;
			}
			else {
				o << *c;
			}
		}
	}
	return o.str();
}

static void webview_eval_finished(GObject* object, GAsyncResult* result, gpointer userdata) {
	InvokeJSWaitInfo* waitInfo = (InvokeJSWaitInfo*)userdata;
	waitInfo->isCompleted = true;
}

void WebWindow::SendMessage(AutoString message)
{
	std::string js;
	js.append("__dispatchMessageCallback(\"");
	js.append(escape_json(message));
	js.append("\")");

	InvokeJSWaitInfo invokeJsWaitInfo = {};
	webkit_web_view_run_javascript(WEBKIT_WEB_VIEW(_webview),
		js.c_str(), NULL, webview_eval_finished, &invokeJsWaitInfo);
	while (!invokeJsWaitInfo.isCompleted) {
		g_main_context_iteration(NULL, TRUE);
	}
}

void WebWindow::ShowUserNotification(AutoString image, AutoString title, AutoString message, AutoString navURI)
{
	GNotification *notification;
	GFile *file;
	GIcon *icon;

	//GtkClipboard* clipboard = gtk_clipboard_get(GDK_SELECTION_CLIPBOARD);
	//gtk_clipboard_request_text(clipboard, text_request_callback, message);

	notification = g_notification_new (title);
	g_notification_set_body (notification, message);
	file = g_file_new_for_path (image);
	icon = g_file_icon_new (file);
	g_notification_set_icon (notification, G_ICON (icon));
	g_object_unref (icon);
	g_object_unref (file);

	if (navURI) g_notification_add_button_with_target (notification, "페이지 이동", "app.navigate-uri", "s", navURI);

	g_application_send_notification (G_APPLICATION(_app), title, notification);
	g_object_unref (notification);
}

void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, gpointer user_data)
{
	WebResourceRequestedCallback webResourceRequestedCallback = (WebResourceRequestedCallback)user_data;

	const gchar* uri = webkit_uri_scheme_request_get_uri(request);
	int numBytes;
	AutoString contentType;
	void* dotNetResponse = webResourceRequestedCallback((AutoString)uri, &numBytes, &contentType);
	GInputStream* stream = g_memory_input_stream_new_from_data(dotNetResponse, numBytes, NULL);
	webkit_uri_scheme_request_finish(request, (GInputStream*)stream, -1, contentType);
	g_object_unref(stream);
	delete[] contentType;
}

void WebWindow::AddCustomScheme(AutoString scheme, WebResourceRequestedCallback requestHandler)
{
	WebKitWebContext* context = webkit_web_context_get_default();
	webkit_web_context_register_uri_scheme(context, scheme,
		(WebKitURISchemeRequestCallback)HandleCustomSchemeRequest,
		(void*)requestHandler, NULL);
}

void WebWindow::SetResizable(bool resizable)
{
	gtk_window_set_resizable(GTK_WINDOW(_window), resizable ? TRUE : FALSE);
}

void WebWindow::GetSize(int* width, int* height)
{
	gtk_window_get_size(GTK_WINDOW(_window), width, height);
}

void WebWindow::SetSize(int width, int height)
{
	gtk_window_resize(GTK_WINDOW(_window), width, height);
}

void on_size_allocate(GtkWidget* widget, GdkRectangle* allocation, gpointer self)
{
	int width, height;
	gtk_window_get_size(GTK_WINDOW(widget), &width, &height);
	((WebWindow*)self)->InvokeResized(width, height);
}

void WebWindow::GetAllMonitors(GetAllMonitorsCallback callback)
{
    if (callback)
    {
        GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(_window));
        GdkDisplay* display = gdk_screen_get_display(screen);
        int n = gdk_display_get_n_monitors(display);
        for (int i = 0; i < n; i++)
        {
            GdkMonitor* monitor = gdk_display_get_monitor(display, i);
            Monitor props = {};
            gdk_monitor_get_geometry(monitor, (GdkRectangle*)&props.monitor);
            gdk_monitor_get_workarea(monitor, (GdkRectangle*)&props.work);
            if (!callback(&props)) break;
        }
    }
}

unsigned int WebWindow::GetScreenDpi()
{
	GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(_window));
	gdouble dpi = gdk_screen_get_resolution(screen);
	if (dpi < 0) return 96;
	else return (unsigned int)dpi;
}

void WebWindow::GetPosition(int* x, int* y)
{
	gtk_window_get_position(GTK_WINDOW(_window), x, y);
}

void WebWindow::SetPosition(int x, int y)
{
	gtk_window_move(GTK_WINDOW(_window), x, y);
}

gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, gpointer self)
{
	if (event->type == GDK_CONFIGURE)
	{
		((WebWindow*)self)->InvokeMoved(event->configure.x, event->configure.y);
	}
	return FALSE;
}

gboolean on_window_state_event(GtkWidget *widget, GdkEventWindowState *event, gpointer self)
{
    //Minimized window check
	if(event->new_window_state & GDK_WINDOW_STATE_ICONIFIED)
    {
    	((WebWindow *)self)->MoveWebWindowToTray();
		gtk_window_deiconify (GTK_WINDOW(((WebWindow *)self)->_window));
    }
    return TRUE;
}

void WebWindow::SetTopmost(bool topmost)
{
	gtk_window_set_keep_above(GTK_WINDOW(_window), topmost ? TRUE : FALSE);
}

void WebWindow::SetIconFile(AutoString filename)
{
	gtk_window_set_icon_from_file(GTK_WINDOW(_window), filename, NULL);
}

static void
request_text_received_func (GtkClipboard     *clipboard,
							GtkSelectionData *selection_data,
							gpointer          data)
{
	gchar *result = NULL;
	ClipBoardParam *pstParm = (ClipBoardParam *)data;

	result = (gchar *) gtk_selection_data_get_text (selection_data);
	if (!result)
	{
		/* If we asked for UTF8 and didn't get it, try compound_text;
		 * if we asked for compound_text and didn't get it, try string;
		 * If we asked for anything else and didn't get it, give up.
		 */
		GdkAtom target = gtk_selection_data_get_target (selection_data);
		if (target == gdk_atom_intern_static_string ("UTF8_STRING"))
		{
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("COMPOUND_TEXT"),
					request_text_received_func, data);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("COMPOUND_TEXT"))
		{
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("STRING"),
					request_text_received_func, data);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("STRING"))
		{
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("TEXT"),
					request_text_received_func, data);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("TEXT"))
		{
			gtk_clipboard_request_contents (clipboard,
					GDK_TARGET_STRING,
					request_text_received_func, data);
			return;
		}
		else
		{
			NTLog(pstParm->self, Warning, "WARN: Don't judged Text Type !!!!!\n");
			return;
		}
	}

	if (result)
	{
		// printf("recv func: %s\n", result);
		// Send Clipboard Text Transfer
		/*
			public enum CLIPTYPE : int
			{
				TEXT = 1,
				IMAGE = 2,
				OBJECT = 3,
			}
		*/

		((WebWindow*)(pstParm->self))->InvokeClipBoard(pstParm->nGroupId, D_CLIP_TEXT, strlen(result), result, 0, NULL);
		if (result) g_free (result);
	}
}

static void
request_image_received_func (GtkClipboard     *clipboard,
							 GtkSelectionData *selection_data,
							 gpointer          data)
{
	GdkPixbuf *result = NULL;
	ClipBoardParam *pstParm = (ClipBoardParam *)data;

	result = gtk_selection_data_get_pixbuf (selection_data);
	if (!result)
	{
		/* If we asked for image/png and didn't get it, try image/jpeg;
		 * if we asked for image/jpeg and didn't get it, try image/gif;
		 * if we asked for image/gif and didn't get it, try image/bmp;
		 * If we asked for anything else and didn't get it, give up.
		 */
		GdkAtom target = gtk_selection_data_get_target (selection_data);
		if (target == gdk_atom_intern_static_string ("image/png"))
		{
			strcpy(pstParm->szExt, "jpeg");
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/jpeg"),
					request_image_received_func, pstParm);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("image/jpeg"))
		{
			strcpy(pstParm->szExt, "gif");
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/gif"),
					request_image_received_func, pstParm);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("image/gif"))
		{
			strcpy(pstParm->szExt, "bmp");
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/bmp"),
					request_image_received_func, pstParm);
			return;
		}
		else
		{
			NTLog(pstParm->self, Warning, "WARN: Don't judged Image Type !!!!!\n");
			return;
		}
	}

	if (result)
	{
		gchar szFileName[256];
		gint64 timeVal = g_get_real_time();

		char tBuff[64];
		time_t now = time (0);
		strftime (tBuff, 100, "%Y-%m-%d%H:%M:%S.000", localtime (&now));

		//sprintf(szFileName, "/tmp/%s.%s", tBuff, pstParm->szExt);
		sprintf(szFileName, "/tmp/%s.%s", tBuff, "bmp");
		printf("dest: %s\n", szFileName);
 		//gdk_pixbuf_save(result, szFileName, (gchar *)pstParm->szExt, NULL, NULL);
 		gdk_pixbuf_save(result, szFileName, (gchar *)"bmp", NULL, NULL);

		gsize BufferSize = gdk_pixbuf_get_byte_length(result);
		gchar *ImageBuffer = (gchar *)g_malloc0(BufferSize);
		gdk_pixbuf_save_to_buffer (result, &ImageBuffer, &BufferSize, (gchar *)"bmp", NULL, NULL);
		// Send ClipBoard Image Transfer
		/*
			public enum CLIPTYPE : int
			{
				TEXT = 1,
				IMAGE = 2,
				OBJECT = 3,
			}
		*/
		((WebWindow*)(pstParm->self))->InvokeClipBoard(pstParm->nGroupId, D_CLIP_IMAGE, BufferSize, ImageBuffer, 0, NULL);
		// Just Test : Recv ClipBoard
		//((WebWindow*)(pstParm->self))->SetClipBoard(D_CLIP_IMAGE, BufferSize, ImageBuffer);
		g_free(ImageBuffer);
		g_object_unref (result);
	}
}

static void
request_uris_received_func (GtkClipboard     *clipboard,
                            GtkSelectionData *selection_data,
                            gpointer          data)
{
	int i=0;
	gchar **uris;
	ClipBoardParam *pstParm = (ClipBoardParam *)data;
	uris = gtk_selection_data_get_uris (selection_data);

	do
	{
		printf("Recv URIS: %s\n", uris[i]);
		// TODO: Data Transfer Uris
	} while(uris[++i] != NULL);

	g_strfreev (uris);
}

static void
request_rich_text_received_func (GtkClipboard     *clipboard,
                                 GtkSelectionData *selection_data,
                                 gpointer          data)
{
	gsize length = 0;
	guint8 *result = NULL;
	ClipBoardParam *pstParm = (ClipBoardParam *)data;

	result = (guint8 *) gtk_selection_data_get_data (selection_data);
	if (!result)
	{
		/* If we asked for text/html and didn't get it, try text/plain;
		 * if we asked for text/plain and didn't get it, try Rich Text Format;
		 * If we asked for anything else and didn't get it, give up.
		 */
		GdkAtom target = gtk_selection_data_get_target (selection_data);
		if (target == gdk_atom_intern_static_string ("text/html;charset=utf-8"))
		{
			gtk_clipboard_request_contents (clipboard, 
					gdk_atom_intern_static_string ("text/plain;charset=utf-8"),
					request_rich_text_received_func, data);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("text/plain;charset=utf-8"))
		{
			gtk_clipboard_request_contents (clipboard, 
					gdk_atom_intern_static_string ("text/html"),
					request_rich_text_received_func, data);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("text/html"))
		{
			gtk_clipboard_request_contents (clipboard, 
					gdk_atom_intern_static_string ("text/plain"),
					request_rich_text_received_func, data);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("text/plain"))
		{
			gtk_clipboard_request_contents (clipboard, 
					gdk_atom_intern_static_string ("Rich Text Format"),
					request_rich_text_received_func, data);
			return;
		}
		else
		{
			NTLog(pstParm->self, Warning, "WARN: Don't judged Rich Text Type !!!!!\n");
			return;
		}
	}

	// Data Transfer rich text
	length = gtk_selection_data_get_length (selection_data);
	// do not needed free result
	((WebWindow*)(pstParm->self))->InvokeClipBoard(pstParm->nGroupId, D_CLIP_TEXT, length, result, 0, NULL);
}

/*
		Gdk.Atom.intern("TIMESTAMP", False), 
		Gdk.Atom.intern("TARGETS", False), 
		Gdk.Atom.intern("MULTIPLE", False), 
		Gdk.Atom.intern("SAVE_TARGETS", False), 
		Gdk.Atom.intern("text/html", False), 
		Gdk.Atom.intern("text/_moz_htmlinfo", False), 
		Gdk.Atom.intern("text/_moz_htmlcontext", False), 
		Gdk.Atom.intern("image/png", False), 
		Gdk.Atom.intern("image/bmp", False), 
		Gdk.Atom.intern("image/x-bmp", False), 
		Gdk.Atom.intern("image/x-MS-bmp", False), 
		Gdk.Atom.intern("image/x-icon", False), 
		Gdk.Atom.intern("image/x-ico", False), 
		Gdk.Atom.intern("image/x-win-bitmap", False), 
		Gdk.Atom.intern("image/vnd.microsoft.icon", False), 
		Gdk.Atom.intern("application/ico", False), 
		Gdk.Atom.intern("image/ico", False), 
		Gdk.Atom.intern("image/icon", False), 
		Gdk.Atom.intern("text/ico", False), 
		Gdk.Atom.intern("image/jpeg", False), 
		Gdk.Atom.intern("image/tiff", False)]), 
*/
void ClipBoardReceivedFunc(GtkClipboard *clipboard, GtkSelectionData *selection_data, gpointer data)
{
	GdkAtom target = gtk_selection_data_get_target (selection_data);
	ClipBoardParam *pstParm = (ClipBoardParam *)data;

	/* If we asked for UTF8 and didn't get it, try compound_text;
	 * if we asked for compound_text and didn't get it, try string;
	 * If we asked for anything else and didn't get it, give up.
	 */
	if (target == gdk_atom_intern_static_string ("UTF8_STRING"))
	{
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("UTF8_STRING"),
				request_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("COMPOUND_TEXT"))
	{
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("COMPOUND_TEXT"),
				request_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("STRING"))
	{
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("STRING"),
				request_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("TEXT"))
	{
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("TEXT"),
				request_text_received_func, data);
		return;
	}
	else if (target == GDK_TARGET_STRING)
	{
		gtk_clipboard_request_contents (clipboard,
				GDK_TARGET_STRING,
				request_text_received_func, data);
		return;
	}
	/* If we asked for image/png and didn't get it, try image/jpeg;
	 * if we asked for image/jpeg and didn't get it, try image/gif;
	 * if we asked for image/gif and didn't get it, try image/bmp;
	 * If we asked for anything else and didn't get it, give up.
	 */
	else if (target == gdk_atom_intern_static_string ("image/png"))
	{
		strcpy(pstParm->szExt, "png");
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("image/png"),
				request_image_received_func, pstParm);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("image/jpeg"))
	{
		strcpy(pstParm->szExt, "jpeg");
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("image/jpeg"),
				request_image_received_func, pstParm);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("image/gif"))
	{
		strcpy(pstParm->szExt, "gif");
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("image/gif"),
				request_image_received_func, pstParm);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("image/bmp"))
	{
		strcpy(pstParm->szExt, "bmp");
		gtk_clipboard_request_contents (clipboard,
				gdk_atom_intern_static_string ("image/bmp"),
				request_image_received_func, pstParm);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("text/uri-list"))
	{
		gtk_clipboard_request_contents (clipboard, 
				gdk_atom_intern_static_string ("text/uri-list"),
                request_uris_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("text/html;charset=utf-8"))
	{
	 	gtk_clipboard_request_contents (clipboard, 
				gdk_atom_intern_static_string ("text/html;charset=utf-8"),
				request_rich_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("text/plain;charset=utf-8"))
	{
	 	gtk_clipboard_request_contents (clipboard, 
				gdk_atom_intern_static_string ("text/plain;charset=utf-8"),
				request_rich_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("text/html"))
	{
	 	gtk_clipboard_request_contents (clipboard, 
				gdk_atom_intern_static_string ("text/html"),
				request_rich_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string ("text/plain"))
	{
	 	gtk_clipboard_request_contents (clipboard, 
				gdk_atom_intern_static_string ("text/plain"),
				request_rich_text_received_func, data);
		return;
	}
	else if (target == gdk_atom_intern_static_string("Rich Text Format"))
	{
	 	gtk_clipboard_request_contents (clipboard, 
				gdk_atom_intern_static_string("Rich Text Format"),
				request_rich_text_received_func, data);
		return;
	}
}

/*
void ClipBoardHandler(GtkClipboard *clipboard, const gchar *text, gpointer data) {
	NTLog(SelfThis, Info, "In ClipBoardHandler: text = '%s'", text);
}
*/

/*
	In targetCallback: Atom(0. TIMESTAMP)
	In targetCallback: Atom(1. TARGETS)
	In targetCallback: Atom(2. SAVE_TARGETS)
	In targetCallback: Atom(3. MULTIPLE)
	In targetCallback: Atom(4. STRING)
	In targetCallback: Atom(5. UTF8_STRING)
	In targetCallback: Atom(6. TEXT)
	In targetCallback: Atom(7. text/html)
	In targetCallback: Atom(8. text/plain)
*/
void TargetCallback(GtkClipboard *clipboard, GdkAtom *atoms, gint n_atoms, gpointer data)
{
	int i_for;
	ClipBoardParam *pstParm = (ClipBoardParam *)data;

	bool fSelectedString = false;
	bool fSelectedImage = false;
	bool fSelectedRichText = false;
	for(i_for = 0; i_for < n_atoms; i_for++) {
		if ((atoms[i_for] == gdk_atom_intern_static_string ("text/html;charset=utf-8")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("text/plain;charset=utf-8")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("text/html")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("text/plain")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string("Rich Text Format")) 
		)	fSelectedString = true;
	}

	for(i_for = 0; i_for < n_atoms; i_for++) {
		if ((atoms[i_for] == gdk_atom_intern_static_string ("UTF8_STRING"))
			||(atoms[i_for] == gdk_atom_intern_static_string ("TEXT"))
		)
		{
			fSelectedString = false;
			fSelectedRichText = true;
		}
	}

	for(i_for = 0; i_for < n_atoms; i_for++) {
		if ((atoms[i_for] == gdk_atom_intern_static_string ("image/png")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("image/jpeg")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("image/gif")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("image/bmp"))
		)
		{
			fSelectedImage = false;
			fSelectedRichText = true;
		}
	}

	for(i_for = 0; i_for < n_atoms; i_for++) {
		if ((atoms[i_for] == gdk_atom_intern_static_string ("UTF8_STRING")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("COMPOUND_TEXT")) 
			//|| (atoms[i_for] == gdk_atom_intern_static_string ("STRING")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("TEXT"))
			//|| (atoms[i_for] == GDK_TARGET_STRING)
		)
		{
			if (!fSelectedString && (atoms[i_for] == gdk_atom_intern_static_string ("UTF8_STRING"))) 
				fSelectedString = true;
			else if (!fSelectedString && (atoms[i_for] == gdk_atom_intern_static_string ("COMPOUND_TEXT"))) 
				fSelectedString = true;
			else if (!fSelectedString && (atoms[i_for] == gdk_atom_intern_static_string ("STRING"))) 
				fSelectedString = true;
			else if (!fSelectedString && (atoms[i_for] == gdk_atom_intern_static_string ("TEXT"))) 
				fSelectedString = true;
			else if (!fSelectedString && (atoms[i_for] == GDK_TARGET_STRING)) 
				fSelectedString = true;
			else continue;
		}
		else if ((atoms[i_for] == gdk_atom_intern_static_string ("image/png")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("image/jpeg")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("image/gif")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("image/bmp"))
		)
		{
			if (!fSelectedImage && (atoms[i_for] == gdk_atom_intern_static_string ("image/png"))) 
				fSelectedImage = true;
			else if (!fSelectedImage && (atoms[i_for] == gdk_atom_intern_static_string ("image/jpeg"))) 
				fSelectedImage = true;
			else if (!fSelectedImage && (atoms[i_for] == gdk_atom_intern_static_string ("image/gif"))) 
				fSelectedImage = true;
			else if (!fSelectedImage && (atoms[i_for] == gdk_atom_intern_static_string ("image/bmp"))) 
				fSelectedImage = true;
			else continue;
		}
		else if ((atoms[i_for] == gdk_atom_intern_static_string ("text/html;charset=utf-8")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("text/plain;charset=utf-8")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("text/html")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string ("text/plain")) 
			|| (atoms[i_for] == gdk_atom_intern_static_string("Rich Text Format")) 
		)
		{
			if (!fSelectedRichText && (atoms[i_for] == gdk_atom_intern_static_string ("text/html;charset=utf-8"))) 
				fSelectedRichText = true;
			else if (!fSelectedRichText && (atoms[i_for] == gdk_atom_intern_static_string ("text/plain;charset=utf-8"))) 
				fSelectedRichText = true;
			else if (!fSelectedRichText && (atoms[i_for] == gdk_atom_intern_static_string ("text/html"))) 
				fSelectedRichText = true;
			else if (!fSelectedRichText && (atoms[i_for] == gdk_atom_intern_static_string ("text/plain"))) 
				fSelectedRichText = true;
			else if (!fSelectedRichText && (atoms[i_for] == gdk_atom_intern_static_string ("Rich Text Format"))) 
				fSelectedRichText = true;
			else continue;
		}
		else continue;

		NTLog(pstParm->self, Info, "In targetCallback: Atom(%d. %s)\n", i_for, gdk_atom_name(atoms[i_for]));
		gtk_clipboard_request_contents (clipboard, atoms[i_for], ClipBoardReceivedFunc, data);
	}
}

void ClipBoardKeybinderHandler(const char *keystring, void *user_data)
{
	ClipBoardParam *pstParm = (ClipBoardParam *)user_data;
	int nGroupId = pstParm->nGroupId;

	NTLog(pstParm->self, Info, "Called ClipBoardKeybinderHandler, \" %s \" with GroupId(%d)", keystring, nGroupId);
	GdkDisplay *display = gdk_display_get_default();
	GtkClipboard *clipboard =
		gtk_clipboard_get_for_display(display, GDK_SELECTION_CLIPBOARD);
		//gtk_clipboard_get_for_display(display, GDK_SELECTION_PRIMARY);
		//gtk_clipboard_get(GDK_SELECTION_CLIPBOARD);
	//gtk_clipboard_request_text(clipboard, ClipBoardHandler, NULL);
  	gtk_clipboard_request_targets (clipboard, TargetCallback, user_data);

	if (gdk_display_supports_clipboard_persistence(display)) {
		NTLog(pstParm->self, Info, "Saved to ClipBoard Store, Supports clipboard persistence. \" %s \" with GroupId(%d)", keystring, nGroupId);
		gtk_clipboard_store(clipboard);
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

  	// keybinder_unbind(strModifiers.c_str(), NULL);
	keybinder_unbind_all (strModifiers.c_str());

	if (bShift) keybinder_set_use_cooked_accelerators (FALSE);

	_clipboard[groupID].nGroupId = groupID;
	_clipboard[groupID].self = this;
	keybinder_bind(strModifiers.c_str(), ClipBoardKeybinderHandler, &_clipboard[groupID]);
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to activate keybinding\n", strModifiers.c_str());
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

 	// keybinder_unbind(strModifiers.c_str(), NULL);
	keybinder_unbind_all (strModifiers.c_str());
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to deactivate keybinding\n", strModifiers.c_str());
}

void WebWindow::OnHotKey(int groupID)
{
	_clipboard[groupID].nGroupId = groupID;
	_clipboard[groupID].szExt[0] = '\0';
	_clipboard[groupID].self = this;
	ClipBoardKeybinderHandler("GetClipBoard(Send) for Click!", (void*)&_clipboard[groupID]);
}

void WebWindow::RegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{
	std::string strModifiers = "";
	std::string strKeyCode(1, chVKCode);
	if (bAlt)
		strModifiers += "<Alt>";             // Alt 키 조합 (0x0001)
	if (bControl)
		strModifiers += "<Ctrl>";			 // Control 키 조합 (0x0002)
	if (bShift)
		strModifiers += "<Shift>";			 // Shift 키 조합 (0x0004)
	if (bWin)
		strModifiers += "<Super>";			 // Window 키 조합 (0x0008)

	strModifiers += strKeyCode; // Key Code

	// keybinder_unbind(strModifiers.c_str(), NULL);
	keybinder_unbind_all(strModifiers.c_str());

	if (bShift) keybinder_set_use_cooked_accelerators(FALSE);

	_clipboard[groupID].nGroupId = groupID;
	_clipboard[groupID].self = this;
	keybinder_bind(strModifiers.c_str(), ClipBoardKeybinderHandler, &_clipboard[groupID]);
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to activate keybinding\n", strModifiers.c_str());
}

void WebWindow::UnRegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{
	// have to use same parameter of RegisterClipboardHotKey.
	std::string strModifiers = "";
	std::string strKeyCode(1, chVKCode);
	if (bAlt)
		strModifiers += "<Alt>";             // Alt 키 조합 (0x0001)
	if (bControl)
		strModifiers += "<Ctrl>";			 // Control 키 조합 (0x0002)
	if (bShift)
		strModifiers += "<Shift>";			 // Shift 키 조합 (0x0004)
	if (bWin)
		strModifiers += "<Super>";			 // Window 키 조합 (0x0008)

	strModifiers += strKeyCode; // Key Code

	// keybinder_unbind(strModifiers.c_str(), NULL);
	keybinder_unbind_all(strModifiers.c_str());
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to deactivate keybinding\n", strModifiers.c_str());
}

void WebWindow::FolderOpen(AutoString strDownPath)
{
	// 탐색기 Open 하는 로직 필요
	// But Do not needed this function on Linux OS.
}

void WebWindow::SetClipBoard(int groupID,int nType, int nClipSize, void* data)
{
	/* TEXT = 1, IMAGE = 2, OBJECT = 3 */
	NTLog(this, Info, "Called SetClipBoard, Type=%d(%s) Size(%ld)", nType, nType==D_CLIP_TEXT?"TEXT":nType==D_CLIP_IMAGE?"IMAGE":"OBJECT", nClipSize);
	GdkDisplay *display 	= gdk_display_get_default();
	GtkClipboard *clipboard = gtk_clipboard_get_for_display(display, GDK_SELECTION_CLIPBOARD);

	switch(nType)
	{
		case D_CLIP_TEXT:
		{
			/* Set clipboard text */
			gtk_clipboard_set_text (clipboard, (const gchar *)data, nClipSize);
		} break;
		case D_CLIP_IMAGE:
		{
			GdkPixbuf *pixbuf = NULL;

			FILE *fp = fopen("/tmp/hs_clipboard_temporary", "w+");
			fwrite(data, 1, nClipSize, fp);
			fclose(fp);
			pixbuf = gdk_pixbuf_new_from_file ("/tmp/hs_clipboard_temporary", NULL);
			if(pixbuf == NULL) {
				//NTLog(this, Error, "Fail: gdk_pixbuf_new_from_file()");
				NTLog(this, Err, "Fail: gdk_pixbuf_new_from_file()");
				return;
			}

			/* Set clipboard image */
			gtk_clipboard_set_image (clipboard, pixbuf);
			g_object_unref (pixbuf);

			/* Remove temporary file */
			unlink("/tmp/hs_clipboard_temporary");
		} break;
		case D_CLIP_OBJECT:
		default:
		{

		} break;
	}

	if (_recvclipboardCallback != NULL)
		_recvclipboardCallback(groupID);
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
	struct tray_menu *item = tray.menu;
	do
	{
		if (strcmp(item->text, "Hide") == 0) {
			toggle_show(item);
			break;
		}
	} while ((++item)->text != NULL);
}

void WebWindow::MoveTrayToWebWindow()
{
	NTLog(this, Info, "Called : OpenNetLink Move To WebWindow");
	struct tray_menu* item = tray.menu;
	do
	{
		if (strcmp(item->text, "Show") == 0) {
			toggle_show(item);
			break;
		}
	} while ((++item)->text != NULL);
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

void WebWindow::RegisterStartProgram()
{
	int myuid;

	passwd *mypasswd;
	myuid = getuid();
	mypasswd = getpwuid(myuid);

	std::string filePath = std::string(mypasswd->pw_dir) + "/.config/autostart/SecureGate.desktop";

	// write File
	std::ofstream writeFile(filePath.data());
	if( writeFile.is_open() ){
		writeFile << "#!/usr/bin/env xdg-open\n";
		writeFile << "\n";
		writeFile << "[Desktop Entry]\n";
		//writeFile << "Name=SecureGate\n";
		writeFile << "Name=OpenNetLink\n";
		writeFile << "Comment=SecureGate\n";
		writeFile << "GenericName=File Transfer\n";
		writeFile << "Exec=/opt/hanssak/opennetlink/OpenNetLinkApp.sh\n";
		
		//writeFile << "Exec=/bin/sh -c '$HOME/hanssak/OpenNetLinkApp/OpenNetLinkApp.sh'\n";
		//writeFile << "Exec=/bin/sh -c '/data/CrossPlatformWork/OPEN/OpenNetLink/src/OpenNetLinkApp/bin/Debug/netcoreapp3.1/OpenNetLinkApp.sh'\n";
		//writeFile << "Icon=/usr/share/icons/SecureGate.ico\n";
		writeFile << "Icon=/opt/hanssak/opennetlink/wwwroot/SecureGate.ico\n";
		writeFile << "Type=Application\n";
		writeFile << "Categories=Utility;\n";
		writeFile << "Keywords=SecureGate;OpenNetLink;NetLink;\n";
		writeFile << "NoDisplay=false\n";
		writeFile << "X-GNOME-Autostart-enabled=true\n";
		writeFile.close();
		NTLog(this, Info, "Called : RegisterStartProgram, Success: Create File [%s]", filePath.data());
	} else {
		//NTLog(this, Error, "Called : RegisterStartProgram, Fail: Create File [%s] Err[%s]", filePath.data(), strerror(errno));
		NTLog(this, Err, "Called : RegisterStartProgram, Fail: Create File [%s] Err[%s]", filePath.data(), strerror(errno));
	}
}

void WebWindow::UnRegisterStartProgram()
{
	int myuid;

	passwd *mypasswd;
	myuid = getuid();
	mypasswd = getpwuid(myuid);

	std::string filePath = std::string(mypasswd->pw_dir) + "/.config/autostart/SecureGate.desktop";
	if (std::remove(filePath.data()) == 0) // delete file
		NTLog(this, Info, "Called : UnRegisterStartProgram, Success: Remove File [%s]", filePath.data());
	else
		//NTLog(this, Error, "Called : UnRegisterStartProgram, Fail: Remove File [%s] Err[%s]", filePath.data(), strerror(errno));
		NTLog(this, Err, "Called : UnRegisterStartProgram, Fail: Remove File [%s] Err[%s]", filePath.data(), strerror(errno));
}

void WebWindow::SetUseClipCopyNsend(bool bUse)
{
	g_bClipCopyNsend = bUse;
	//NTLog(this, Info, "Called : SetUseClipCopyNsend(@@@@@@@@@@) : %s", (AutoString)(bUse ? "Yes" : "No"));
}

void FreeClipHotKey(int nGroupID)
{
	if (mapHotKey.find(nGroupID) == mapHotKey.end())
	{
		NTLog(SelfThis, Info, "FreeClipHotKey - nGroupID : %d, ClipHotKey Data Empty!", nGroupID);
		return;
	}

	wstring strClipHotKey = mapHotKey[nGroupID];
	if (strClipHotKey.size() == 5)
	{
		wstring strWinKey = strClipHotKey.substr(0, 1);
		wstring strCtrlKey = strClipHotKey.substr(1, 1);
		wstring strAltKey = strClipHotKey.substr(2, 1);
		wstring strShiftKey = strClipHotKey.substr(3, 1);
		wstring strKeyName = strClipHotKey.substr(4, 1);

		Sleep(100);
		if (strKeyName.empty() != true)
			keybd_event((BYTE)strKeyName.data(), 0x98, KEYEVENTF_KEYUP, 0);
		if (strShiftKey.compare(_T("1")) == 0)
			keybd_event(VK_SHIFT, 0x9d, KEYEVENTF_KEYUP, 0);
		if (strCtrlKey.compare(_T("1")) == 0)
			keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_KEYUP, 0);
		if (strAltKey.compare(_T("1")) == 0)
			keybd_event(VK_MENU, 0x9d, KEYEVENTF_KEYUP, 0);
		if (strWinKey.compare(_T("1")) == 0)
		{
			keybd_event(VK_LWIN, 0x9d, KEYEVENTF_KEYUP, 0);
			keybd_event(VK_RWIN, 0x9d, KEYEVENTF_KEYUP, 0);
		}
	}
	else
	{
		NTLog(SelfThis, Info, "FreeClipHotKey - nGroupID : %d, ClipHotKey Data is Wrong(Size Must be 5!) (Data : %s)", nGroupID, (AutoString)strClipHotKey.data());
	}
}

void WebWindow::SetNativeClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{
	wstring strTempHotKey = _T("");

	strTempHotKey += (bWin ? _T("1") : _T("0"));
	strTempHotKey += (bControl ? _T("1") : _T("0"));
	strTempHotKey += (bAlt ? _T("1") : _T("0"));
	strTempHotKey += (bShift ? _T("1") : _T("0"));
	strTempHotKey += chVKCode;

	NTLog(this, Info, "Called - SetNativeClipboardHotKey(###) - GroupID : %d, bAlt : %s, bControl : %s, bShift : %s, bWin : %s, VKCode : %c, nIdx : %d, Sum-Data : %s",
		groupID,
		(AutoString)(bAlt ? L"Y" : L"N"),
		(AutoString)(bControl ? L"Y" : L"N"),
		(AutoString)(bShift ? L"Y" : L"N"),
		(AutoString)(bWin ? L"Y" : L"N"),
		(wchar_t)chVKCode, nIdx,
		(AutoString)strTempHotKey.data());

	mapHotKey[groupID] = strTempHotKey;
}

#endif
