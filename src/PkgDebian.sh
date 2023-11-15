#!/bin/bash

VERSION=$1
ISPATCH=$2 #isPatch
NETWORK_FLAG=$3
CUSTOM_NAME=$4
PATH_PACKAGE=$5 #artifacts/installer/debian/packages
ISUPDATECHECK=$6 #isUpdateCheck
STARTAUTO=$7
STORAGE_NAME=$8

PKG_NAME=opennetlink
mkdir -p $PATH_PACKAGE


BEFOREINSTALL="OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh"
AFTERINSTALL="OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh"

if [ $ISPATCH == "FALSE" ]; then
    ISUPDATECHECK="FALSE"
fi

echo "ISPATCH : $ISPATCH / ISUPDATECHECK : $ISUPDATECHECK"

if [ $STARTAUTO == "FALSE" ]; then 
    sed -i 's/START_AUTO=true/START_AUTO=false/g' $AFTERINSTALL
else
    sed -i 's/START_AUTO=false/START_AUTO=true/g' $AFTERINSTALL
fi

if [ $ISUPDATECHECK == "FALSE" ]; then 
    sed -i 's/UPDATE_CHECK=true/UPDATE_CHECK=false/g' $AFTERINSTALL
    sed -i 's/UPDATE_CHECK=true/UPDATE_CHECK=false/g' $BEFOREINSTALL
else
    sed -i 's/UPDATE_CHECK=false/UPDATE_CHECK=true/g' $AFTERINSTALL
    sed -i 's/UPDATE_CHECK=false/UPDATE_CHECK=true/g' $BEFOREINSTALL
fi


fpm -f -s dir -t deb -n $PKG_NAME -v $VERSION -C artifacts/debian/published -p $PATH_PACKAGE --prefix=/opt/hanssak/opennetlink --before-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh --after-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh --after-remove OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionRemove.sh --license "Apache-2.0 License" --url "http://www.hanssak.co.kr" --vendor "Hanssak System Co.Ltd." --maintainer "angellos@hanssak.co.kr" --description "Based on Cross-Platform (.Net Core + Blazor), Open NetLink (Network Connection Agent) Software. This Application is for SecureGate Network Connection System." --deb-no-default-config-files --verbose --log info

cd $PATH_PACKAGE

if [[ $ISPATCH == "TRUE" ]]; then 
    #for patch
    mv $PKG_NAME"_"$VERSION"_amd64.deb" "OpenNetLink-Debian-"$VERSION".deb"
else
    #for setup
    mv $PKG_NAME"_"$VERSION"_amd64.deb" "[$CUSTOM_NAME] OpenNetLink_"$STORAGE_NAME"_"$NETWORK_FLAG"_Debian_"$VERSION".deb"
fi

