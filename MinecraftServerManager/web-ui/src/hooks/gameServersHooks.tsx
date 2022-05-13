import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
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
	const params = useParams()
	const allServers = useAllServerNames();
	let serverName = params.serverId;

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