REM @echo off
pushd "web-ui"
call :start "web-ui" npm start
popd

pushd "web-proxy"
call :start "web-proxy" node server.js
popd
goto :eof

:start
wt -w ShipStation new-tab --title %1 -p "Command Prompt" cmd /k %2 %3 %4 %5 %6 %7 %8 %9
goto :eof
