#!/bin/bash
if [ -z "$APIURL" ]
then
    echo "No APIURL found, aborting..."
    exit
else
    echo "APIURL found: $APIURL"
fi

echo "{ \"apiURL\": \"$APIURL\" }"  > ./usr/share/nginx/html/assets/config.json
