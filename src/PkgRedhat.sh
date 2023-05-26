#!/bin/bash

PATH_PACKAGE=$5 #artifacts/installer/redhat/packages
PKG_NAME=opennetlink
mkdir -p $PATH_PACKAGE


#redhat 기반은 자동으로 release 번호가 매겨지므로, iteration 옵션으로 0으로  고정
fpm -f -s dir -t rpm -n $PKG_NAME -v $1 -C artifacts/redhat/published -p $PATH_PACKAGE --prefix=/opt/hanssak/opennetlink --before-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh --after-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh --after-remove OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionRemove.sh --license "Apache-2.0 License" --url "http://www.hanssak.co.kr" --vendor "Hanssak System Co.Ltd." --maintainer "angellos@hanssak.co.kr" --description "Based on Cross-Platform (.Net Core + Blazor), Open NetLink (Network Connection Agent) Software. This Application is for SecureGate Network Connection System." --verbose --log info --iteration 0


cd $PATH_PACKAGE

if [[ $2 == "TRUE" ]]; then 
    #for patch
    mv $PKG_NAME"-$1-0.x86_64.rpm" "OpenNetLink-redhat-$1.rpm"
else
    #for setup
    mv $PKG_NAME"-$1-0.x86_64.rpm" "#$4# OpenNetLink_"$3"_redhat_"$1".rpm"
fi

