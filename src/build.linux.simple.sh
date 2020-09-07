#/bin/bash

DATE=`date +%y%m%d.%H.%M` 


TOPF=hanssak
APPF=OpenNetLinkApp

\rm -rf ${TOPF}
mkdir -p  ${TOPF}/${APPF}
dotnet clean ;
dotnet build -c Release -o  ${TOPF}/${APPF}

tar cvf bin.${DATE}.tar  ./${TOPF}
