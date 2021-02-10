/*
 DDHotKey -- DDHotKeyUtilities.m
 
 Copyright (c) Dave DeLong <http://www.davedelong.com>
 
 Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.
 
 The software is  provided "as is", without warranty of any kind, including all implied warranties of merchantability and fitness. In no event shall the author(s) or copyright holder(s) be liable for any claim, damages, or other liability, whether in an action of contract, tort, or otherwise, arising from, out of, or in connection with the software or the use or other dealings in the software.
 */

#import "DDHotKeyUtilities.h"
#import <Carbon/Carbon.h>

static NSDictionary *_DDKeyCodeToCharacterMap(void);
static NSDictionary *_DDKeyCodeToCharacterMap(void) {
    static NSDictionary *keyCodeMap = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        keyCodeMap = @{
                       @(kVK_Return) : @"↩",
                       @(kVK_Tab) : @"⇥",
                       @(kVK_Space) : @"⎵",
                       @(kVK_Delete) : @"⌫",
                       @(kVK_Escape) : @"⎋",
                       @(kVK_Command) : @"⌘",
                       @(kVK_Shift) : @"⇧",
                       @(kVK_CapsLock) : @"⇪",
                       @(kVK_Option) : @"⌥",
                       @(kVK_Control) : @"⌃",
                       @(kVK_RightShift) : @"⇧",
                       @(kVK_RightOption) : @"⌥",
                       @(kVK_RightControl) : @"⌃",
                       @(kVK_VolumeUp) : @"🔊",
                       @(kVK_VolumeDown) : @"🔈",
                       @(kVK_Mute) : @"🔇",
                       @(kVK_Function) : @"\u2318",
                       @(kVK_F1) : @"F1",
                       @(kVK_F2) : @"F2",
                       @(kVK_F3) : @"F3",
                       @(kVK_F4) : @"F4",
                       @(kVK_F5) : @"F5",
                       @(kVK_F6) : @"F6",
                       @(kVK_F7) : @"F7",
                       @(kVK_F8) : @"F8",
                       @(kVK_F9) : @"F9",
                       @(kVK_F10) : @"F10",
                       @(kVK_F11) : @"F11",
                       @(kVK_F12) : @"F12",
                       @(kVK_F13) : @"F13",
                       @(kVK_F14) : @"F14",
                       @(kVK_F15) : @"F15",
                       @(kVK_F16) : @"F16",
                       @(kVK_F17) : @"F17",
                       @(kVK_F18) : @"F18",
                       @(kVK_F19) : @"F19",
                       @(kVK_F20) : @"F20",
                       //                       @(kVK_Help) : @"",
                       @(kVK_ForwardDelete) : @"⌦",
                       @(kVK_Home) : @"↖",
                       @(kVK_End) : @"↘",
                       @(kVK_PageUp) : @"⇞",
                       @(kVK_PageDown) : @"⇟",
                       @(kVK_LeftArrow) : @"←",
                       @(kVK_RightArrow) : @"→",
                       @(kVK_DownArrow) : @"↓",
                       @(kVK_UpArrow) : @"↑",
                       };
    });
    return keyCodeMap;
}

