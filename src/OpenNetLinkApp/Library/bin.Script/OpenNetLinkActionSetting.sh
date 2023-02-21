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
    cp -rf ${PATH_BIN_SCRIPT}/org.gnome.SecureGate.desktop ${PATH_APPLICATIONS}

    #파일탐색기가 나무가 아닌 경우엔, 노틸러스 쪽 우클릭 동작   
    if [ -d ${PATH_NEMO} ] ; then
        # nemo (right click) action setting
        cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nemo_action ${PATH_NEMO}
    else
        # nautilus (right click) action setting
        cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nautilus_action ${PATH_NAUTILUS}/'Sending OpenNetLink'
        chmod 777 ${PATH_NAUTILUS}/'Sending OpenNetLink'
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

#로그인 정보 및 노티 정보가 있는 DB 파일도 백업하여 유지
BACKUP_SGNOTIFY_FILE=/tmp/SGNotifyDB.db
if [ -e $BACKUP_SGNOTIFY_FILE ] ; then
    cp ${BACKUP_SGNOTIFY_FILE} ${PATH_OPENNETLINK}/wwwroot/db/
    rm -rf ${BACKUP_SGNOTIFY_FILE}
fi

BACKUP_SGSETTING_FILE=/tmp/SGSettingsDB.db
 if [ -e $BACKUP_SGSETTING_FILE ] ; then
    cp ${BACKUP_SGSETTING_FILE} ${PATH_OPENNETLINK}/wwwroot/db/
    rm -rf ${BACKUP_SGSETTING_FILE}
fi

#####<마임체크에 필요한 libbz2에 대한 so파일 확인>########
#libgtk를 이용하여 해당 OS에 라이브러리 폴더 경로 추출한다. (필수 요소인 GTK 사용)
LIBGTK_FILE=`ldconfig -p | grep libgtk |awk 'NR==1 {print $4}'`
if [ -z $LIBGTK_FILE ] ; then
    LIBRARY_PATH=/lib64/    
else    
    LIBRARY_PATH=`dirname $LIBGTK_FILE`
fi;
#마임체크를 위해 존재해야하는 so 파일
LIBBZ2_1_FILE="$LIBRARY_PATH/libbz2.so.1"
LIBBZ2_2_FILE="$LIBRARY_PATH/libbz2.so.1.0"

if [ ! -f $LIBBZ2_1_FILE ] ; then   #libbz2.so.1 파일이 존재하지 않으면, 라이브러리에 복사    
    cp ${PATH_OPENNETLINK}/Library/libbz2.so.1 $LIBBZ2_1_FILE
fi;

if [ ! -f $LIBBZ2_2_FILE ] ; then   #libbz2.so.1.0 파일이 존재하진 않으면, 라이브러리에 복사   
    cp ${PATH_OPENNETLINK}/Library/libbz2.so.1 $LIBBZ2_2_FILE
fi;
