import { Layout } from "../components/Layout";
import { ServerBackupsList } from "../components/ServerBackupsList";

export default function Backups() {
  return (
    <Layout>
      <h4>Backups</h4>
      <ServerBackupsList />
    </Layout>
  );
}