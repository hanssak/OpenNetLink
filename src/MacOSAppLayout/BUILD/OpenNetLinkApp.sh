#!/usr/bin/osascript

tell application "Terminal"
    activate
    try
	do script "cd /Applications/OpenNetLinkApp.app/Contents/MacOS && nohup ./OpenNetLinkApp &" 
    end try
    delay 0.5

    tell application "System Events"
	key down command
	keystroke tab
	key up command
    end tell

    tell application "System Events"
	keystroke "exit"
	keystroke return
	keystroke "exit"
	keystroke return
    end tell
    delay 3
    quit
end tell

#delay 0.5 
#tell application "System Events" to tell UI element "OpenNetLinkApp" of list 1 of process "Dock"
#    if not (exists) then return
#    perform action "AXShowMenu"
#    set language to user locale of (system info)
#
#    if (language = "ko-Kore_KR") then
#	click menu item "옵션" of menu 1
#    else
#	click menu item "Options" of menu 1
#    end if
#
#    if (language = "ko-Kore_KR") then
#	click menu item "Dock에서 제거" of menu 1 of menu item "옵션" of menu 1
#    else
#	click menu item "Remove from Dock" of menu 1 of menu item "Options" of menu 1
#    end if
#
#end tell
