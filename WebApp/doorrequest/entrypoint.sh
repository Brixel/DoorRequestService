#!/bin/sh
if [ -z "$APIURL" ]
then
    echo "No APIURL found, aborting..."
    exit
else
    echo "APIURL found: $APIURL"
fi

echo "{ \"apiUri\": \"$APIURL\",\"authUri\": \"$APIURL\" }"  > ./usr/share/nginx/html/assets/config.json

echo "Starting nginx..."
nginx -g "daemon off;"
