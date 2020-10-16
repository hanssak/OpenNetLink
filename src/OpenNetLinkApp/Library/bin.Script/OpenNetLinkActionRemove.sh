#!/bin/bash

PATH_NEMO_CONTEXTMENU=/usr/share/nemo/actions/ContextMenu.nemo_action
PATH_APPLICATIONS_SECUREGATE=/usr/share/applications/SecureGate.desktop
PATH_OPENNETLINK=/opt/hanssak/opennetlink

function CheckToRemoveFileAndDirectory()
{
	if [ $# -ne 1 ]; then
		echo "Need by 1 parameter"
	elif [ -f $1 ]; then
		echo "Remove File : $1"
		rm -rf $1
	elif [ -d $1 ]; then
		echo "Remove Directory : $1"
		rm -rf $1
	fi;
}

# Remove: start desktop
CheckToRemoveFileAndDirectory $PATH_APPLICATIONS_SECUREGATE

# Remove: nemo action
CheckToRemoveFileAndDirectory $PATH_NEMO_CONTEXTMENU

# Update Fail
#CheckToRemoveFileAndDirectory $PATH_OPENNETLINK

