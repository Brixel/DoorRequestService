#!/bin/sh
if [ -z "$APISERVICE" ]
then
    echo "No APISERVICE found, aborting..."
    exit
else
    echo "APISERVICE found: $APISERVICE"
fi

if [ -z "$ISSUER" ]
then
    echo "No ISSUER found, aborting..."
    exit
else
    echo "ISSUER found: $ISSUER"
fi

if [ -z "$CLIENTID" ]
then
    echo "No CLIENTID found, aborting..."
    exit
else
    echo "CLIENTID found: $CLIENTID"
fi

sed -i "s/apiService: ""/apiService: $APISERVICE/g" /usr/share/nginx/html/main.*.js
sed -i "s/issuer: ""/issuer: $ISSUER/g" /usr/share/nginx/html/main.*.js
sed -i "s/clientId: ""/clientId: $CLIENTID/g" /usr/share/nginx/html/main.*.js

echo "Starting nginx..."
nginx -g "daemon off;"
