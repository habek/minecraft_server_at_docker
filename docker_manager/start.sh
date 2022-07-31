#!/bin/bash
cd /app/web-proxy
node server.js &
sleep 2

cd /app/web-ui 
# node node_modules/serve/bin/serve.js &
npx serve &

sleep 2
cd /app/web-api
chmod +x web-api
export ASPNETCORE_URLS=http://localhost:5054
dotnet web-api.dll

