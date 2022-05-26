#!/bin/bash
cd /app/web-ui 
# node node_modules/serve/bin/serve.js &

cd /app/web-api
chmod +x web-api
# ls -la
# dotnet --info
dotnet web-api.dll

cd /app/web-proxy
node server.js