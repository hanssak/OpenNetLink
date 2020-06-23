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

std::mutex invokeLockMutex;
std::vector<FileInfoDND> vecDNDList;

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

void on_size_allocate(GtkWidget* widget, GdkRectangle* allocation, gpointer self);
gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, gpointer self);

GtkTargetEntry ui_drag_targets[UI_DRAG_TARGETS_COUNT] = {
    {"text/plain", 0, DT_TEXT},
    {"text/uri", 0, DT_URI},
    {"text/uri-list", 0, DT_URI_LIST}
};

WebWindow::WebWindow(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback) : _webview(nullptr)
{
	_webMessageReceivedCallback = webMessageReceivedCallback;

	// It makes xlib thread safe.
	// Needed for get_position.
	XInitThreads();

	gtk_init(0, NULL);
	_app = gtk_application_new("webwindow.hanssak.open.netlink", G_APPLICATION_FLAGS_NONE);
	g_application_register(G_APPLICATION(_app), NULL, NULL);
	_window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
	gtk_window_set_default_size(GTK_WINDOW(_window), 1280, 800);
	SetTitle(title);

	if (parent == NULL)
	{
		g_signal_connect(G_OBJECT(_window), "destroy",
			G_CALLBACK(+[](GtkWidget* w, gpointer arg) { gtk_main_quit(); }),
			this);
		g_signal_connect(G_OBJECT(_window), "size-allocate",
			G_CALLBACK(on_size_allocate),
			this);
		g_signal_connect(G_OBJECT(_window), "configure-event",
			G_CALLBACK(on_configure_event),
			this);
	}
}

