import './App.css';
import { Button, Col, Container, Navbar, NavbarBrand, NavbarText, NavbarToggler, Offcanvas, OffcanvasBody, OffcanvasHeader, Row } from 'reactstrap';
import { ServersList } from './components/ServersList';
import { ServerLogs } from './components/ServerLogs';
import { ServerUsers } from './components/ServerUsers';
import { useState } from 'react';
import { useSelectedGameServerName } from './hooks/gameServersHooks';
import { ServerBackupButton } from './components/ServerButtons';
import { ToastContainer } from 'react-toastify';
import { ServerBackupsList } from './components/ServerBackupsList';

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
						<ServerBackupsList/>
					</Col>
				</Row>
			</Container>
			<ToastContainer position="bottom-center" newestOnTop={true} pauseOnFocusLoss={true} />

		</div>
	);
}

export default App;
