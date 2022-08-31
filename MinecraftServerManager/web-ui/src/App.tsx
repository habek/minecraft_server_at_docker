import './App.css';
import { Col, Container, Row } from 'reactstrap';
import { ServerLogs } from './components/ServerLogs';
import { ServerUsers } from './components/ServerUsers';
import { useEffect } from 'react';
import { useAllServerNames, useSelectedGameServerName } from './hooks/gameServersHooks';
import { ServerBackupsList } from './components/ServerBackupsList';
import { Link, useParams } from 'react-router-dom';
import { routeServersIndex } from './routes/routesList';
import { Layout } from './components/Layout';

function App() {
	const serverId = useSelectedGameServerName()
	const params = useParams()
	const allServers = useAllServerNames();
	useEffect(() => {
		// if (!serverId) {
		// 	// let serverName = params.serverId;
		// 	const defaultServer = allServers.length > 0 ? allServers[0] : null;
		// 	console.info(`Default server: ${defaultServer}`)
		// 	// if (!!defaultServer) {
		// 	// 	const defaultUrl = `${encodeURIComponent(defaultServer)}`;
		// 	// 	console.warn(`Invalid server id: ${serverName}, navigating to first server on list: ${defaultUrl}`)
		// 	// 	navigate(defaultUrl)
		// 	// }
		// 	// navigate(routeServersIndex)
		// }
	});
	if (!serverId && allServers != null) {
		return <div><h1>Unknown server: "{params.serverId}"</h1><Link to={routeServersIndex}>Go to servers list</Link> </div>
	}
	return (
		<Layout>
			<Container fluid="true">
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
		</Layout>
	);
}

export default App;
