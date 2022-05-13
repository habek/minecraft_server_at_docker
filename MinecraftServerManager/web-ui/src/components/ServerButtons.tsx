import { useEffect, useState } from "react"
import { toast } from "react-toastify"
import { Button } from "reactstrap"
import 'react-toastify/dist/ReactToastify.css'

function ServerBackupButton() {

	function doBackup() {
		toast("test")
	}

	return (
		<div>
			<Button color="primary" onClick={doBackup}>Backup</Button>
		</div>
	)
}

export { ServerBackupButton }