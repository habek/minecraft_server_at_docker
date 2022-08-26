#!/bin/bash
if [ -d "/backup_to_restore" ]; then
	echo Restoring world from backup...
	cp -R "/backup_to_restore/*" "/bedrock"
	rm -r "/backup_to_restore"
	echo Backup restored
fi
/scripts/update.sh
/bedrock/bedrock_server