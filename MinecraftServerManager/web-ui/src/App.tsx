import './App.css';
import { Col, Collapse, Container, DropdownToggle, Nav, Navbar, NavbarText, Row, UncontrolledDropdown } from 'reactstrap';
import { ServersList } from './components/ServersList';
import { ServerLogs } from './components/ServerLogs';
import { ServerUsers } from './components/ServerUsers';
import { useEffect } from 'react';
import { useAllServerNames, useSelectedGameServerName } from './hooks/gameServersHooks';
import { ServerBackupButton } from './components/ServerButtons';
import { ToastContainer } from 'react-toastify';
import { ServerBackupsList } from './components/ServerBackupsList';
import { Link, useParams } from 'react-router-dom';
import { routeServersIndex } from './routes/routesList';

function App() {
	const serverId = useSelectedGameServerName()
	const params = useParams()
	const allServers = useAllServerNames();
	useEffect(() => {
		if (!serverId) {
			// let serverName = params.serverId;
			const defaultServer = allServers.length > 0 ? allServers[0] : null;
			console.info(`Default server: ${defaultServer}`)
			// if (!!defaultServer) {
			// 	const defaultUrl = `${encodeURIComponent(defaultServer)}`;
			// 	console.warn(`Invalid server id: ${serverName}, navigating to first server on list: ${defaultUrl}`)
			// 	navigate(defaultUrl)
			// }
			// navigate(routeServersIndex)
		}
	});
	if (!serverId) {
		return <div><h1>Unknown server: "{params.serverId}"</h1><Link to={routeServersIndex}>Go to servers list</Link> </div>
	}
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
