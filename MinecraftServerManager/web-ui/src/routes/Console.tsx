import { Link, useParams } from "react-router-dom";
import { Col, Container, Row } from "reactstrap";
import { Layout } from "../components/Layout";
import { ServerLogs } from "../components/ServerLogs";
import { routeServersIndex } from "./routesList";

export default function Console() {
    const params = useParams()
    const serverId = params.serverId
    if (!serverId) {
        return <div><h1>Unknown server: "{params.serverId}"</h1><Link to={routeServersIndex}>Go to servers list</Link> </div>
    }
    return (
        <Layout>
            <h4>Server logs</h4>
            <ServerLogs />
        </Layout>
    );
}