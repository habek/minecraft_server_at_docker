﻿import './App.css';
import { Button, Col, Container, Navbar, NavbarBrand, NavbarText, NavbarToggler, Offcanvas, OffcanvasBody, OffcanvasHeader, Row } from 'reactstrap';
import { ServersList } from './components/ServersList';
import { ServerLogs } from './components/ServerLogs';
import { ServerUsers } from './components/ServerUsers';
import { useState } from 'react';
import { useSelectedGameServerName } from './hooks/gameServersHooks';
import { ServerBackupButton } from './components/ServerButtons';
import { ToastContainer } from 'react-toastify';

function App() {
	const [isServerListOpen, setIsServerListOpen] = useState(false)
	const serverId = useSelectedGameServerName()
	return (
		<div className="App">
			<Container fluid="true">
				<Offcanvas isOpen={isServerListOpen} backdrop={false} scrollable={true}>
					<OffcanvasHeader toggle={() => setIsServerListOpen(false)}>
						Minecraft servers
					</OffcanvasHeader>
					<OffcanvasBody>
						<ServersList />
					</OffcanvasBody>
				</Offcanvas>
				<Navbar color="light" light expand="md">
					<NavbarBrand onClick={() => setIsServerListOpen(true)}>
						{serverId}
					</NavbarBrand>
					<ServerBackupButton />
				</Navbar>
				<Row>
					{/*<Col xs="2"><ServersList /></Col>*/}
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
			<ToastContainer position="bottom-center" newestOnTop={true} pauseOnFocusLoss={true} />

		</div>
	);
}

export default App;
