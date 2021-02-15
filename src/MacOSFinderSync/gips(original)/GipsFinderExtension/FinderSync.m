//
//  FinderSync.m
//  GipsFinderExtension
//
//  Created by Moaaz Sidat on 2015-12-31.
//  Copyright © 2015 MS. All rights reserved.
//

#import "FinderSync.h"

@interface FinderSync ()

@property NSURL *myFolderURL;

@end

@implementation FinderSync

- (instancetype)init {
    self = [super init];

    NSLog(@"%s launched from %@ ; compiled at %s", __PRETTY_FUNCTION__, [[NSBundle mainBundle] bundlePath], __TIME__);

    // Set up the directory we are syncing.
//    self.myFolderURL = [NSURL fileURLWithPath:@"/Users/Shared/MySyncExtension Documents"];
//    [FIFinderSyncController defaultController].directoryURLs = [NSSet setWithObject:self.myFolderURL];

    // Set up images for our badge identifiers. For demonstration purposes, this uses off-the-shelf images.
//    [[FIFinderSyncController defaultController] setBadgeImage:[NSImage imageNamed: NSImageNameColorPanel] label:@"Status One" forBadgeIdentifier:@"One"];
//    [[FIFinderSyncController defaultController] setBadgeImage:[NSImage imageNamed: NSImageNameCaution] label:@"Status Two" forBadgeIdentifier:@"Two"];
    
    //    self.myFolderURL = [sharedDefaults URLForKey:MyFolderKey];
    
    // Set up the folder we are syncing
    NSUserDefaults *sharedDefaults = [[NSUserDefaults alloc] initWithSuiteName:@"com.moaaz.Gips"];
    
    if (self.myFolderURL == nil) {
        self.myFolderURL = [NSURL fileURLWithPath:[@"~/Pictures/GipsFinderExtension" stringByExpandingTildeInPath]];
    }
    
    [FIFinderSyncController defaultController].directoryURLs = [NSSet setWithObject:self.myFolderURL];
    
//    NSLog(@"Self myfolderURL: %@", self.myFolderURL);
//    
//    NSURL *myURL = [NSURL fileURLWithPath:[@"~/Pictures" stringByExpandingTildeInPath]];
//    NSLog(@"fileURLWithPath: %@", myURL);
//    
//    NSURL *picturesURL = [NSURL fileURLWithPath:[@"~/Pictures" stringByExpandingTildeInPath]];
//    NSURL *picturesURL2 = [NSURL fileURLWithPath:[@"~/Pictures/Room" stringByExpandingTildeInPath]];
//    NSURL *documentsURL = [NSURL fileURLWithPath:[@"~/Pictures/GipsFinderExtension" stringByExpandingTildeInPath]];
//    NSURL *musicURL = [NSURL fileURLWithPath:[@"~/Music/iTunes" stringByExpandingTildeInPath]];
//
//    
//    [FIFinderSyncController defaultController].directoryURLs = [NSSet setWithObjects:picturesURL, picturesURL2, documentsURL, musicURL, nil];
    
    return self;
}

#pragma mark - Primary Finder Sync protocol methods

- (void)beginObservingDirectoryAtURL:(NSURL *)url {
    // The user is now seeing the container's contents.
    // If they see it in more than one view at a time, we're only told once.
    NSLog(@"beginObservingDirectoryAtURL:%@", url.filePathURL);
}


- (void)endObservingDirectoryAtURL:(NSURL *)url {
    // The user is no longer seeing the container's contents.
    NSLog(@"endObservingDirectoryAtURL:%@", url.filePathURL);
}

- (void)requestBadgeIdentifierForURL:(NSURL *)url {
    NSLog(@"requestBadgeIdentifierForURL:%@", url.filePathURL);
    
    // For demonstration purposes, this picks one of our two badges, or no badge at all, based on the filename.
    NSInteger whichBadge = [url.filePathURL hash] % 3;
    NSString* badgeIdentifier = @[@"", @"One", @"Two"][whichBadge];
    [[FIFinderSyncController defaultController] setBadgeIdentifier:badgeIdentifier forURL:url];
}

#pragma mark - Menu and toolbar item support

- (NSString *)toolbarItemName {
    return @"GipsFinderExtension";
}

- (NSString *)toolbarItemToolTip {
    return @"GipsFinderExtension: Click the toolbar item for a menu.";
}

- (NSImage *)toolbarItemImage {
    return [NSImage imageNamed:NSImageNameCaution];
}

- (NSMenu *)menuForMenuKind:(FIMenuKind)whichMenu {
    NSArray *selectedItemURLs = [[FIFinderSyncController defaultController] selectedItemURLs];
    
    NSMutableArray *selectedItemPaths = [[NSMutableArray alloc] initWithCapacity:[selectedItemURLs count]];
    
    for (NSURL* selectedItemURL in selectedItemURLs) {
        [selectedItemPaths addObject:[selectedItemURL path]];
    }
    
    NSLog(@"Selected item path: %@", selectedItemPaths);
    
    // Produce a menu for the extension.
    NSMenu *menu = [[NSMenu alloc] initWithTitle:@""];
    
    if ([selectedItemPaths count] == 1) {
        // Check here if the selectedItemPaths[0] is a file or directory
        // NSLog(@"The selected item url is %@", selectedItemPaths[0]);
//        [menu addItemWithTitle:@"Resize image" action:@selector(sampleAction:) keyEquivalent:@""];
        [menu addItemWithTitle:@"Resize image with Gips" action:@selector(sendToGips:) keyEquivalent:@""];
    }

    return menu;
}

- (IBAction)sendToGips:(id)sender {
//    NSURL* target = [[FIFinderSyncController defaultController] targetedURL];
    NSArray* items = [[FIFinderSyncController defaultController] selectedItemURLs];
    NSString *imagePath = [items[0] path];
    // Open the file with Gips here and one file at a time
    [[NSWorkspace sharedWorkspace] openFile:imagePath withApplication:@"Gips"];
}

- (IBAction)sampleAction:(id)sender {
    NSURL* target = [[FIFinderSyncController defaultController] targetedURL];
    NSArray* items = [[FIFinderSyncController defaultController] selectedItemURLs];

    NSLog(@"sampleAction: menu item: %@, target = %@, items = ", [sender title], [target filePathURL]);
    [items enumerateObjectsUsingBlock: ^(id obj, NSUInteger idx, BOOL *stop) {
        NSLog(@"    %@", [obj filePathURL]);
    }];
}

@end

