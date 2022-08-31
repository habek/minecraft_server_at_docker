import { Link } from "react-router-dom";
import { Layout } from "../components/Layout";
import { useAllServerNames } from "../hooks/gameServersHooks";
import { routeToServer } from "./routesList";


export function ServersList() {
  const allServerNames = useAllServerNames()
  return (
    <div>
      {
        allServerNames?.map(serverName => (
          <div key={"menu" + serverName}>
            <Link to={routeToServer(serverName)}>{serverName}</Link><br></br>
          </div>
        ))
      }
    </div>
  )
};

export default function ServersIndex() {
  return (
    <Layout>
      <main style={{ padding: "1rem 0" }}>
        <h2>Hosted servers</h2>
        <ServersList />
      </main>
    </Layout>
  );
}


