#!/bin/bash

PATH_OPEN_NETLINK=/opt/hanssak/OpenNetLink
PATH_BIN_SCRIPT=${PATH_OPEN_NETLINK}/bin.Script


# nemo action setting
cp -rf ${PATH_BIN_SCRIPT}/SecureGate.desktop /usr/share/applications/

# start desktop stting
cp -rf ${PATH_BIN_SCRIPT}/ContextMenu.nemo_action /usr/share/nemo/actions/