WebWindow::~WebWindow()
{
	gtk_widget_destroy(_window);
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

		/* Drag and Drop Start */
		/* For Motion Capture : maybe need it */
		g_signal_connect(G_OBJECT (_webview), "motion-notify-event",G_CALLBACK (+[](GtkWidget *widget,
															GdkEvent *event, gpointer user_data) -> gboolean {
			GtkWidget *window = (GtkWidget *)user_data;
			if (event->type==GDK_MOTION_NOTIFY) {
				GdkEventMotion* e=(GdkEventMotion*)event;
				//g_print("Coordinates: (%u,%u)\n", (guint)e->x,(guint)e->y);
				//gtk_window_move (GTK_WINDOW(dialog), e->x-150, e->y-150);
				/*
				int x, y;
				gtk_window_get_position(GTK_WINDOW(window), &x, &y);
				gtk_window_move(GTK_WINDOW(widget), x+300, y-300);
				*/
			}
		}), _window);
		gtk_drag_dest_set(_webview, GTK_DEST_DEFAULT_MOTION, ui_drag_targets, UI_DRAG_TARGETS_COUNT, static_cast<GdkDragAction>(GDK_ACTION_COPY | GDK_ACTION_MOVE | GDK_ACTION_LINK | GDK_ACTION_PRIVATE));
	    g_signal_connect_swapped(_webview, "drag-motion", G_CALLBACK(+[](GtkWidget *widget, GdkDragContext *context, 
																gint x, gint y, guint time, gpointer user_data) -> gboolean {
  		    /* No more use, don't care */
			/*
			GdkAtom target;

			target = gtk_drag_dest_find_target (widget, context, NULL);
			if (target == GDK_NONE)
				gdk_drag_status (context, (GdkDragAction)0, time);
			else
			{
				gtk_drag_get_data (widget, context, target, time);
			}
			*/
			return FALSE;
		}), this);
		g_signal_connect_swapped(_webview, "drag-data-received", G_CALLBACK(+[](GtkWidget*, GdkDragContext* context, gint x, gint y, 
																		GtkSelectionData* data, guint info, guint time, 
																		gpointer userData) {
			guchar *text = NULL;
			gchar **files = NULL;

			/* Don't touch below line in referenced Motion, It is freezed!!. */
			//gtk_drag_finish(context, TRUE, FALSE, (guint)time);
			switch (info)
			{
				case DT_URI_LIST:
					files = gtk_selection_data_get_uris(data);
					break;

				case DT_TEXT:
					text = gtk_selection_data_get_text(data);
					g_strchomp((gchar *)text);
					files = g_strsplit((const gchar *)text, "\n", -1);
					break;

				default:
					g_print("Warning: not handled drag and drop target %u.", info);
					break;
			}

			if (text != NULL)
			{
				g_free(text);
			}
			if (files != NULL)
			{
				DragNDropData *wd;
				GThread *thread;

				wd = (DragNDropData *)g_malloc(sizeof *wd);
				wd->files = files;

				thread = g_thread_new("DragNDropWorker", DragNDropWorker, wd);
				g_thread_unref(thread);
			}
    	}), this);
		g_signal_connect_swapped(_webview, "drag-drop", G_CALLBACK(+[](GtkWidget*, GdkDragContext* context, gint x, gint y, 
															guint time, gpointer userData) -> gboolean {
			GList *targets;
			targets = gdk_drag_context_list_targets(context);
			if (targets == NULL)
			{
				return FALSE;
			}
			return FALSE;
		}), this);
		/* Drag and Drop End */

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

void WebWindow::SetTitle(AutoString title)
{
	gtk_window_set_title(GTK_WINDOW(_window), title);
}

void WebWindow::WaitForExit()
{
	gtk_main();
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

void WebWindow::GetDragDropList(GetDragDropListCallback callback)
{
    if (callback)
    {
		for(const auto& dndFileInfo : vecDNDList) if (!callback(&dndFileInfo)) break;
    }
}

gpointer WebWindow::DragNDropWorker(gpointer data)
{
    DragNDropData *workerData = (DragNDropData *)data;

	vecDNDList.clear();
    for (int i = 0; workerData->files[i] != NULL; i++)
    {
        gchar *file = workerData->files[i];
        if (g_str_has_prefix(file, "file://"))
        {
            chopN(file, 7);
        }

		std::string strDecodedFile = UrlDecoded(file);
        g_print("Received2: '%s'\n", strDecodedFile.c_str());
		FileInfoDND retDND = GetFileInfoDND(strDecodedFile);
		if(retDND.strFullName.size() > 0) vecDNDList.push_back(retDND);
    }

    g_strfreev(workerData->files);
    g_free(workerData);
    return NULL;
}

size_t WebWindow::chopN(char *str, size_t n)
{
    g_assert(n != 0 && str != 0);
    size_t len = strlen(str);
    if (n > len)
        n = len;
    memmove(str, str + n, len - n + 1);
    return (len - n);
}

std::string WebWindow::UrlEncode(std::string strUri)
{
    std::string strEncoded;
    char *encoded_uri = NULL;
    //const char *escape_char_str = "!*'();:@&=+$,/?#[]";

    encoded_uri = g_uri_escape_string(strUri.c_str(), nullptr, TRUE);
    strEncoded = encoded_uri;
    
    free(encoded_uri);
    return strEncoded;
}   

std::string WebWindow::UrlDecoded(std::string strUri)
{
    std::string strDecoded;
    char *decoded_uri = NULL;
    //const char *escape_char_str = "!*'();:@&=+$,/?#[]";
    decoded_uri = g_uri_unescape_string (strUri.c_str(), nullptr);
    strDecoded = decoded_uri;
    
    free(decoded_uri);
    return strDecoded;
}

FileInfoDND WebWindow::GetFileInfoDND(std::string strFile)
{
   struct stat fileInfo;
   struct FileInfoDND retFileInfo;

   if (stat(strFile.c_str(), &fileInfo) != 0) {  // Use stat() to get the info
      retFileInfo.strFullName.clear();
      return retFileInfo;
   }

   retFileInfo.strFullName = strFile;
   g_print("GetFileInfoDND: File: '%s'\n", retFileInfo.strFullName.c_str());
   if ((fileInfo.st_mode & S_IFMT) == S_IFDIR) { // From sys/types.h
      retFileInfo.st_mode = 0;
   } else {
      retFileInfo.st_mode = 1;
   }
   	g_print("GetFileInfoDND: Type: '%d'\n", retFileInfo.st_mode);

   retFileInfo.st_size = fileInfo.st_size;
   	g_print("GetFileInfoDND: size: '%ld'\n", retFileInfo.st_size);
   retFileInfo.tCreate = fileInfo.st_ctime;
   	g_print("GetFileInfoDND: ctime: '%ld'\n", retFileInfo.tCreate);
   retFileInfo.tLast = fileInfo.st_mtime;
   	g_print("GetFileInfoDND: mtime: '%ld'\n", retFileInfo.tLast);

   return retFileInfo;
}


#endif
