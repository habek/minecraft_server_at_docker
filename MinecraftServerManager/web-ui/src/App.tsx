import React, { useEffect, useRef, useState } from 'react';
import './App.css';
import { Badge, Col, Container, Row } from 'reactstrap';
import { ServersList } from './components/ServersList';
import { ServerLogs } from './components/ServerLogs';
import { useSelectedGameServerName } from './hooks/gameServersHooks';
import { GameServerContext } from './contexts';
import { ServerUsers } from './components/ServerUsers';

function App() {
	const serverName = useSelectedGameServerName();
	return (
		<div className="App">
			<GameServerContext.Provider value={serverName}>
				<Container fluid="true">
					<Row>
						<Col xs="2"><ServersList /></Col>
						<Col xs="10">
							<ServerUsers />
						</Col>
					</Row>
					<Row>
						<Col>
							<ServerLogs />
						</Col>
					</Row>
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
				</Container>
			</GameServerContext.Provider>
		</div>
	);
}

export default App;
