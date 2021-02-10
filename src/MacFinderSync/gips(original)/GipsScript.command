#!/bin/sh

#  GipsScript.command
#  Gips
#
#  Created by Moaaz Sidat on 2015-08-19.
#  Copyright (c) 2015 MS. All rights reserved.
echo "******************************"
echo "Gips Started"
echo "******************************"

cp "${1}" "${2}"

/usr/bin/sips "${3}" $4 "${5}"

echo "******************************"
echo "Gips Ended"
echo "******************************"