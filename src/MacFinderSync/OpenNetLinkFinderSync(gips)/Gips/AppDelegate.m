//
//  AppDelegate.m
//  Gips
//
//  Created by Moaaz Sidat on 2015-08-18.
//  Copyright (c) 2015 MS. All rights reserved.
//

#import "AppDelegate.h"

@interface AppDelegate ()

@end

@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification {
    // Insert code here to initialize your application
    [[NSUserNotificationCenter defaultUserNotificationCenter] setDelegate:self];
    
    NSString* observedObject = @"com.hanssak.OpenNetLinkApp.SGFinderSync";
    NSDistributedNotificationCenter* center = [NSDistributedNotificationCenter defaultCenter];
    [center addObserver:self selector:@selector(observingFinderSync:)
                name:@"SecureGate FinderSync Notify" object:observedObject];
    NSLog(@"-000---menuForMenuKind SecureGate FinderSync Notify call");
}

- (void)observingFinderSync:(NSNotification *)aNotification {
    NSLog(@"receive FinderSync: %@", aNotification);
    return ;
}

- (void)applicationWillTerminate:(NSNotification *)aNotification {
    // Insert code here to tear down your application
}

- (BOOL)application:(NSApplication *)sender openFile:(NSString *)filename {
    NSLog(@"Filename via Open file: %@", filename);
    [[NSNotificationCenter defaultCenter] postNotificationName:@"openFileWithApp" object:filename];
    return YES;
}

- (BOOL)userNotificationCenter:(NSUserNotificationCenter *)center shouldPresentNotification:(NSUserNotification *)notification {
    return YES;
}

- (void)userNotificationCenter:(NSUserNotificationCenter *)center didActivateNotification:(NSUserNotification *)notification {
    NSString* newImageLocation = notification.userInfo[@"newImageLocation"];
    NSURL *newImageURL = [NSURL fileURLWithPath:newImageLocation];
//    NSLog(@"new image location: %@ and URL: %@", newImageLocation, [newImageURL path]);
    NSArray *imageURLs = [NSArray arrayWithObjects:newImageURL, nil];
    [[NSWorkspace sharedWorkspace] activateFileViewerSelectingURLs:imageURLs];
//    NSRunAlertPanel(@"Hello, Gips", [newImageURL path], @"Ok", nil, nil);
}

@end
