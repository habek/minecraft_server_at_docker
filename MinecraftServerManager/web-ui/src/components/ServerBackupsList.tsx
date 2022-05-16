import { useEffect, useState } from "react"
import { Button, Table } from "reactstrap"
import { useSelectedGameServerName } from "../hooks/gameServersHooks"
import ServerClient, { BackupInfo } from "../libs/ServerClient"

function ServerBackupsList() {
	const serverName = useSelectedGameServerName()
	const [backups, setBackups] = useState<BackupInfo[]>([])
	useEffect(() => {
		if (!serverName) {
			return;
		}
		ServerClient.GetBackups(serverName).then(backupInfos => setBackups(backupInfos))

	}, [serverName])
	if (!serverName) {
		return (<div />)
	}
	const restoreBackup = (backupName: string | undefined) => {
		if (!backupName) {
			return
		}
		ServerClient.RestoreBackup(serverName, backupName)
	}

	const rows = backups.map(backupInfo => (
		<tr key={backupInfo.name}>
			<th>
				{backupInfo.name}
			</th>
			<th>
				{backupInfo.size?.toLocaleString('pl')}
			</th>
			<th>
				<Button size="sm" color="primary" onClick={(e) => restoreBackup(backupInfo.name)}>Restore</Button>
			</th>
		</tr>));
	return (
		<div>
			<Table striped size="sm">
				<thead>
					<tr>
						<th>
							Name
						</th>
						<th>
							Size
						</th>
						<th>
							Action
						</th>
					</tr>
				</thead>
				<tbody>
					{rows}
				</tbody>
			</Table>
		</div>
	)
}
export { ServerBackupsList }