NSString *DDStringFromKeyCode(unsigned short keyCode, NSUInteger modifiers) {
    NSMutableString *final = [NSMutableString stringWithString:@""];
    NSDictionary *characterMap = _DDKeyCodeToCharacterMap();
    
    if (modifiers & NSEventModifierFlagControl) {
        [final appendString:[characterMap objectForKey:@(kVK_Control)]];
    }
    if (modifiers & NSEventModifierFlagOption) {
        [final appendString:[characterMap objectForKey:@(kVK_Option)]];
    }
    if (modifiers & NSEventModifierFlagShift) {
        [final appendString:[characterMap objectForKey:@(kVK_Shift)]];
    }
    if (modifiers & NSEventModifierFlagCommand) {
        [final appendString:[characterMap objectForKey:@(kVK_Command)]];
    }
    
    if (keyCode == kVK_Control || keyCode == kVK_Option || keyCode == kVK_Shift || keyCode == kVK_Command) {
        return final;
    }
    
    NSString *mapped = [characterMap objectForKey:@(keyCode)];
    if (mapped != nil) {
        [final appendString:mapped];
    } else {
        
        TISInputSourceRef currentKeyboard = TISCopyCurrentKeyboardInputSource();
        CFDataRef uchr = (CFDataRef)TISGetInputSourceProperty(currentKeyboard, kTISPropertyUnicodeKeyLayoutData);
        
        // Fix crash using non-unicode layouts, such as Chinese or Japanese.
        if (!uchr) {
            CFRelease(currentKeyboard);
            currentKeyboard = TISCopyCurrentASCIICapableKeyboardLayoutInputSource();
            uchr = (CFDataRef)TISGetInputSourceProperty(currentKeyboard, kTISPropertyUnicodeKeyLayoutData);
        }
        
        const UCKeyboardLayout *keyboardLayout = (const UCKeyboardLayout*)CFDataGetBytePtr(uchr);
        
        if (keyboardLayout) {
            UInt32 deadKeyState = 0;
            UniCharCount maxStringLength = 255;
            UniCharCount actualStringLength = 0;
            UniChar unicodeString[maxStringLength];
            
            UInt32 keyModifiers = DDCarbonModifierFlagsFromCocoaModifiers(modifiers);
            
            OSStatus status = UCKeyTranslate(keyboardLayout,
                                             keyCode, kUCKeyActionDown, keyModifiers,
                                             LMGetKbdType(), 0,
                                             &deadKeyState,
                                             maxStringLength,
                                             &actualStringLength, unicodeString);
            
            if (actualStringLength > 0 && status == noErr) {
                NSString *characterString = [NSString stringWithCharacters:unicodeString length:(NSUInteger)actualStringLength];
                
                [final appendString:characterString];
            }
        }
    }
    
    return final;
}

UInt32 DDCarbonModifierFlagsFromCocoaModifiers(NSUInteger flags) {
    UInt32 newFlags = 0;
    if ((flags & NSEventModifierFlagControl) > 0) { newFlags |= controlKey; }
    if ((flags & NSEventModifierFlagCommand) > 0) { newFlags |= cmdKey; }
    if ((flags & NSEventModifierFlagShift) > 0) { newFlags |= shiftKey; }
    if ((flags & NSEventModifierFlagOption) > 0) { newFlags |= optionKey; }
    if ((flags & NSEventModifierFlagCapsLock) > 0) { newFlags |= alphaLock; }
    return newFlags;
}

NSUInteger GetKeyMask(bool bCommand, bool bOption, bool bControl, bool bShift)
{
    NSUInteger keyMask=0;
    
    if(bCommand)
        keyMask = NSEventModifierFlagCommand;
    
    if(bOption)
        keyMask |= NSEventModifierFlagOption;
    
    if(bControl)
        keyMask |= NSEventModifierFlagControl;
    
    if(bShift)
        keyMask |= NSEventModifierFlagShift;
    
    return keyMask;
}

