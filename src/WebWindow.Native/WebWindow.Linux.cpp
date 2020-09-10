// For this to build on WSL (Ubuntu 18.04) you need to:
//  sudo apt-get install libgtk-3-dev libwebkit2gtk-4.0-dev
#ifdef OS_LINUX
#include "WebWindow.h"
#include <mutex>
#include <condition_variable>
#include <X11/Xlib.h>
#include <webkit2/webkit2.h>
#include <JavaScriptCore/JavaScript.h>
#include <sstream>
#include <iomanip>
#include <vector>

void *SelfThis = nullptr;

#include "NativeLog.h"
#include "TrayFunc.h"

std::mutex invokeLockMutex;

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

WebWindow::WebWindow(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback) : _webview(nullptr)
{
	SelfThis = this;
	_webMessageReceivedCallback = webMessageReceivedCallback;
	bTrayUse = false;

	// It makes xlib thread safe.
	// Needed for get_position.
	XInitThreads();

	gtk_init(0, NULL);
 	keybinder_init();

	_app = gtk_application_new("hanssak.webwindow.open.netlink", G_APPLICATION_FLAGS_NONE);
	g_application_register(G_APPLICATION(_app), NULL, NULL);
	_window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
	_g_window = _window;
	gtk_window_set_default_size(GTK_WINDOW(_window), 1280, 800);
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
	}

	tray.icon = TRAY_ICON1;
	tray.menu = (struct tray_menu *)malloc(sizeof(struct tray_menu)*8);
    tray.menu[0] = {"About",0,0,0,hello_cb,NULL,NULL};
    tray.menu[1] = {"-",0,0,0,NULL,NULL,NULL};
    tray.menu[2] = {"Hide",0,0,0,toggle_show,NULL,NULL};
    tray.menu[3] = {"-",0,0,0,NULL,NULL,NULL};
    tray.menu[4] = {"Quit",0,0,0,quit_cb,NULL,NULL};
    tray.menu[5] = {NULL,0,0,0,NULL,NULL,NULL};
	/*
            {.text = "About", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = hello_cb},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Hide", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = toggle_show},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Quit", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = quit_cb},
            {.text = NULL, .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL}}
	*/
}

gboolean on_widget_deleted(GtkWidget *widget, GdkEvent *event, gpointer self)
{
	if (((WebWindow*)self)->bTrayUse == false)
	{
		NTLog(self, Info, "Called : OpenNetLink Exit");
		tray_exit();
	}
	else
	{
		struct tray_menu *item = tray.menu;
		do
		{
			if (strcmp(item->text, "Hide") == 0) {
				toggle_show(item);
				break;
			}
		} while ((++item)->text != NULL);
	}
	
    return TRUE;
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

	gtk_widget_show_all(_window);

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

void WebWindow::WaitForExit()
{
	//gtk_main();

	if (tray_init(&tray) < 0)
	{
		printf("failed to create tray\n");
		return ;
	}
	while (tray_loop(1) == 0)
	{
		//printf("iteration\n");
	}

	/* Self Force Kill */
	kill(getpid(), SIGKILL); /* because of do not exit */
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

void WebWindow::ShowUserNotification(AutoString image, AutoString title, AutoString message)
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
					GDK_TARGET_STRING,
					request_text_received_func, data);
			return;
		}
	}

  	printf("recv func: %s\n", result);
	// Send Clipboard Text Transfer
	/*
		public enum CLIPTYPE : int
		{
			TEXT = 1,
			IMAGE = 2,
			OBJECT = 3,
		}
	*/
	((WebWindow*)(pstParm->self))->InvokeClipBoard(pstParm->nGroupId, D_CLIP_TEXT, strlen(result), result);
	g_free (result);
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
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/png"),
					request_image_received_func, pstParm);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("image/jpeg"))
		{
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/jpeg"),
					request_image_received_func, pstParm);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("image/gif"))
		{
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/gif"),
					request_image_received_func, pstParm);
			return;
		}
		else if (target == gdk_atom_intern_static_string ("image/bmp"))
		{
			gtk_clipboard_request_contents (clipboard,
					gdk_atom_intern_static_string ("image/bmp"),
					request_image_received_func, pstParm);
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
		((WebWindow*)(pstParm->self))->InvokeClipBoard(pstParm->nGroupId, D_CLIP_IMAGE, BufferSize, ImageBuffer);
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
  guint8 *result = NULL;
  gsize length = 0;
  result = (guint8 *) gtk_selection_data_get_data (selection_data);
  length = gtk_selection_data_get_length (selection_data);
  // TODO: Data Transfer rich text
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
void ClipboardReceivedFunc(GtkClipboard *clipboard, GtkSelectionData *selection_data, gpointer data)
{
	GdkAtom target = gtk_selection_data_get_target (selection_data);
	ClipBoardParam *pstParm = (ClipBoardParam *)data;

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

	for(i_for = 0; i_for < n_atoms; i_for++) {
		NTLog(pstParm->self, Info, "In targetCallback: Atom(%d. %s)\n", i_for, gdk_atom_name(atoms[i_for]));
		gtk_clipboard_request_contents (clipboard, atoms[i_for], ClipboardReceivedFunc, data);
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

  	keybinder_unbind(strModifiers.c_str(), NULL);

	if (bShift) keybinder_set_use_cooked_accelerators (FALSE);

	_clipboard[groupID].nGroupId = groupID;
	_clipboard[groupID].self = this;
	keybinder_bind(strModifiers.c_str(), ClipBoardKeybinderHandler, &_clipboard[groupID]);
	NTLog(this, Info, "Setting ClipBoard HotKey, \" %s \" to activate keybinding\n", strModifiers.c_str());
}

void WebWindow::UnRegisterClipboardHotKey(int groupID)
{
	// TODO: have to use same parameter of RegisterClipboardHotKey.
}

void WebWindow::FolderOpen(AutoString strDownPath)
{
	// 탐색기 Open 하는 로직 필요
}
#include <gdk-pixbuf/gdk-pixdata.h>
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
				NTLog(this, Error, "Fail: gdk_pixbuf_new_from_file()");
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

}
#endif
