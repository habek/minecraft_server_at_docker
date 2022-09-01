import { Layout } from "../components/Layout";
import { ServerUsers } from "../components/ServerUsers";

export default function Users() {
  return (
    <Layout>
      <h4>Users</h4>
      <ServerUsers />
    </Layout>
  );
}