import React from "react";
import { useEffect, useRef, useState } from "react";
import { useSelectedGameServerName } from "../hooks/gameServersHooks";
import ServerClient from "../libs/ApiClient";

function ServerConsoleLogs(props: object) {
	const serverName = useSelectedGameServerName();
	const [lines, setLines] = useState<string[]>([])
	const endRef = useRef<any>(null);
	const list = React.Children.toArray(lines.map((l) => { return (<div>{l}</div>) }))
	useEffect(() => {
		ServerClient.ReadLogs(serverName!)
			.then(logs => {
				setLines(logs)
			})
	}, [serverName])
	useEffect(() => {
		endRef.current.scrollIntoView()
	}, [lines])
	return (<div>{list}<div ref={endRef} /></div>)
}

function ServerConsoleLogStream(props: object) {
	const serverName = useSelectedGameServerName();
	const logDest = useRef<any>(null);

	useEffect(() => {
		const handler = (line: string) => {
			console.debug(`console line: ${line}`);
			const el = document.createElement('div')
			el.className = "text-start"
			el.innerHTML = line;
			console.info(logDest.current?.appendChild(el));
			logDest.current.scrollIntoView()
		}
		ServerClient.subscribe(serverName as string, handler);
		return () => {
			ServerClient.unsubscribe(serverName as string, handler);
		}
	}, [serverName])
	return (<div ref={logDest}></div>)
}

function ServerLogs() {
	return (
		<code className="lh-sm" >
			<pre>
				<div className=" overflow-auto" style={{ "maxHeight": "400px" }}>
					<ServerConsoleLogs />
					<ServerConsoleLogStream />
				</div>
			</pre>
		</code>
	)

}

export { ServerLogs }