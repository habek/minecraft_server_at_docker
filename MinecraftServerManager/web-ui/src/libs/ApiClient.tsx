import { HubConnection, IRetryPolicy, RetryContext } from "@microsoft/signalr";

const events = require('events');
const signalR = require("@microsoft/signalr");

const subscribers: any = {}
const subscribedServerNames: string[] = []

class RetryPolicy implements IRetryPolicy {
	nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
		return 5000;
	}
}

class ServerProxy extends events.EventEmitter {

	constructor(url: string) {
		super()
		this.signalrConnection = new signalR.HubConnectionBuilder()
			.withUrl(url)
			.withAutomaticReconnect(new RetryPolicy())
			.configureLogging(signalR.LogLevel.Information)
			.build();
		this.signalrConnection.start().then(() => this.registerConsoleEvents(false))
		this.signalrConnection.onreconnected(() => this.registerConsoleEvents(true))
		this.signalrConnection.on("ServerListChanged", (serverIds: string[]) => {
			console.debug(`Servers: ${serverIds}`);
			this.serverNames = serverIds;
			this.emit('ServerListChanged', serverIds)

		});
		//	JavaScript

		//	Copy
		//	$.connection.hub.disconnected(function () {
		//		if ($.connection.hub.lastError) { alert("Disconnected. Reason: " + $.connection.hub.lastError.message); }
		//	});
	}
	signalrConnection: HubConnection;
	serverNames: string[] = []
	registerConsoleEvents(force: boolean) {
		if (force) {
			subscribedServerNames.length = 0
		}
		if (this.signalrConnection.state !== "Connected") {
			return;
		}
		for (const serverName in subscribers) {
			if (subscribers[serverName].length > 0) {
				if (subscribedServerNames.includes(serverName)) {
					console.info("TODO: unregister console")
					//const [subscription, setSubscription] = useState<ISubscription<string>>(null as unknown as ISubscription<string>)
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
		this.registerConsoleEvents(false)
	}
	unsubscribe(serverName: string, action: (line: string) => void) {
		if (!subscribers[serverName]) {
			return;
		}
		subscribers[serverName] = subscribers[serverName].filter((func: (line: string) => void) => func !== action)
	}

	async ReadLogs(serverName: string): Promise<string[]> {
		try {
			const response = await fetch(`/api/GameServer/Logs/${encodeURIComponent(serverName)}`, { method: "GET" });
			var data = response.json() as unknown as Promise<string[]>
			return data;
		} catch (err) {
			console.error(err);
			return []
		}
	}
}

const ServerClient = new ServerProxy("/api/events");

export default ServerClient;