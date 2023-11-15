#!/bin/bash
VERSION=$1
ISPATCH=$2 #isPatch
NETWORK_FLAG=$3
CUSTOM_NAME=$4
PATH_PACKAGE=$5 #artifacts/installer/redhat/packages
ISUPDATECHECK=$6 #isUpdateCheck
STARTAUTO=$7
STORAGE_NAME=$8

PKG_NAME=opennetlink
mkdir -p $PATH_PACKAGE


#redhat 기반은 자동으로 release 번호가 매겨지므로, iteration 옵션으로 0으로  고정
fpm -f -s dir -t rpm -n $PKG_NAME -v $VERSION -C artifacts/redhat/published -p $PATH_PACKAGE --prefix=/opt/hanssak/opennetlink --before-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh --after-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh --after-remove OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionRemove.sh --license "Apache-2.0 License" --url "http://www.hanssak.co.kr" --vendor "Hanssak System Co.Ltd." --maintainer "angellos@hanssak.co.kr" --description "Based on Cross-Platform (.Net Core + Blazor), Open NetLink (Network Connection Agent) Software. This Application is for SecureGate Network Connection System." --verbose --log info --iteration 0


cd $PATH_PACKAGE

if [[ $ISPATCH == "TRUE" ]]; then 
    #for patch
    mv $PKG_NAME"-$VERSION-0.x86_64.rpm" "OpenNetLink-redhat-$VERSION.rpm"
else
    #for setup
    mv $PKG_NAME"-$VERSION-0.x86_64.rpm" "#$CUSTOM_NAME# OpenNetLink_"$STORAGE_NAME"_"$NETWORK_FLAG"_redhat_"$VERSION".rpm"
fi

