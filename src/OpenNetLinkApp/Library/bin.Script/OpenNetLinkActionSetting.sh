#!/bin/bash

PATH_OPENNETLINK=/opt/hanssak/opennetlink
PATH_BIN_SCRIPT=$PATH_OPENNETLINK/bin.Script
PATH_APPLICATIONS=/usr/share/applications/
PATH_NEMO=/usr/share/nemo/actions/

# Directory check
if [ ! -d $PATH_OPENNETLINK ] ; then
    echo "NOT FOUND PATH: ${PATH_OPENNETLINK}"
    exit 0
fi;

# Tray icon (xdg-icon-resource) add
xdg-icon-resource install --novendor --size 16 ${PATH_OPENNETLINK}/wwwroot/images/adminlte/ico.png hanssak.webwindow.open.netlink

# nemo action setting
cp -rf ${PATH_BIN_SCRIPT}/SecureGate.desktop ${PATH_APPLICATIONS}

# start desktop stting
cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nemo_action ${PATH_NEMO}

# 
chown -R ${USER}:${USER} /opt/hanssak