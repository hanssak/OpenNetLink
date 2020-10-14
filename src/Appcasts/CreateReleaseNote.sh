#!/bin/bash

## 파라미터가 없으면 종료 
if [ "$#" -lt 1 ]; then
	echo "$# is Illegal number of parameters."
	echo "Usage: $0 [AssemblyVersion]"
	exit -1
fi

echo "Testing Release Note $1 !!" > artifacts/packages/debian/$1/$1.md
exit 0

