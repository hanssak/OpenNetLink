#include "FinderSync.h"
#include "ShellCommand.h"

#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>

#define XCODE_APP 1

static const char* kApplePluginkitBinary = "/usr/bin/pluginkit";
static const char* kFinderSyncBundleIdentifier = "com.hanssak.OpenNetLinkApp.SGFinderSync";

static inline std::string pluginPath()
{
    NSBundle *nsBundle = [NSBundle mainBundle];
    NSString *strBundle = [nsBundle bundlePath];
#ifdef XCODE_APP
    return std::string(strBundle.UTF8String) + "/Contents/PlugIns/SGFinderSync.appex";
#else
    return std::string(strBundle.UTF8String) + "/fsplugin/SGFinderSync.appex";
#endif
}

/// \brief list all installed plugins
static bool installedPluginPath(std::string &path)
{
    int status;
    std::string arguments = std::string(" -m -v -i ") + kFinderSyncBundleIdentifier;
    path = ::execCommand(std::string(kApplePluginkitBinary) + arguments, status);
    if (status != 0 || path.empty())
        return false;
    int begin = path.find_first_of('/');
    if (begin == std::string::npos)
        return false;
    int end = path.find_first_of('\n', begin);
    if (end == std::string::npos || end - begin < 0)
        return false;
    path = path.substr(begin, end - begin);

    NSLog(@"[FinderSync] found installed plugin %s", path.data());

    return true;
}

bool FinderSyncExtensionHelper::isInstalled() {
    int status;
    std::string output;
    std::string arguments = std::string(" -m -i ") + kFinderSyncBundleIdentifier;
    output = ::execCommand(std::string(kApplePluginkitBinary) + arguments, status);
    if (status != 0 || output.empty())
        return false;

    return true;
}

bool FinderSyncExtensionHelper::isEnabled() {
    int status;
    std::string output;
    std::string arguments = std::string(" -m -i ") + kFinderSyncBundleIdentifier;
    output = ::execCommand(std::string(kApplePluginkitBinary) + arguments, status);
    if (status != 0 || output.empty())
        return false;
    if (output[0] != '+' && output[0] != '?')
        return false;

    return true;
}

bool FinderSyncExtensionHelper::isBundled() {
    std::string plugin_path = pluginPath();
    struct stat buffer;
    if (stat (plugin_path.c_str(), &buffer) != 0) {
        NSLog(@"[FinderSync] unable to find bundled plugin at %s", plugin_path.data());
        return false;
    }

    NSLog(@"[FinderSync] found bundled plugin at %s", plugin_path.data());
    return true;
}

// Developer notes:
//  In Mac OSX Sierra and higher, to install a finder plugin (a .appex folder) ,
//  two conditions must be satisfied:
//    - the plugin must be signed
//    - the plugin must be included as part of a .app
//  So when seadrive-gui is not compiled with xcode, there is no way to install
//  the plugin.
#ifdef XCODE_APP
bool FinderSyncExtensionHelper::reinstall(bool force) {
    if (!isBundled()) return false;

    int status;
    std::string bundled_plugin_path = pluginPath();
    std::string plugin_path;
    std::string remove_arguments;

    // remove all installed plugins
    while(installedPluginPath(plugin_path)) {
        if (!force && bundled_plugin_path.compare(plugin_path) == 0) {
            NSLog(@"[FinderSync] current plugin detected: %s", bundled_plugin_path.data());
            return true;
        }
        remove_arguments = std::string(" -r ") + plugin_path;
        // this command returns non-zero when succeeds,
        // so don't bother to check it
        ::execCommand(std::string(kApplePluginkitBinary) + remove_arguments, status);
    }

    std::string output;
    std::string install_arguments = std::string(" -a ") + bundled_plugin_path;
    output = ::execCommand(std::string(kApplePluginkitBinary) + install_arguments, status);
    if (status != 0) return false;

    NSLog(@"[FinderSync] reinstalled");
    return true;
}
#else
bool FinderSyncExtensionHelper::reinstall(bool force) {
    return false;
}
#endif

bool FinderSyncExtensionHelper::setEnable(bool enable) {
    int status;
    const char *election = enable ? "use" : "ignore";
    std::string arguments = std::string(" -e ") + election + std::string(" -i ") + kFinderSyncBundleIdentifier;

    ::execCommand(std::string(kApplePluginkitBinary) + arguments, status);
    if (status != 0)
        return false;

    return true;
}

// End Of File