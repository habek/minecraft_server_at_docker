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
						<br />
						<a href="/api/swagger/index.html">Api</a>
					</Col>
				</Row>
				<Row>
					<Col xs="2"><ServersList /></Col>
					<Col xs="10">
						<code className="lh-sm" >
							<pre>
								<div className=" overflow-auto" style={{ "maxHeight": "700px" }}>
									<ServerConsoleLogs />
									<ServerConsoleLogStream />
								</div>
							</pre>
						</code>
					</Col>
				</Row>
			</Container>
		</div>
	);
}

function ServerConsoleLogs(props: object) {
	const serverName = useSelectedGameServerName();
	const [lines, setLines] = useState<string[]>([])
	const endRef = useRef<any>(null);
	const list = React.Children.toArray(lines.map((l) => { return (<div>{l}</div>) }))
	useEffect(() => {
		ServerClient.ReadLogs(serverName!)
			.then(logs => {
				setLines(logs)
			})
	}, [serverName])
	useEffect(() => {
		endRef.current.scrollIntoView()
	}, [lines])
	return (<div>{list}<div ref={endRef} /></div>)
}

function ServerConsoleLogStream(props: object) {
	const serverName = useSelectedGameServerName();
	const logDest = useRef<any>(null);

	useEffect(() => {
		const handler = (line: string) => {
			console.debug(`console line: ${line}`);
			const el = document.createElement('div')
			el.className = "text-start"
			el.innerHTML = line;
			console.info(logDest.current?.appendChild(el));
			logDest.current.scrollIntoView()
		}
		ServerClient.subscribe(serverName as string, handler);
		return () => {
			ServerClient.unsubscribe(serverName as string, handler);
		}
	}, [serverName])
	return (<div ref={logDest}></div>)
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

export default App;
