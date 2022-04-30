import React from 'react';
import logo from './logo.svg';
import './App.css';
const signalR = require("@microsoft/signalr");
let signalRconnection = new signalR.HubConnectionBuilder()
	.withUrl("/api/events")
	.withAutomaticReconnect()
	.configureLogging(signalR.LogLevel.Information)
	.build();
signalRconnection.on("ServerListChanged", (serverIds: string[]) => {
	console.debug(`<div class="message-author">${serverIds}</div>`);
});
signalRconnection.start().catch((err: string) => {
	console.error(err);
});

function App() {
	return (
		<div className="App">
			<header className="App-header">
				{/*<img src={logo} className="App-logo" alt="logo" />*/}
				<p>
					Edit <code>src/App.tsx</code> and save to reload.
				</p>
				<a
					className="App-link"
					href="https://reactjs.org"
					target="_blank"
					rel="noopener noreferrer"
				>
					Learn React
				</a>
				<a href="/api/swagger/index.html">Api</a>
				<ShoppingList />
			</header>
		</div>
	);
}

class ShoppingList extends React.Component {
	render() {
		return (
			<div className="shopping-list">
				{/*<h1>Shopping List for {this.props.name}</h1>*/}
				<h1>Shopping List for </h1>
				<ul>
					<li>Instagram</li>
					<li>WhatsApp</li>
					<li>Oculus</li>
				</ul>
			</div>
		);
	}
};

export default App;
