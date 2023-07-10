#!/bin/zsh

# pkgAndNotarize.sh

# 2019 - Armin Briegel - Scripting OS X

# place a copy of this script in in the project folder
# when run it will build for installation,
# create a pkg from the product,
# upload the pkg for notarization and monitor the notarization status

# before you can run this script:
# - set release signing of the tool to 'Developer ID Application'
# - enable the hardened run-time
# - change the 'Installation Build Products Location' to `$SRCROOT/build/pkgroot`
# 
# you want to add the `build` subdirectory to gitignore


# put your dev account information into these variables

# the email address of your developer account
dev_account="angellos124@gmail.com"

# the name of your Developer ID installer certificate
# signature="Developer ID Installer: First Last (ABCD123456)"
signature="Developer ID Installer: Hanssak System Co., Ltd. (L7W5N48H4G)"
# appledevsig="Apple Development: 명훈 민 (NY3NHU3Q3D)"
appledevsig="Developer ID Application: Hanssak System Co., Ltd. (L7W5N48H4G)"

# the 10-digit team id
# dev_team="ABCD123456"
dev_team="L7W5N48H4G"

# the label of the keychain item which contains an app-specific password
# dev_keychain_label="Developer-altool"
# dev_keychain_label="eumg-vmam-nluz-ygto"
dev_keychain_label="sxog-tiki-hjrx-pxfs"


