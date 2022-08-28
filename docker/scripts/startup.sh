#!/bin/bash
if [ -d "/backup_to_restore" ]; then
	echo Replacing world with files from backup...
	cp -a "/backup_to_restore/." "/bedrock"
	rm -r "/backup_to_restore"
	echo World restored
fi
/scripts/update.sh
/bedrock/bedrock_server