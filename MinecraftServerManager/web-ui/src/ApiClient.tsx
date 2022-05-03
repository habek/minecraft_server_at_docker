const events = require('events');
const signalR = require("@microsoft/signalr");
//import { ISubscription } from '@microsoft/signalr';

const subscribers: any = {}
const subscribedServerNames: string[] = []

class ServerProxy extends events.EventEmitter {

	constructor(url: string) {
		super()
		this.signalrConnection = new signalR.HubConnectionBuilder()
			.withUrl(url)
			.withAutomaticReconnect()
			.configureLogging(signalR.LogLevel.Information)
			.build();
		this.signalrConnection.start().then(() => this.registerConsoleEvents())
		this.signalrConnection.on("ServerListChanged", (serverIds: string[]) => {
			console.debug(`Servers: ${serverIds}`);
			this.serverNames = serverIds;
			this.emit('ServerListChanged', serverIds)
		});
	}
	signalrConnection: any;
	serverNames: string[] = []
	registerConsoleEvents() {
		if (this.signalrConnection.state !== "Connected") {
			return;
		}
		for(const serverName in subscribers){
			if (subscribers[serverName].length > 0) {
				if (subscribedServerNames.includes(serverName)) {
					console.info("TODO: unregister console")
				}
			}
			if (!subscribedServerNames.includes(serverName)) {
				this.signalrConnection.stream("ReadConsole", serverName).subscribe({
					next: (line: string) => {
						console.debug(`console ${serverName}, line: ${line}`);
						subscribers[serverName].forEach((subscriber: (arg0: string) => any) => subscriber(line))
					},
					complete: () => { },
					error: (err: string) => {
						console.error(`console ${serverName}, error: ${err}`);
					}
				})
			}
		}
	}
	subscribe(serverName: string, action: (line: string) => void) {
		if (!subscribers[serverName]) {
			subscribers[serverName] = []
		}
		subscribers[serverName].push(action)
		this.registerConsoleEvents()
	}
	unsubscribe(serverName: string, action: (line: string) => void) {
		if (!subscribers[serverName]) {
			return;
		}
		subscribers[serverName].filter((func: (line: string) => void) => func !== action)
	}
}

const ServerClient = new ServerProxy("/api/events");

export default ServerClient;