import { Link } from "react-router-dom";
import { useAllServerNames } from "../hooks/gameServersHooks";
import { routeToServer } from "./routesList";


export function ServersList() {
  const allServerNames = useAllServerNames()
  return (
    <div>
      {
        allServerNames.map(serverName => (
          <div>
            <Link to={routeToServer(serverName)}>{serverName}</Link><br></br>
          </div>
        ))
      }
    </div>
  )
};

export default function ServersIndex() {
  return (
    <main style={{ padding: "1rem 0" }}>
      <h2>Hosted servers</h2>
      <ServersList />
    </main>
  );
}


