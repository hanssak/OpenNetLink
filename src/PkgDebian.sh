#!/bin/bash

PATH_PACKAGE=artifacts/installer/debian/packages

mkdir -p $PATH_PACKAGE

fpm -f -s dir -t deb -n opennetlink -v $1 -C artifacts/debian/published -p $PATH_PACKAGE --prefix=/opt/hanssak/opennetlink --after-install OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionSetting.sh --after-remove OpenNetLinkApp/Library/bin.Script/OpenNetLinkActionRemove.sh --license "Apache-2.0 License" --url "http://www.hanssak.co.kr" --vendor "Hanssak System Co.Ltd." --maintainer "angellos@hanssak.co.kr" --description "Based on Cross-Platform (.Net Core + Blazor), Open NetLink (Network Connection Agent) Software. This Application is for SecureGate Network Connection System." --deb-no-default-config-files --verbose --log info
