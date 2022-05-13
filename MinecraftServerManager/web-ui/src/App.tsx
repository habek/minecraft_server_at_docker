import './App.css';
import { Col, Container, Row } from 'reactstrap';
import { ServersList } from './components/ServersList';
import { ServerLogs } from './components/ServerLogs';
import { ServerUsers } from './components/ServerUsers';

function App() {
	return (
		<div className="App">
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
		</div>
	);
}

export default App;
