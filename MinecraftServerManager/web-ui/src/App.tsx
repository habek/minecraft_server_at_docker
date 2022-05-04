import React, { useEffect, useRef, useState } from 'react';
import './App.css';
import { Badge, Col, Container, ListGroup, ListGroupItem, Row } from 'reactstrap';
import { StringParam, useQueryParam } from 'use-query-params';
import ServerClient from './ApiClient';

function App() {
	return (
		<div className="App">
			<Container fluid="true">
				<Row>
					<Col>
						<a
							className="App-link"
							href="https://reactjs.org"
							target="_blank"
							rel="noopener noreferrer"
						>
							Learn React
						</a>
						<br/>
						<a href="/api/swagger/index.html">Api</a>
					</Col>
				</Row>
				<Row>
					<Col xs="2"><ServersList /></Col>
					<Col xs="10">
						<ServerConsoleLogs />
					</Col>
				</Row>
			</Container>
		</div>
	);
}

function ServerConsoleLogs() {
	const serverName = useSelectedGameServerName();
	const logDest = useRef<any>(null);
	useEffect(() => {
		const handler = (line: string) => {
			console.debug(`console line: ${line}`);
			const el = document.createElement('div')
			el.className ="text-start lh-1"
			el.innerHTML = line;
			console.info(logDest.current?.appendChild(el));
		}
		ServerClient.subscribe(serverName as string, handler);
		return () => {
			ServerClient.unsubscribe(serverName as string, handler);
		}
	}, [serverName])
	return (<code><pre><div ref={logDest} className="lh-sm" >Tu są logi<br />i druga linia</div></pre></code>)
}

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

function useSelectedGameServerName() {
	const [serverName, setServerName] = useQueryParam("serverName", StringParam)
	const allServers = useAllServerNames();
	if (allServers.length === 0) {
		return serverName;
	}
	for (let name of allServers) {
		if (name === serverName) {
			return serverName;
		}
	}
	setServerName(allServers[0]);
	return serverName;
}

interface ServerButtonParams {
	serverName: string
}
function ServerButton(props: ServerButtonParams) {
	const setQueryParam = useQueryParam("serverName", StringParam)[1];

	const setParam = () => {
		setQueryParam(props.serverName)
		window.location.href = window.location.href
	}
	return (
		<div onClick={setParam}>
			{props.serverName}
			<Badge pill> 1</Badge>
		</div>);
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


//function checkSignalR(cb: CallableFunction) {
//	if (signalrConnection.state === "Connected") {
//		cb(signalrConnection)
//		return;
//	}
//	setTimeout(() => { checkSignalR(cb) }, 100);
//}
//function useSignalrConnection() {
//	const [connection, setConnection] = useState<any>(null)
//	useEffect(() => {
//		checkSignalR(setConnection)
//	}, []);
//	return connection;
//}

export default App;
