//
//  FinderSync.m
//  SGFinderSync
//
//  Created by Angellos-mac-mini on 08/02/2021.
//  Copyright © 2019 Hanssak.co.LTD. All rights reserved.
//

#import "FinderSync.h"
#import <sys/sysctl.h>
#import <sys/socket.h>
#import <sys/un.h>
#import <sys/types.h>

@interface FinderSync ()

@property NSURL *myFolderURL;

@property NSString * strRListName;

@end

@implementation FinderSync

- (instancetype)init {
    self = [super init];

    NSLog(@"%s launched from %@ ; compiled at %s", __PRETTY_FUNCTION__, [[NSBundle mainBundle] bundlePath], __TIME__);

    // Set up the directory we are syncing.
    // NSUserDefaults *sharedDefaults = [[NSUserDefaults alloc] initWithSuiteName:@"com.hanssak.OpenNetLinkApp"];
    self.myFolderURL = [NSURL fileURLWithPath:@"/"];
    [FIFinderSyncController defaultController].directoryURLs = [NSSet setWithObject:self.myFolderURL];
   
    self.strRListName=NSHomeDirectory();
    self.strRListName=[self.strRListName stringByAppendingString:@"/RList.txt"];
    
#if 0
    // Set up images for our badge identifiers. For demonstration purposes, this uses off-the-shelf images.
    [[FIFinderSyncController defaultController] setBadgeImage:[NSImage imageNamed: NSImageNameColorPanel] label:@"Status One" forBadgeIdentifier:@"One"];
    [[FIFinderSyncController defaultController] setBadgeImage:[NSImage imageNamed: NSImageNameCaution] label:@"Status Two" forBadgeIdentifier:@"Two"];
#endif
    return self;
}

#pragma mark - Primary Finder Sync protocol methods

- (void)beginObservingDirectoryAtURL:(NSURL *)url
{
    // The user is now seeing the container's contents.
    // If they see it in more than one view at a time, we're only told once.
    NSLog(@"beginObservingDirectoryAtURL:%@", url.filePathURL);
}


- (void)endObservingDirectoryAtURL:(NSURL *)url
{
    // The user is no longer seeing the container's contents.
    NSLog(@"endObservingDirectoryAtURL:%@", url.filePathURL);
}

- (void)requestBadgeIdentifierForURL:(NSURL *)url
{
    NSLog(@"requestBadgeIdentifierForURL:%@", url.filePathURL);
    
    // For demonstration purposes, this picks one of our two badges, or no badge at all, based on the filename.
#if 0
    NSInteger whichBadge = [url.filePathURL hash] % 3;
    NSString* badgeIdentifier = @[@"", @"One", @"Two"][whichBadge];
    [[FIFinderSyncController defaultController] setBadgeIdentifier:badgeIdentifier forURL:url];
#endif
}

#pragma mark - Menu and toolbar item support

- (NSString *)toolbarItemName
{
    return @"SGFinderSync";
}

- (NSString *)toolbarItemToolTip
{
    return @"SGFinderSync: Click the file item, add to transfer lists.";
}

- (NSImage *)toolbarItemImage
{
    return [NSImage imageNamed:NSImageNameCaution];
}

- (NSMenu *)menuForMenuKind:(FIMenuKind)whichMenu
{
    // Produce a menu for the extension.
    NSMenu *menu = [[NSMenu alloc] initWithTitle:@""];
    [menu addItemWithTitle:@"파일전송시스템 파일 추가" action:@selector(ActionAddFileListSG:) keyEquivalent:@""];
    NSLog(@"menuForMenuKind: Add Contextual Menu Item for SecureGate FinderSync Notify call");

    return menu;
}

- (NSString *) MakeAddFileSG
{
    FIFinderSyncController *syncController = [FIFinderSyncController defaultController];
    NSArray<NSURL *> * nsArray = syncController.selectedItemURLs;
    NSString * nsFileList = @"";
    NSUInteger count = nsArray.count;
    for(int n = 0; n < count; n++)
    {
        NSURL * nsurl=(NSURL *)nsArray[n];
        nsFileList=[nsFileList stringByAppendingString:nsurl.path];
        if( (n+1) < count ) nsFileList=[nsFileList stringByAppendingString:@"\r\n"];
    }
    
    return nsFileList;
}

- (IBAction)ActionAddFileListSG:(id)sender
{
    int nGroupId = 0;
    NSString *CmdFileList = [@(nGroupId) stringValue];
    CmdFileList = [CmdFileList stringByAppendingString:@"\r\n"];
    CmdFileList = [CmdFileList stringByAppendingString:[self MakeAddFileSG]];
    
    NSString * strNSExecName= @"OpenNetLinkApp";
    [self  ToSaveForAddFileListSG:CmdFileList];
    if([self FindProcess:strNSExecName]==false)
    {
       NSString * strProcPath=@"/Applications/OpenNetLinkApp.app";
       char chPath[4098];
       memset(chPath, 0x00, sizeof(chPath));
       sprintf(chPath, "open %s", (char *)strProcPath.UTF8String);
       
       system(chPath);
    }
    
    NSLog(@"ActionAddFileListSG Called to Save for SecureGate FinderSync Notify call");
}

- (void) ToSaveForAddFileListSG:(NSString *)data
{
    char *chFileName=(char *)[self.strRListName UTF8String];

    FILE * file=fopen(chFileName, "w");
    if(file!=NULL)
    {
        char *chData =(char *)data.UTF8String;
        fwrite(chData, 1, strlen(chData), file);
        fwrite("\n", 1, strlen("\n"), file);
        fclose(file);
    }

    return ;
}

- (bool) FindProcess:(NSString *)strProcessName
{
    int maxArgumentSize = 4096;
    int mib[3] = { CTL_KERN, KERN_PROC, KERN_PROC_ALL};
    struct kinfo_proc *info;
    size_t length;
    //  int count;
    
    if (sysctl(mib, 3, NULL, &length, NULL, 0) < 0)
        return false;
    
    if (!(info = malloc(length)))
        return false;
    
    if (sysctl(mib, 3, info, &length, NULL, 0) < 0)
    {
        free(info);
        return false;
    }
    
    bool bFindProcess=false;
    int count = (int)(length / sizeof(struct kinfo_proc));
    for (int i = 0; i < count; i++)
    {
        pid_t pid = info[i].kp_proc.p_pid;
        if (pid == 0)
            continue;
        
        size_t size = maxArgumentSize;
        char* buffer = (char *)malloc(length);
        if (sysctl((int[]){ CTL_KERN, KERN_PROCARGS2, pid }, 3, buffer, &size, NULL, 0) == 0)
        {
            NSString* strExecName = [NSString stringWithCString:(buffer+sizeof(int)) encoding:NSUTF8StringEncoding];
            NSArray * nsArray = [strExecName componentsSeparatedByString: @"/"];
            int count = (int)nsArray.count;
            strExecName=nsArray[count-1];
            if([strProcessName compare:strExecName]==kCFCompareEqualTo)
            {
                bFindProcess=true;
                free(buffer);
                break;
            }
        }
        
        free(buffer);
    }
    
    free(info);
    
    return bFindProcess;
}

@end
