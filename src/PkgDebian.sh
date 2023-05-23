#!/bin/bash

PATH_PACKAGE=$5 #artifacts/installer/debian/packages
PKG_NAME=opennetlink
mkdir -p $PATH_PACKAGE

fpm -f -s dir -t deb -n $PKG_NAME -v $1 -C artifacts/debian/published -p $PATH_PACKAGE --prefix=/opt/hanssak/opennetlink --before-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh --after-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh --after-remove OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionRemove.sh --license "Apache-2.0 License" --url "http://www.hanssak.co.kr" --vendor "Hanssak System Co.Ltd." --maintainer "angellos@hanssak.co.kr" --description "Based on Cross-Platform (.Net Core + Blazor), Open NetLink (Network Connection Agent) Software. This Application is for SecureGate Network Connection System." --deb-no-default-config-files --verbose --log info

cd $PATH_PACKAGE

if [[ $2 == "TRUE" ]]; then 
    #for patch
    mv $PKG_NAME"_$1_amd64.deb" "OpenNetLink-Debian-$1.deb"
else
    #for setup
    mv $PKG_NAME"_$1_amd64.deb" "[$4] OpenNetLink_"$3"_Debian_"$1".deb"
fi

