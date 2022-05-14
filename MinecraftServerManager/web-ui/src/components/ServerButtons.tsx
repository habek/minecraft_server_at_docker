import { useEffect, useState } from "react"
import { toast } from "react-toastify"
import { Button } from "reactstrap"
import 'react-toastify/dist/ReactToastify.css'
import ServerClient from "../libs/ServerClient"
import { useSelectedGameServerName } from "../hooks/gameServersHooks"

function ServerBackupButton() {
	const serverId = useSelectedGameServerName()
	function doBackup() {
		ServerClient.DoBackup(serverId)
	}

	return (
		<div>
			<Button color="primary" onClick={doBackup}>Backup</Button>
		</div>
	)
}

export { ServerBackupButton }