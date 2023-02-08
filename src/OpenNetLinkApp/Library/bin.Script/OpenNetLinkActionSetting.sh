#!/bin/bash

PATH_HANSSAK=/opt/hanssak
PATH_OPENNETLINK=$PATH_HANSSAK/opennetlink
PATH_BIN_SCRIPT=$PATH_OPENNETLINK/bin.Script
PATH_APPLICATIONS=/usr/share/applications/
PATH_NEMO=/usr/share/nemo/actions/

# directory authority setting
USERNAME=`users | xargs -n1 | uniq`
GROUPNAME=`id -gn ${USERNAME}`

#노틸러스 파일탐색기 우클릭 메뉴 추가
PATH_NAUTILUS=/home/${USERNAME}/.local/share/nautilus/scripts/

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

    #파일탐색기가 나무가 아닌 경우엔, 노틸러스 쪽 우클릭 동작   
    if [ -d ${PATH_NEMO} ] ; then
        # nemo (right click) action setting
        cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nemo_action ${PATH_NEMO}
    else
        # nautilus (right click) action setting
        cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nautilus_action ${PATH_NAUTILUS}/'Sending OpenNetLink'
        chmod 777 ${PATH_NAUTILUS}/ContextMenu.nautilus_action
    fi
fi

chown -R ${USERNAME}:${GROUPNAME} $PATH_HANSSAK

# OpenNetLink shell authority setting
chmod 777 ${PATH_OPENNETLINK}/OpenNetLinkApp.sh

#Move Network.json, AppEnvSetting.json not to change thoes information
NETWORK_FILE="$PATH_HANSSAK/opennetlink/wwwroot/conf/NetWork.json"
BACKUP_NETWORK_FILE=/tmp/Network.json
if [ -e $BACKUP_NETWORK_FILE ] ; then
    cp /tmp/NetWork.json ${PATH_OPENNETLINK}/wwwroot/conf/
    rm -rf /tmp/NetWork.json
fi

APPENV_FILE="$PATH_HANSSAK/opennetlink/wwwroot/conf/AppEnvSetting.json"
BACKUP_APPENV_FILE=/tmp/AppEnvSetting.json
if [ -e $BACKUP_APPENV_FILE ] ; then
    cp /tmp/AppEnvSetting.json ${PATH_OPENNETLINK}/wwwroot/conf/
    rm -rf /tmp/AppEnvSetting.json
fi



 



