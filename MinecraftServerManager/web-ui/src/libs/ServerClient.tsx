import { HubConnection, IRetryPolicy, RetryContext } from "@microsoft/signalr";
import { toast } from "react-toastify";

const events = require('events');
const signalR = require("@microsoft/signalr");

const logListeners: any = {}
const subscribedServerNames: string[] = []

class RetryPolicy implements IRetryPolicy {
	nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
		return 5000;
	}
}
enum ChangedDataType {
	Users = "Users",
	State = "State",
	Configuration = "Configuration"
}

class XBoxUser {
	userName: string | undefined
	xuid: string | undefined
}

class UserInfo {
	isConnected: boolean | undefined
	user: XBoxUser | undefined
	permission: string | undefined
}

class ServerInfo {
	constructor(serverId: string) {
		this.serverId = serverId;
	}
	serverId: string
	usersNumber: number | undefined
}

class ServersInfo {
	[serverId: string]: ServerInfo
}
class BackupInfo {
	name: string | undefined
	size: number | undefined
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
			console.debug(`ServerListChanged: ${serverIds}`);
			this.serverNames = serverIds;
			for (var serverId of serverIds) {
				if (this.getServerInfo(serverId).usersNumber === undefined) {
					this.GetUsers(serverId)
				}
			}
			localStorage.setItem("ServersInfo", JSON.stringify(this.serversInfo))
			this.emit('ServerListChanged', serverIds)
		});
		const serversInfo = localStorage.getItem("ServersInfo")
		if (serversInfo !== null) {
			this.serversInfo = JSON.parse(serversInfo);
			for (var serverId in this.serversInfo) {
				this.serversInfo[serverId].usersNumber = undefined
				this.GetUsers(serverId)
			}
		}
		this.signalrConnection.on("DataChanged", (serverId: string, changedData: ChangedDataType) => {
			console.debug(`DataChanged: ${serverId}, ${changedData}`);
			if (changedData === ChangedDataType.Users) {
				this.GetUsers(serverId)
			}
		})
		//	JavaScript

		//	Copy
		//	$.connection.hub.disconnected(function () {
		//		if ($.connection.hub.lastError) { alert("Disconnected. Reason: " + $.connection.hub.lastError.message); }
		//	});
	}
	signalrConnection: HubConnection;
	serverNames: string[] = []
	serversInfo: ServersInfo = new ServersInfo()
	getServerInfo(serverId: string) {
		var server = this.serversInfo[serverId]
		if (!server) {
			server = new ServerInfo(serverId)
			this.serversInfo[serverId] = server
		}
		return server
	}
	registerConsoleEvents(force: boolean) {
		if (force) {
			subscribedServerNames.length = 0
		}
		if (this.signalrConnection.state !== "Connected") {
			return;
		}
		for (const serverName in logListeners) {
			if (logListeners[serverName].length > 0) {
				if (subscribedServerNames.includes(serverName)) {
					console.info("TODO: unregister console")
					//const [subscription, setSubscription] = useState<ISubscription<string>>(null as unknown as ISubscription<string>)
				}
			}
			if (!subscribedServerNames.includes(serverName)) {
				this.signalrConnection.stream("ReadConsole", serverName).subscribe({
					next: (line: string) => {
						console.debug(`console ${serverName}, line: ${line}`);
						logListeners[serverName].forEach((subscriber: (arg0: string) => any) => subscriber(line))
					},
					complete: () => { },
					error: (err: string) => {
						console.error(`console ${serverName}, error: ${err}`);
						toast.error(JSON.stringify(err))
					}
				})
			}
		}
	}
	subscribeLogs(serverName: string, action: (line: string) => void) {
		console.info(`Start subscribing for ${serverName}`);
		if (!serverName) {
			return;
		}
		if (!logListeners[serverName]) {
			logListeners[serverName] = []
		}
		logListeners[serverName].push(action)
		this.registerConsoleEvents(false)
	}
	unsubscribeLogs(serverName: string, action: (line: string) => void) {
		console.info(`Stop subscribing for ${serverName}`);
		if (!logListeners[serverName]) {
			return;
		}
		logListeners[serverName] = logListeners[serverName].filter((func: (line: string) => void) => func !== action)
	}

	async ReadLogs(serverName: string): Promise<string[]> {
		try {
			const response = await fetch(`/api/GameServer/Logs/${encodeURIComponent(serverName)}`, { method: "GET" });
			var data = response.json() as unknown as Promise<string[]>
			return data;
		} catch (err) {
			console.error(err);
			toast.error(JSON.stringify(err))
			return []
		}
	}
	async GetUsers(serverId: string): Promise<UserInfo[]> {
		try {
			const getUsersPath = `/api/GameServer/Users/${encodeURIComponent(serverId)}`;
			const response = await fetch(getUsersPath, { method: "GET" });
			if (response.status > 200) {
				console.warn(`Call to ${getUsersPath} returned ${response.status} ${response.statusText}`)
				return [];
			}
			var userInfos = await (response.json() as unknown as Promise<UserInfo[]>)
			var activeUsersNumber = 0;
			for (var user of userInfos) {
				if (user.isConnected) {
					activeUsersNumber++
				}
			}
			this.getServerInfo(serverId).usersNumber = activeUsersNumber
			console.log("UsersDataChanged_" + serverId)
			this.emit("UsersDataChanged_" + serverId, userInfos);
			return userInfos;
		} catch (err) {
			console.error(err);
			toast.error(JSON.stringify(err))
			return []
		}
	}

	async handleApiCall(input: RequestInfo, init?: RequestInit): Promise<Response | undefined> {
		try {
			var response = await fetch(input, init)
			if (!response.ok) {
				toast.error(JSON.stringify(response.statusText))
			}
		}
		catch (err) {
			console.error(err);
			toast.error(JSON.stringify(err))
			return undefined
		}
	}

	async DoBackup(serverId: string) {
		try {
			var response = await fetch(`/api/GameServer/Actions/Backup/${encodeURIComponent(serverId)}`, { method: "GET" })
			if (!response.ok) {
				toast.error(JSON.stringify(response.statusText))
			}
		}
		catch (err) {
			console.error(err);
			toast.error(JSON.stringify(err))
			return []
		}
	}

	async RestoreBackup(serverId: string, backupName: string) {
		await this.handleApiCall(`/api/GameServer/${encodeURIComponent(serverId)}/restore/${backupName}`, { method: "GET" })
	}

	async GetBackups(serverId: string): Promise<BackupInfo[]> {
		try {
			const response = await fetch(`/api/GameServer/${encodeURIComponent(serverId)}/backups`, { method: "GET" });
			if (!response.ok) {
				toast.error(response.statusText)
			}
			return await (response.json() as unknown as Promise<BackupInfo[]>)
		} catch (err) {
			console.error(err);
			toast.error(JSON.stringify(err))
			return []
		}
	}
}

const ServerClient = new ServerProxy("/api/events");

export default ServerClient;
export { ChangedDataType, UserInfo, BackupInfo }