#!/bin/bash

## 파라미터가 없으면 종료 
if [ "$#" -lt 1 ]; then
	echo "$# is Illegal number of parameters."
	echo "Usage: $0 [options]"
	exit 1
fi
args=("$@")

## for loop 를 파라미터 갯수만큼 돌리기 위해 three-parameter loop control 사용
for (( c=0; c<$#; c=c+2 ))
do
	#echo "$c th parameter = ${args[$c]}";
	echo "${args[$c]} : ${args[$c+1]}";
done
