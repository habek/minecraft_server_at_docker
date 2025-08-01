#!/bin/bash
# Test internet connectivity first
wget --quiet -U "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)" --timeout=30 http://www.minecraft.net/ -O /dev/null
if [ "$?" != 0 ]; then
    echo "Unable to connect to update website (internet connection may be down).  Skipping update ..."
else
    if [ "$BEDROCK_DOWNLOAD_URL" ]; then
      # Use specified version
      DownloadURL="$BEDROCK_DOWNLOAD_URL"
    else
      # # Download server index.html to check latest version
      # wget -U "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)" --timeout=30 -O /downloads/version.html https://minecraft.net/en-us/download/server/bedrock/
      # # DownloadURL=$(grep -o 'https://minecraft.azureedge.net/bin-linux/[^"]*' /downloads/version.html)
      # DownloadURL=$(grep -o 'https://www.minecraft.net/bedrockdedicatedserver/bin-linux/[^"]*' /downloads/version.html)
	  # https://www.minecraft.net/bedrockdedicatedserver/bin-linux/bedrock-server-1.21.93.1.zip
      # echo "Link found: $DownloadURL"
		set -e

		echo "🔍 Fetching latest Bedrock server info..."

		# 1. Query Mojang's JSON API for download links
		json=$(curl -s https://net-secondary.web.minecraft-services.net/api/v1.0/download/links)

		# 2. Extract URL for Linux Bedrock server
		DownloadURL=$(echo "$json" | jq -r '.result.links[]
				 | select(.downloadType=="serverBedrockLinux")
				 | .downloadUrl')

		if [[ -z "$DownloadURL" || "$DownloadURL" == "null" ]]; then
		  echo "❌ Could not find Bedrock server download URL. API may have changed."
		  exit 1
		fi

    fi
    DownloadFile=$(echo "$DownloadURL" | sed 's#.*/##')

    # Download latest version of Minecraft Bedrock dedicated server if a new one is available
    if [ -f "/downloads/$DownloadFile" ]
    then
        echo "Minecraft Bedrock server is up to date..."
    else
        echo "New version $DownloadFile is available.  Updating Minecraft Bedrock server ..."
        echo "Downloading $DownloadURL ..."
        rm -f /downloads/*
        wget -U "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)" -O "/downloads/$DownloadFile" "$DownloadURL" 
        echo "Downloading finished"
        if [ -f "/bedrock/server.properties" ]
        then
          unzip -q -o "/downloads/$DownloadFile" -x "*server.properties*" "*permissions.json*" "*whitelist.json*" -d /bedrock/
        else
          unzip -q -o "/downloads/$DownloadFile" -d /bedrock/
        fi
        chmod +x /bedrock/bedrock_server
    fi
fi
