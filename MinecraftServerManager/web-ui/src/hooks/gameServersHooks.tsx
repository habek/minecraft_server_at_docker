import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import ServerClient from "../libs/ServerClient";

function useAllServerNames() {
	const [serverList, setServerList] = useState<string[] | null>(null);

	useEffect(() => {
		setServerList(ServerClient.serverNames)
		const listener = (serverIds: string[]) => {
			setServerList(serverIds);
		}
		ServerClient.on("ServerListChanged", listener);
		return () => {
			ServerClient.off("ServerListChanged", listener)
		}
	}, [])
	return serverList;
}

export function useCurrentServerId(): string | undefined {
	const params = useParams()
	return params.serverId;
}

function useSelectedGameServerName(): string {
	const params = useParams()
	const allServers = useAllServerNames();
	let serverName = params.serverId;
	if (allServers == null) {
		return serverName || "";
	}
	for (let name of allServers) {
		if (name === serverName) {
			return serverName;
		}
	}
	return "";
}

function useNumberOfActivePlayers(serverId: string) {
	const [activePlayersCount, setActivePlayersCount] = useState<number | undefined>(undefined)
	useEffect(() => {
		const hook = () => {
			//console.log("^^^^^^^^^^^^^^^^^^" + serverId, ServerClient.getServerInfo(serverId).usersNumber)
			setActivePlayersCount(ServerClient.getServerInfo(serverId).usersNumber)
		}
		hook()
		ServerClient.on("UsersDataChanged_" + serverId, hook)
		return () => {
			ServerClient.off("UsersDataChanged_" + serverId, hook)
		}
	}, [serverId])
	return activePlayersCount
}

export { useAllServerNames, useSelectedGameServerName, useNumberOfActivePlayers }