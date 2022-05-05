import { userInfo } from "os"
import { useEffect, useState } from "react"
import { Table } from "reactstrap"
import { useSelectedGameServerName } from "../hooks/gameServersHooks"
import ServerClient, { ChangedDataType, UserInfo } from "../libs/ApiClient"

function ServerUsers() {
	const serverName = useSelectedGameServerName()
	const [userInfos, setUserInfos] = useState<UserInfo[]>([])
	useEffect(() => {
		const getUsers = () => {
			ServerClient.GetUsers(serverName).then((userInfos: UserInfo[]) => {
				setUserInfos(userInfos)
			});
		}
		getUsers();
		ServerClient.on("UsersDataChanged_" + serverName, getUsers);
		return () => {
			ServerClient.off("UsersDataChanged_" + serverName, getUsers);
		}

	}, [serverName])

	const userRows = userInfos.map(userInfo => (<tr>
		<td>
			{userInfo.isConnected ? "Online": ""}
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
		<Table>
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