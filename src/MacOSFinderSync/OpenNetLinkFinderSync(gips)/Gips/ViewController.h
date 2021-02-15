//
//  ViewController.h
//  Gips
//
//  Created by Moaaz Sidat on 2015-08-18.
//  Copyright (c) 2015 MS. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import <ImageIO/ImageIO.h>


@interface ViewController : NSViewController <NSApplicationDelegate>
//@property (weak) IBOutlet NSPathControl *imagePath;
@property (weak) IBOutlet NSButton *exportButton;
@property (weak) IBOutlet NSTextField *imageHeight;
@property (weak) IBOutlet NSTextField *imageWidth;
@property (weak) IBOutlet NSButton *maintainRatio;
@property (weak) IBOutlet NSTextField *chosenFile;
@property (weak) IBOutlet NSImageView *chosenImage;

/* Actions */
- (IBAction)openImage:(id)sender;
- (IBAction)heightChanged:(id)sender;
- (IBAction)widthChanged:(id)sender;
- (IBAction)gipsImage:(id)sender;

- (BOOL)application:(NSApplication *)sender openFile:(NSString *)filename;

/* NSTask */
@property (nonatomic, strong) __block NSTask *gipsTask;
@property (nonatomic) BOOL isRunning;
@property (nonatomic) BOOL isImage;

/* Image file */
@property (weak) NSURL* imageURL;
@property (nonatomic) int width;
@property (nonatomic) int height;

- (BOOL) openFileWithApp:(NSNotification *)notification;

@end

