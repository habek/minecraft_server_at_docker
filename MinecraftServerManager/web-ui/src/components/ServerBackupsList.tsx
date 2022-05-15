import { useEffect, useState } from "react"
import { Table } from "reactstrap"
import { useSelectedGameServerName } from "../hooks/gameServersHooks"
import ServerClient, { BackupInfo } from "../libs/ServerClient"

function ServerBackupsList() {
	const serverName = useSelectedGameServerName()
	const [backups, setBackups] = useState<BackupInfo[]>([])
	useEffect(() => {
		if (!serverName) return;
		ServerClient.GetBackups(serverName).then(backupInfos => setBackups(backupInfos))

	}, [serverName])
	if (!serverName) {
		return (<div />)
	}

	const rows = backups.map(backupInfo => (
		<div key={backupInfo.name}>
			{backupInfo.name}, Size: {backupInfo.size}
		</div>));
	return (
		<div>
			{rows}
		</div>
	)
}
export { ServerBackupsList }