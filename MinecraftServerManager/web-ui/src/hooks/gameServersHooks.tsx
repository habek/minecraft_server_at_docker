import { useEffect, useState } from "react";
import { StringParam, useQueryParam } from "use-query-params";
import ServerClient, { UserInfo } from "../libs/ApiClient";

function useAllServerNames() {
	const [serverList, setServerList] = useState<string[]>([]);

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

function useSelectedGameServerName(): string {
	const [serverName, setServerName] = useQueryParam("serverName", StringParam)
	const allServers = useAllServerNames();
	if (allServers.length === 0) {
		return serverName ?? "";
	}
	for (let name of allServers) {
		if (name === serverName) {
			return serverName;
		}
	}
	setServerName(allServers[0]);
	return serverName ?? "";
}

function useNumberOfActivePlayers(serverId: string) {
	const [activePlayersCount, setActivePlayersCount] = useState<number | undefined>(undefined)
	useEffect(() => {
		console.log("^^^^^^^^^^^^^^^^^^" + serverId, ServerClient.getServerInfo(serverId).usersNumber)
		setActivePlayersCount(ServerClient.getServerInfo(serverId).usersNumber)
		const hook = () => {
			setActivePlayersCount(ServerClient.getServerInfo(serverId).usersNumber)
		}
		ServerClient.on("UsersDataChanged_" + serverId, hook)
		return () => {
			ServerClient.off("UsersDataChanged_" + serverId, hook)
		}
	}, [serverId])
	return activePlayersCount
}

export { useAllServerNames, useSelectedGameServerName, useNumberOfActivePlayers }