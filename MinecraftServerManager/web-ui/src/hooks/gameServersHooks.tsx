import { useEffect, useState } from "react";
import { StringParam, useQueryParam } from "use-query-params";
import ServerClient from "../libs/ApiClient";

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

export { useAllServerNames, useSelectedGameServerName }