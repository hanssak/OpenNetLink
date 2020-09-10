#/bin/bash

DATE=`date +%y%m%d.%H.%M` 

SLFF=selfcontained
TOPF=hanssak
APPF=OpenNetLinkApp


\rm -rf ${TOPF}
\rm -rf ${SLFF}

dotnet clean ;

dotnet publish -c Release -o  ${TOPF}/${APPF}
dotnet publish -c Release -o  ${SLFF}/${TOPF}/${APPF} -r linux-x64


cp bin.Script/OpenNetLinkApp.sh ${TOPF}/${APPF}
cp bin.Script/OpenNetLinkApp.sh ${SLFF}/${TOPF}/${APPF}

cp -r bin.Script ${TOPF}/${APPF}
cp -r bin.Script ${SLFF}/${TOPF}/${APPF}

chmod 755 ${TOPF}/${APPF}/OpenNetLinkApp.sh
chmod 755 ${SLFF}/${TOPF}/${APPF}/OpenNetLinkApp.sh

tar cvf bin.${DATE}.tar  ./${TOPF}
