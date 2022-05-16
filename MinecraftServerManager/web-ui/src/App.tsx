import './App.css';
import { Button, Col, Collapse, Container, DropdownItem, DropdownMenu, DropdownToggle, Nav, Navbar, NavbarBrand, NavbarText, NavbarToggler, NavItem, NavLink, Offcanvas, OffcanvasBody, OffcanvasHeader, Row, UncontrolledDropdown } from 'reactstrap';
import { ServersList } from './components/ServersList';
import { ServerLogs } from './components/ServerLogs';
import { ServerUsers } from './components/ServerUsers';
import { useState } from 'react';
import { useSelectedGameServerName } from './hooks/gameServersHooks';
import { ServerBackupButton } from './components/ServerButtons';
import { ToastContainer } from 'react-toastify';
import { ServerBackupsList } from './components/ServerBackupsList';

function App() {
	const serverId = useSelectedGameServerName()
	return (
		<div className="App">
			<Container fluid="true">
				<Navbar
					color="light"
					expand="md"
					light
				>
					{/*<NavbarBrand href="/">*/}
					{/*	reactstrap*/}
					{/*</NavbarBrand>*/}
					{/*<NavbarToggler onClick={function noRefCheck() { }} />*/}
					<Collapse navbar>
						<Nav
							className="me-auto"
							navbar
						>
							{/*<NavItem>*/}
							{/*	<NavLink href="/components/">*/}
							{/*		Components*/}
							{/*	</NavLink>*/}
							{/*</NavItem>*/}
							{/*<NavItem>*/}
							{/*	<NavLink href="https://github.com/reactstrap/reactstrap">*/}
							{/*		GitHub*/}
							{/*	</NavLink>*/}
							{/*</NavItem>*/}
							<UncontrolledDropdown
								inNavbar
								nav
							>
								<DropdownToggle
									caret
									nav
								>
									{serverId}
								</DropdownToggle>
								<ServersList />
							</UncontrolledDropdown>
						</Nav>
						<NavbarText>
							<ServerBackupButton />
						</NavbarText>
					</Collapse>
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
						<ServerBackupsList />
					</Col>
				</Row>
			</Container>
			<ToastContainer position="bottom-center" newestOnTop={true} pauseOnFocusLoss={true} />

		</div>
	);
}

export default App;
