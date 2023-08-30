#!/bin/bash

PATH_HANSSAK=/opt/hanssak
PATH_OPENNETLINK=$PATH_HANSSAK/opennetlink
PATH_BIN_SCRIPT=$PATH_OPENNETLINK/bin.Script
PATH_APPLICATIONS=/usr/share/applications/
PATH_NEMO=/usr/share/nemo/actions/

UPDATE_CHECK=false

if [ $UPDATE_CHECK == true ] ; then
	 if [ -d "$PATH_OPENNETLINK" ]; then
        cp -r $PATH_OPENNETLINK /tmp
        rm -rf $PATH_OPENNETLINK
    fi
else
	#Move Network.json, AppEnvSetting.json not to change thoes information
	NETWORK_FILE="$PATH_OPENNETLINK/wwwroot/conf/NetWork.json"
	if [ -e $NETWORK_FILE ] ; then
		cp ${NETWORK_FILE} /tmp
	fi

	APPENV_FILE="$PATH_OPENNETLINK/wwwroot/conf/AppEnvSetting.json"
	if [ -e $APPENV_FILE ] ; then
		cp ${APPENV_FILE} /tmp
	fi

	SGNOTIFY_FILE="$PATH_OPENNETLINK/wwwroot/db/SGNotifyDB.db"
	if [ -e $SGNOTIFY_FILE ] ; then
		cp ${SGNOTIFY_FILE} /tmp
	fi

	SGSETTING_FILE="$PATH_OPENNETLINK/wwwroot/db/SGSettingsDB.db"
	if [ -e $SGSETTING_FILE ] ; then
		cp ${SGSETTING_FILE} /tmp
	fi

	HSCK_FILE="$PATH_OPENNETLINK/wwwroot/conf/hsck"
	if [ -e $HSCK_FILE ] ; then
		cp ${HSCK_FILE} /tmp
	fi
fi


