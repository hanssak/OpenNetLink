#/bin/bash

DATE=`date +%y%m%d.%H.%M` 

SLFF=selfcontained
TOPF=hanssak
APPF=OpenNetLinkApp


\rm -rf ${TOPF}
\rm -rf ${SLFF}

dotnet clean ./OpenNetLinkApp/OpenNetLinkApp.csproj  -c Release

dotnet publish ./OpenNetLinkApp/OpenNetLinkApp.csproj  -c Release -o  ${TOPF}/${APPF}  -f netcoreapp3.1 
dotnet publish ./ContextTransferClient/ContextTransferClient.csproj  -c Release -o  ${TOPF}/${APPF}  -f netcoreapp3.1 
dotnet publish ./PreviewUtil/PreviewUtil.csproj  -c Release -o  ${TOPF}/${APPF}  -f netcoreapp3.1 

dotnet clean ./OpenNetLinkApp/OpenNetLinkApp.csproj -c Release
dotnet publish ./OpenNetLinkApp/OpenNetLinkApp.csproj -c Release -o  ${SLFF}/${TOPF}/${APPF} -r linux-x64 -f netcoreapp3.1

cp bin.Script/OpenNetLinkApp.sh ${TOPF}/${APPF}
cp bin.Script/OpenNetLinkApp.sh ${SLFF}/${TOPF}/${APPF}

cp -r bin.Script ${TOPF}/${APPF}
cp -r bin.Script ${SLFF}/${TOPF}/${APPF}

chmod 755 ${TOPF}/${APPF}/OpenNetLinkApp.sh
chmod 755 ${SLFF}/${TOPF}/${APPF}/OpenNetLinkApp.sh

#tar cvf bin.${DATE}.tar  ./${TOPF}