unsigned short GetCharKeyCode(NSString* NSKey)
{
    unsigned short keyCode=0;
    
    if ([NSKey isEqualToString:@"0"])
        keyCode=kVK_ANSI_0;
    else if ([NSKey isEqualToString:@"1"])
        keyCode=kVK_ANSI_1;
    else if ([NSKey isEqualToString:@"2"])
        keyCode=kVK_ANSI_2;
    else if ([NSKey isEqualToString:@"3"])
        keyCode=kVK_ANSI_3;
    else if ([NSKey isEqualToString:@"4"])
        keyCode=kVK_ANSI_4;
    else if ([NSKey isEqualToString:@"5"])
        keyCode=kVK_ANSI_5;
    else if ([NSKey isEqualToString:@"6"])
        keyCode=kVK_ANSI_6;
    else if ([NSKey isEqualToString:@"7"])
        keyCode=kVK_ANSI_7;
    else if ([NSKey isEqualToString:@"8"])
        keyCode=kVK_ANSI_8;
    else if ([NSKey isEqualToString:@"9"])
        keyCode=kVK_ANSI_9;
    
    else if ( [NSKey isEqualToString:@"A"] || [NSKey isEqualToString:@"a"] )
        keyCode=kVK_ANSI_A;
    else if ( [NSKey isEqualToString:@"B"] || [NSKey isEqualToString:@"b"] )
        keyCode=kVK_ANSI_B;
    else if ( [NSKey isEqualToString:@"C"] || [NSKey isEqualToString:@"c"] )
        keyCode=kVK_ANSI_C;
    else if ( [NSKey isEqualToString:@"D"] || [NSKey isEqualToString:@"d"] )
        keyCode=kVK_ANSI_D;
    else if ( [NSKey isEqualToString:@"E"] || [NSKey isEqualToString:@"e"] )
        keyCode=kVK_ANSI_E;
    else if ( [NSKey isEqualToString:@"F"] || [NSKey isEqualToString:@"f"] )
        keyCode=kVK_ANSI_F;
    else if ( [NSKey isEqualToString:@"G"] || [NSKey isEqualToString:@"g"] )
        keyCode=kVK_ANSI_G;
    else if ( [NSKey isEqualToString:@"H"] || [NSKey isEqualToString:@"h"] )
        keyCode=kVK_ANSI_H;
    else if ( [NSKey isEqualToString:@"I"] || [NSKey isEqualToString:@"i"] )
        keyCode=kVK_ANSI_I;
    else if ( [NSKey isEqualToString:@"J"] || [NSKey isEqualToString:@"j"] )
        keyCode=kVK_ANSI_J;
    else if ( [NSKey isEqualToString:@"K"] || [NSKey isEqualToString:@"k"] )
        keyCode=kVK_ANSI_K;
    else if ( [NSKey isEqualToString:@"L"] || [NSKey isEqualToString:@"l"] )
        keyCode=kVK_ANSI_L;
    else if ( [NSKey isEqualToString:@"M"] || [NSKey isEqualToString:@"m"] )
        keyCode=kVK_ANSI_M;
    else if ( [NSKey isEqualToString:@"N"] || [NSKey isEqualToString:@"n"] )
        keyCode=kVK_ANSI_N;
    else if ( [NSKey isEqualToString:@"O"] || [NSKey isEqualToString:@"o"] )
        keyCode=kVK_ANSI_O;
    else if ( [NSKey isEqualToString:@"P"] || [NSKey isEqualToString:@"p"] )
        keyCode=kVK_ANSI_P;
    else if ( [NSKey isEqualToString:@"Q"] || [NSKey isEqualToString:@"q"] )
        keyCode=kVK_ANSI_Q;
    else if ( [NSKey isEqualToString:@"R"] || [NSKey isEqualToString:@"r"] )
        keyCode=kVK_ANSI_R;
    else if ( [NSKey isEqualToString:@"S"] || [NSKey isEqualToString:@"s"] )
        keyCode=kVK_ANSI_S;
    else if ( [NSKey isEqualToString:@"T"] || [NSKey isEqualToString:@"t"] )
        keyCode=kVK_ANSI_T;
    else if ( [NSKey isEqualToString:@"U"] || [NSKey isEqualToString:@"u"] )
        keyCode=kVK_ANSI_U;
    else if ( [NSKey isEqualToString:@"V"] || [NSKey isEqualToString:@"v"] )
        keyCode=kVK_ANSI_V;
    else if ( [NSKey isEqualToString:@"W"] || [NSKey isEqualToString:@"w"] )
        keyCode=kVK_ANSI_W;
    else if ( [NSKey isEqualToString:@"X"] || [NSKey isEqualToString:@"X"] )
        keyCode=kVK_ANSI_X;
    else if ( [NSKey isEqualToString:@"Y"] || [NSKey isEqualToString:@"y"] )
        keyCode=kVK_ANSI_Y;
    else if ( [NSKey isEqualToString:@"Z"] || [NSKey isEqualToString:@"z"] )
        keyCode=kVK_ANSI_Z;
    
    else if ( [NSKey isEqualToString:@"F1"] || [NSKey isEqualToString:@"f1"] )
        keyCode= kVK_F1;
    else if ( [NSKey isEqualToString:@"F2"] || [NSKey isEqualToString:@"f2"] )
        keyCode= kVK_F2;
    else if ( [NSKey isEqualToString:@"F3"] || [NSKey isEqualToString:@"f3"] )
        keyCode= kVK_F3;
    else if ( [NSKey isEqualToString:@"F4"] || [NSKey isEqualToString:@"f4"] )
        keyCode= kVK_F4;
    else if ( [NSKey isEqualToString:@"F5"] || [NSKey isEqualToString:@"f5"] )
        keyCode= kVK_F5;
    else if ( [NSKey isEqualToString:@"F6"] || [NSKey isEqualToString:@"f6"] )
        keyCode= kVK_F6;
    else if ( [NSKey isEqualToString:@"F7"] || [NSKey isEqualToString:@"f7"] )
        keyCode= kVK_F7;
    else if ( [NSKey isEqualToString:@"F8"] || [NSKey isEqualToString:@"f8"] )
        keyCode= kVK_F8;
    else if ( [NSKey isEqualToString:@"F9"] || [NSKey isEqualToString:@"f9"] )
        keyCode= kVK_F9;
    else if ( [NSKey isEqualToString:@"F10"] || [NSKey isEqualToString:@"f10"] )
        keyCode= kVK_F10;
    else if ( [NSKey isEqualToString:@"F11"] || [NSKey isEqualToString:@"f11"] )
        keyCode= kVK_F11;
    else if ( [NSKey isEqualToString:@"F12"] || [NSKey isEqualToString:@"f12"] )
        keyCode= kVK_F12;
    
    return keyCode;
}

