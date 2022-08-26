#!/bin/bash
if [ -d "/bedrock/worlds/backup_restore" ]; then
    echo Restoring world from backup...
    rm -r "/bedrock/worlds/Bedrock level"
    mv -v "/bedrock/worlds/backup_restore" "/bedrock/worlds/Bedrock level"
    echo Backup restored
fi
/scripts/update.sh
/bedrock/bedrock_server