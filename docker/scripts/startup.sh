#!/bin/bash
if [ -d "/backup_to_restore" ]; then
	echo Removing curent worlds...
	rm -r "/bedrock/worlds"
	echo Restoring worlds from backup...
	cp -a "/backup_to_restore/." "/bedrock"
	echo Cleaning temporary files...
	rm -r "/backup_to_restore"
	echo World restored
fi
/scripts/update.sh
/bedrock/bedrock_server