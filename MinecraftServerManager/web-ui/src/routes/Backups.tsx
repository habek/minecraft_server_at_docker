import { Layout } from "../components/Layout";
import { ServerBackupsList } from "../components/ServerBackupsList";

export default function Backups() {
  return (
    <Layout>
      <main style={{ padding: "1rem 0" }}>
        <h2>Backups</h2>
        <ServerBackupsList/>
      </main>
    </Layout>
  );
}