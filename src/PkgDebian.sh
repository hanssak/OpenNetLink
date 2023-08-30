#!/bin/bash

PATH_PACKAGE=$5 #artifacts/installer/debian/packages
PKG_NAME=opennetlink
mkdir -p $PATH_PACKAGE

ISPATCH=$2 #isPatch
STARTAUTO=$6
ISUPDATECHECK=$8 #isUpdateCheck

BEFOREINSTALL="OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh"
AFTERINSTALL="OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh"

if [ $ISPATCH == "FALSE" ]; then
    ISUPDATECHECK="FALSE"
fi

echo "ISPATCH : $ISPATCH / ISUPDATECHECK : $ISUPDATECHECK"

if [ $STARTAUTO == "FALSE" ]; then 
    sed -i '' -e 's/START_AUTO=true/START_AUTO=false/g' $AFTERINSTALL
else
    sed -i '' -e 's/START_AUTO=false/START_AUTO=true/g' $AFTERINSTALL
fi

if [ $ISUPDATECHECK == "FALSE" ]; then 
    sed -i '' -e 's/UPDATE_CHECK=true/UPDATE_CHECK=false/g' $AFTERINSTALL
    sed -i '' -e 's/UPDATE_CHECK=true/UPDATE_CHECK=false/g' $BEFOREINSTALL
else
    sed -i '' -e 's/UPDATE_CHECK=false/UPDATE_CHECK=true/g' $AFTERINSTALL
    sed -i '' -e 's/UPDATE_CHECK=false/UPDATE_CHECK=true/g' $BEFOREINSTALL
fi


fpm -f -s dir -t deb -n $PKG_NAME -v $1 -C artifacts/debian/published -p $PATH_PACKAGE --prefix=/opt/hanssak/opennetlink --before-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionBeforeInstall.sh --after-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh --after-remove OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionRemove.sh --license "Apache-2.0 License" --url "http://www.hanssak.co.kr" --vendor "Hanssak System Co.Ltd." --maintainer "angellos@hanssak.co.kr" --description "Based on Cross-Platform (.Net Core + Blazor), Open NetLink (Network Connection Agent) Software. This Application is for SecureGate Network Connection System." --deb-no-default-config-files --verbose --log info

cd $PATH_PACKAGE

if [[ $ISPATCH == "TRUE" ]]; then 
    #for patch
    mv $PKG_NAME"_$1_amd64.deb" "OpenNetLink-Debian-$1.deb"
else
    #for setup
    mv $PKG_NAME"_$1_amd64.deb" "[$4] OpenNetLink_"$7"_"$3"_Debian_"$1".deb"
fi