NSString* GetCharKeyString(unsigned short keyCode)
{
    NSString* keyString = nil;
    
    switch(keyCode)
    {
        case kVK_ANSI_0:
            keyString = [[NSString alloc]initWithFormat:@"0"];
            break;
        case kVK_ANSI_1:
            keyString = [[NSString alloc]initWithFormat:@"1"];
            break;
        case kVK_ANSI_2:
            keyString = [[NSString alloc]initWithFormat:@"2"];
            break;
        case kVK_ANSI_3:
            keyString = [[NSString alloc]initWithFormat:@"3"];
            break;
        case kVK_ANSI_4:
            keyString = [[NSString alloc]initWithFormat:@"4"];
            break;
        case kVK_ANSI_5:
            keyString = [[NSString alloc]initWithFormat:@"5"];
            break;
        case kVK_ANSI_6:
            keyString = [[NSString alloc]initWithFormat:@"6"];
            break;
        case kVK_ANSI_7:
            keyString = [[NSString alloc]initWithFormat:@"7"];
            break;
        case kVK_ANSI_8:
            keyString = [[NSString alloc]initWithFormat:@"8"];
            break;
        case kVK_ANSI_9:
            keyString = [[NSString alloc]initWithFormat:@"9"];
            break;
            
        case kVK_ANSI_A:
            keyString = [[NSString alloc]initWithFormat:@"A"];
            break;
        case kVK_ANSI_B:
            keyString = [[NSString alloc]initWithFormat:@"B"];
            break;
        case kVK_ANSI_C:
            keyString = [[NSString alloc]initWithFormat:@"C"];
            break;
        case kVK_ANSI_D:
            keyString = [[NSString alloc]initWithFormat:@"D"];
            break;
        case kVK_ANSI_E:
            keyString = [[NSString alloc]initWithFormat:@"E"];
            break;
        case kVK_ANSI_F:
            keyString = [[NSString alloc]initWithFormat:@"F"];
            break;
        case kVK_ANSI_G:
            keyString = [[NSString alloc]initWithFormat:@"G"];
            break;
        case kVK_ANSI_H:
            keyString = [[NSString alloc]initWithFormat:@"H"];
            break;
        case kVK_ANSI_I:
            keyString = [[NSString alloc]initWithFormat:@"I"];
            break;
        case kVK_ANSI_J:
            keyString = [[NSString alloc]initWithFormat:@"J"];
            break;
        case kVK_ANSI_K:
            keyString = [[NSString alloc]initWithFormat:@"K"];
            break;
        case kVK_ANSI_L:
            keyString = [[NSString alloc]initWithFormat:@"L"];
            break;
        case kVK_ANSI_M:
            keyString = [[NSString alloc]initWithFormat:@"M"];
            break;
        case kVK_ANSI_N:
            keyString = [[NSString alloc]initWithFormat:@"N"];
            break;
        case kVK_ANSI_O:
            keyString = [[NSString alloc]initWithFormat:@"O"];
            break;
        case kVK_ANSI_P:
            keyString = [[NSString alloc]initWithFormat:@"P"];
            break;
        case kVK_ANSI_Q:
            keyString = [[NSString alloc]initWithFormat:@"Q"];
            break;
        case kVK_ANSI_R:
            keyString = [[NSString alloc]initWithFormat:@"R"];
            break;
        case kVK_ANSI_S:
            keyString = [[NSString alloc]initWithFormat:@"S"];
            break;
        case kVK_ANSI_T:
            keyString = [[NSString alloc]initWithFormat:@"T"];
            break;
        case kVK_ANSI_U:
            keyString = [[NSString alloc]initWithFormat:@"U"];
            break;
        case kVK_ANSI_V:
            keyString = [[NSString alloc]initWithFormat:@"V"];
            break;
        case kVK_ANSI_W:
            keyString = [[NSString alloc]initWithFormat:@"W"];
            break;
        case kVK_ANSI_X:
            keyString = [[NSString alloc]initWithFormat:@"X"];
            break;
        case kVK_ANSI_Y:
            keyString = [[NSString alloc]initWithFormat:@"Y"];
            break;
        case kVK_ANSI_Z:
            keyString = [[NSString alloc]initWithFormat:@"Z"];
            break;
            
        case kVK_F1:
            keyString = [[NSString alloc]initWithFormat:@"F1"];
            break;
        case kVK_F2:
            keyString = [[NSString alloc]initWithFormat:@"F2"];
            break;
        case kVK_F3:
            keyString = [[NSString alloc]initWithFormat:@"F3"];
            break;
        case kVK_F4:
            keyString = [[NSString alloc]initWithFormat:@"F4"];
            break;
        case kVK_F5:
            keyString = [[NSString alloc]initWithFormat:@"F5"];
            break;
        case kVK_F6:
            keyString = [[NSString alloc]initWithFormat:@"F6"];
            break;
        case kVK_F7:
            keyString = [[NSString alloc]initWithFormat:@"F7"];
            break;
        case kVK_F8:
            keyString = [[NSString alloc]initWithFormat:@"F8"];
            break;
        case kVK_F9:
            keyString = [[NSString alloc]initWithFormat:@"F9"];
            break;
        case kVK_F10:
            keyString = [[NSString alloc]initWithFormat:@"F10"];
            break;
        case kVK_F11:
            keyString = [[NSString alloc]initWithFormat:@"F11"];
            break;
        case kVK_F12:
            keyString = [[NSString alloc]initWithFormat:@"F12"];
            break;
    }
    
    return keyString;
}