# put your project's information into these variables
if [ $# -ne 5 ]; then
	echo "Usage: $0 {version} $1 {ispatch} $2 {networkflag} $3 {customName} $4 {outputPath} $5"
	exit -1
fi;
version=$1
identifier="com.hanssak.OpenNetLinkApp"
productname="OpenNetLinkApp"
ispatch=$2
networkflag=$3
customname=$4
outputpath=$5

# code starts here
projectdir=$(dirname $0)
builddir="$projectdir/BUILD"
pkgroot="$builddir/PKGROOT"


OUTPUT_PATH="$projectdir/../$outputpath"
LAYOUT_PATH="$builddir/OpenNetLinkApp.app.tgz"
SCRIPT_PATH="$builddir/SCRIPTS"
PLUGIN_PATH="$builddir/PlugIns"
ENT_PATH="$builddir/entitlements.plist"
APP_PATH="$pkgroot/$productname.app"
ZIP_PATH="$pkgroot/$productname.zip"
#BIN_PATH="$projectdir/../OpenNetLinkApp/bin/Debug/net5.0/osx-x64/publish"
BIN_PATH="$projectdir/../artifacts/mac/published"
SHEXE_PATH="$builddir/OpenNetLinkApp.sh"
SHAPP_PATH="$APP_PATH/Contents/MacOS/OpenNetLinkApp.sh"

# functions
codesignapp() { # $1: path to file to codesign, $2: certification
    apppath=${1:?"need a apppath"}
    certificate=${2:?"need an Apple Development: FirstName LastName (XXXXXXXXXX)"}
    entitlements=${3:?"need an entitlements.plist file full path)"}

    # codesign --options=runtime -vvv --force --deep --sign "$certificate" "$apppath"
    codesign -vvv --force --deep --entitlements $entitlements --options=runtime --sign "$certificate" "$apppath"
    echo "Result codesign app: $apppath"
    codesign -dv --verbose=4 "$apppath"
}

requeststatus() { # $1: requestUUID
                              #--password "@keychain:$dev_keychain_label" 2>&1 \
    requestUUID=${1?:"need a request UUID"}
    req_status=$(xcrun altool --notarization-info "$requestUUID" \
                              --username "$dev_account" \
                              --password "$dev_keychain_label" 2>&1 \
                 | awk -F ': ' '/Status:/ { print $2; }' )
    echo "$req_status"
}

notarizefile() { # $1: path to file to notarize, $2: identifier
    filepath=${1:?"need a filepath"}
    identifier=${2:?"need an identifier"}
    
    # upload file
    echo "## uploading $filepath for notarization"
                               #--password "@keychain:$dev_keychain_label" \
    # requestUUID=$(xcrun altool --notarize-app \
    #                           --primary-bundle-id "$identifier" \
    #                           --username "$dev_account" \
    #                           --password "$dev_keychain_label" \
    #                           --asc-provider "$dev_team" \
    #                           --file "$filepath" 2>&1 \
    #              | awk '/RequestUUID/ { print $NF; }')

    requestUUID1=$(xcrun altool --notarize-app \
                               --primary-bundle-id "$identifier" \
                               --username "$dev_account" \
                               --password "$dev_keychain_label" \
                               --asc-provider "$dev_team" \
                               --file "$filepath" 2>&1)

    echo "------------------ vervoses (debug) ----------------"
    echo $requestUUID1

    requestUUID=$(echo $requestUUID1 | awk '/RequestUUID/ { print $NF; }')    
                               
    echo "Notarization RequestUUID: $requestUUID"
    
    if [[ $requestUUID == "" ]]; then 
        echo "could not upload for notarization"
        exit 1
    fi
        
    # wait for status to be not "in progress" any more
    request_status="in progress"
    while [[ "$request_status" == "in progress" ]]; do
        echo -n "waiting... "
        sleep 10
        request_status=$(requeststatus "$requestUUID")
        echo "$request_status"
    done
    
    # print status information
                 # --password "@keychain:$dev_keychain_label"
    xcrun altool --notarization-info "$requestUUID" \
                 --username "$dev_account" \
                 --password "$dev_keychain_label"
    echo 
    
    if [[ $request_status != "success" ]]; then
        echo "## could not notarize $filepath"
        exit 1
    fi
    
}


# build clean install

# echo "## building with Xcode"
# xcodebuild clean install -quiet

# check if pkgroot exists where we expect it
if [[ ! -d $pkgroot ]]; then
    echo "##############################################################################################"
    echo "couldn't find pkgroot $pkgroot"
    echo "##############################################################################################"
    exit 1
fi

if [[ -d $APP_PATH ]]; then
    echo "##############################################################################################"
    echo "## remove previous app data: $APP_PATH"
    rm -rf "$APP_PATH"
    if [[ ! -d $APP_PATH ]]; then
        echo "-> Success: remove previous app data: $APP_PATH"
    fi
fi

echo "##############################################################################################"
## Place the layout in the folder where the APP will be created.
echo "## Place the layout in the folder where the APP will be created: $APP_PATH"
tar xf "$LAYOUT_PATH" -C "$pkgroot/"
if [[ -d $APP_PATH ]]; then
    echo "-> Success: Place the layout in the folder where the APP will be created: $APP_PATH"
fi

echo "##############################################################################################"
## copy files of app package in app layout.
echo "## copy files of app package in app layout: $BIN_PATH/* -> $APP_PATH/Contents/MacOS/"
cp -R $BIN_PATH/* "$APP_PATH/Contents/MacOS/"

echo "##############################################################################################"
## check files of app package after copied.
echo "## check files of app package after copied: $APP_PATH/Contents/MacOS/*"
ls "$APP_PATH/Contents/MacOS/"

echo "##############################################################################################"
## remove garbage file and directory.
echo "## remove garbage file and directory"
rm -rf "$APP_PATH/Contents/MacOS/bin.Script"

echo "##############################################################################################"
## move wwwroot to Resources directory.
echo "## move wwwroot to Resources directory: $APP_PATH/Contents/MacOS/wwwroot -> $APP_PATH/Contents/Resources/"
mv "$APP_PATH/Contents/MacOS/wwwroot" "$APP_PATH/Contents/Resources/"

echo "##############################################################################################"
## check wwwroot directory after moved.
echo "## check wwwroot directory after moved: $APP_PATH/Contents/Resources/wwwroot"
ls -altrd "$APP_PATH/Contents/Resources/wwwroot"

echo "##############################################################################################"
## create symbolic link from wwwroot to MacOS directory.
echo "## create symbolic link from wwwroot to MacOS directory: $APP_PATH/Contents/Resources/wwwroot -> $APP_PATH/Contents/MacOS/"
cd "$APP_PATH/Contents/MacOS"
ln -s "../Resources/wwwroot" "./"
cd "../../../../../"
pwd

echo "##############################################################################################"
## check wwwroot directory after created symbolic link.
echo "## check wwwroot directory after created symbolic link: $APP_PATH/Contents/MacOS/wwwroot"
ls -altr "$APP_PATH/Contents/MacOS/wwwroot"

echo "##############################################################################################"
## copy shell execute to MacOS directory.
echo "## copy shell execute to MacOS directory: $SHEXE_PATH -> $APP_PATH/Contents/MacOS/"
cp "$SHEXE_PATH" "$APP_PATH/Contents/MacOS/"

echo "##############################################################################################"
## change shell execute mode with 755(rwxr-xr-x).
echo "## change shell execute mode with 755(rwxr-xr-x): $SHAPP_PATH"
chmod 755 "$SHAPP_PATH"
ls -altr "$SHAPP_PATH"

echo "##############################################################################################"
## copy the PlugIn that code signed and notarized to app
echo "## copy the PlugIn that code signed and notarized to app: SGFinderSync Extension In App"
cp -R "$PLUGIN_PATH" "$APP_PATH/Contents/"
if [[ -d "$APP_PATH/Contents/PlugIns" ]]; then
    echo "-> Success: copy the PlugIn that code signed and notarized to app: $APP_PATH/Contents/PlugIns"
    rm -rf "$APP_PATH/Contents/PlugIns/SGFinderSync.appex"
    ls -altr "$APP_PATH/Contents/PlugIns"
fi

echo "##############################################################################################"
## make codesign to app
echo "## make codesign app: $APP_PATH"
codesignapp "$APP_PATH" "$appledevsig" "$ENT_PATH"

## make the zip
# Create a ZIP archive suitable for altool.

# echo "## building zip: $ZIP_PATH"
# /usr/bin/ditto -c -k --keepParent "$APP_PATH" "$ZIP_PATH"

# As a convenience, open the export folder in Finder.
# /usr/bin/open "$pkgroot"

echo "##############################################################################################"
## build the pkg

if [[ $ispatch == "TRUE" ]]; then 
    #for patch
    pkgpath="$OUTPUT_PATH/OpenNetLink-Mac-"$version".pkg"
else
    #for setup
    pkgpath="$OUTPUT_PATH/["$customname"] OpenNetLink_"$networkflag"_Mac_"$version".pkg"
fi

echo "## building pkg: $pkgpath"
pkgbuild --root "$pkgroot" \
         --version "$version" \
         --install-location "/Applications" \
         --scripts "$SCRIPT_PATH" \
         --identifier "$identifier" \
         --sign "$signature" \
         "$pkgpath"

echo "##############################################################################################"
# upload for notarization
notarizefile "$pkgpath" "$identifier"
# notarizefile "$ZIP_PATH" "$identifier"

echo "##############################################################################################"
# staple result
echo "## Stapling $pkgpath"
xcrun stapler staple "$pkgpath"

if [[ -d $APP_PATH ]]; then
    echo "##############################################################################################"
    echo "## remove used app data to create app package: $APP_PATH"
    rm -rf "$APP_PATH"
    if [[ ! -d $APP_PATH ]]; then
        echo "-> Success: remove used app data to create app package: $APP_PATH"
    fi
fi
echo "##############################################################################################"
echo '## Done!'
echo "##############################################################################################"

# show the pkg in Finder
open -R "$pkgpath"

exit 0
