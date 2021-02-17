#include <stdio.h>
#include <string>
#include <cstdio>

#import <Cocoa/Cocoa.h>

class FinderSyncExtensionHelper {
    public:
        FinderSyncExtensionHelper() {};

    public:
        bool isInstalled();
        bool isEnabled(); 
        bool isBundled();
        bool reinstall(bool force);
        bool setEnable(bool enable);
};