#!/bin/bash
if [ -d "/bedrock/worlds/backup_restore" ]; then
    rm -r "/bedrock/worlds/Bedrock level"
    mv -v "/bedrock/worlds/backup_restore" "/bedrock/worlds/Bedrock level"
fi
/scripts/update.sh
/bedrock/bedrock_server