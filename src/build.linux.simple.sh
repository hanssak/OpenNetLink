#/bin/bash

DATE=`date +%y%m%d.%H.%M` 


TOPF=hanssak
APPF=OpenNetLinkApp

\rm -rf ${TOPF}
mkdir -p  ${TOPF}/${APPF}
dotnet clean ;
dotnet build -c Release -o  ${TOPF}/${APPF}

cp bin.Script/OpenNetLinkApp.sh ${TOPF}/${APPF}
chmod 755 ${TOPF}/${APPF}/OpenNetLinkApp.sh

tar cvf bin.${DATE}.tar  ./${TOPF}
