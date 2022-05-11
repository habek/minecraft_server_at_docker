import { useState } from "react";
import { Badge, ListGroup, ListGroupItem } from "reactstrap";
import { StringParam, useQueryParam } from "use-query-params";
import { useAllServerNames, useNumberOfActivePlayers, useSelectedGameServerName } from "../hooks/gameServersHooks";


interface ServerButtonParams {
	serverName: string
}

function ServerButton(props: ServerButtonParams) {
	const setQueryParam = useQueryParam("serverName", StringParam)[1];
	const activeUsersCount = useNumberOfActivePlayers(props.serverName)??11

	const setParam = () => {
		setQueryParam(props.serverName)
		window.location.href = window.location.href
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
