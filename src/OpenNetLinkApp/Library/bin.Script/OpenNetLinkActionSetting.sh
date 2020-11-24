#!/bin/bash

PATH_HANSSAK=/opt/hanssak
PATH_OPENNETLINK=$PATH_HANSSAK/opennetlink
PATH_BIN_SCRIPT=$PATH_OPENNETLINK/bin.Script
PATH_APPLICATIONS=/usr/share/applications/
PATH_NEMO=/usr/share/nemo/actions/

## Directory check
if [ ! -d $PATH_OPENNETLINK ] ; then
    echo "NOT FOUND PATH ${PATH_OPENNETLINK}"
    exit 0
fi;

# Tray icon (xdg-icon-resource) add
xdg-icon-resource install --novendor --size 16 ${PATH_OPENNETLINK}/wwwroot/images/adminlte/ico.png hanssak.webwindow.open.netlink

OS=`lsb_release -i |awk '{print $3}'`

# 소문자 변환 후 검사 ( HamoniKR, Gooroom, Tmaxos )
if [ tmaxos = "${OS,,}" ] ; then
	# start desktop setting + right click action setting
	cp -rf ${PATH_BIN_SCRIPT}/tmaxos/SecureGate.desktop ${PATH_APPLICATIONS}
else 
	# start desktop stting
	cp -rf ${PATH_BIN_SCRIPT}/SecureGate.desktop ${PATH_APPLICATIONS}

	# nemo (right click) action setting
	cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nemo_action ${PATH_NEMO}
fi

# directory authority setting
USERNAME=`users | xargs -n1 | uniq`
GROUPNAME=`id -gn ${USERNAME}`
chown -R ${USERNAME}:${GROUPNAME} $PATH_HANSSAK

# OpenNetLink shell authority setting
chmod 777 ${PATH_OPENNETLINK}/OpenNetLinkApp.sh


