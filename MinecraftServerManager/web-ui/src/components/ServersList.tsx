import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Badge, ListGroup, ListGroupItem } from "reactstrap";
import { useAllServerNames, useNumberOfActivePlayers, useSelectedGameServerName } from "../hooks/gameServersHooks";


interface ServerButtonParams {
	serverName: string
}

function ServerButton(props: ServerButtonParams) {
	const activeUsersCount = useNumberOfActivePlayers(props.serverName)
	const navigate = useNavigate();


	const setParam = () => {
		navigate("/servers/" + encodeURIComponent(props.serverName))
		//window.location.href = window.location.href
	}


	return (
		<div onClick={setParam} className="text-break">
			{props.serverName}
			<Badge pill>{activeUsersCount}</Badge>
		</div>
	);
}

function ServersList() {
	const selectedServer = useSelectedGameServerName()
	const allServerNames = useAllServerNames()
	return (
		<ListGroup>
			{allServerNames.map(serverName => (
				<ListGroupItem active={selectedServer === serverName} key={serverName} tag="button" action className="floatRight" >
					<ServerButton serverName={serverName} />
				</ListGroupItem>
			))}
		</ListGroup>
	);
};

export { ServersList }
