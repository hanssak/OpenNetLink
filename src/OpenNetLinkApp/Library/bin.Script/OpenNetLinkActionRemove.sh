#!/bin/bash

PATH_NEMO_CONTEXTMENU=/usr/share/nemo/actions/ContextMenu.nemo_action
PATH_APPLICATIONS_SECUREGATE=/usr/share/applications/SecureGate.desktop
PATH_OPENNETLINK=/opt/hanssak/opennetlink

USERNAME=`users | xargs -n1 | uniq`
PATH_NAUTILUS_CONTEXTMENU=/home/${USERNAME}/.local/share/nautilus/scripts/

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

# Remove: nautilus action (OpenNetLinkActionSettting에서 설정했던 우클릭 메뉴 삭제)
rm -rf $PATH_NAUTILUS_CONTEXTMENU/'Sending OpenNetLink'

# Update Fail
#CheckToRemoveFileAndDirectory $PATH_OPENNETLINK

