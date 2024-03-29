import { useEffect, useState } from "react"
import { Table } from "reactstrap"
import { useSelectedGameServerName } from "../hooks/gameServersHooks"
import ServerClient, { UserInfo } from "../libs/ServerClient"

function ServerUsers() {
	const serverName = useSelectedGameServerName()
	const [userInfos, setUserInfos] = useState<UserInfo[]>([])
	useEffect(() => {
		const getUsersCallback = (userInfos: UserInfo[]) => {
			// console.debug("getUsersCallback")
			setUserInfos(userInfos)
		}
		ServerClient.on("UsersDataChanged_" + serverName, getUsersCallback);
		if (serverName) {
			ServerClient.GetUsers(serverName)
		}
		return () => {
			ServerClient.off("UsersDataChanged_" + serverName, getUsersCallback);
		}

	}, [serverName])
	if (!serverName) {
		return (<div />)
	}

	const userRows = userInfos.map(userInfo => (<tr key={userInfo.user?.xuid}>
		<td>
			{userInfo.isConnected ? "Online" : ""}
		</td>
		<td >
			{userInfo.user?.userName}
		</td>
		<td>
			{userInfo.user?.xuid}
		</td>
		<td>
			{userInfo.permission}
		</td>
	</tr>));
	return (
		<Table striped size="sm">
			<thead>
				<tr>
					<th>
						In game
					</th>
					<th>
						Name
					</th>
					<th>
						Xuid
					</th>
					<th>
						Permission
					</th>
				</tr>
			</thead>
			<tbody>
				{userRows}
			</tbody>
		</Table>
	)
}
export { ServerUsers }