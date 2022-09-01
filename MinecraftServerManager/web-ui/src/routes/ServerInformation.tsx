import { useEffect, useState } from "react";
import { Input } from "reactstrap";
import { Layout } from "../components/Layout";
import { useCurrentServerId } from "../hooks/gameServersHooks";
import ServerClient from "../libs/ServerClient";

export default function ServerInformation() {
  const serverId = useCurrentServerId();
  const [serverPropertiesContent, setServerPropertiesContent] = useState<string | undefined>(undefined);
  useEffect(() => {
    console.info("ServerId:" + serverId)
    if (!!serverId) {
      ServerClient.LoadFile(serverId ?? "", "server.properties").then((content) => { setServerPropertiesContent(content) });
    }
  }, [serverId])

  return (
    <Layout>
      <div>
        <h4>Server information</h4>
        <label>server.properties</label>
        <div className=".text-monospace">
          <Input type="textarea" rows={20} className=".monospace" value={serverPropertiesContent}></Input>
        </div>
      </div>
    </Layout>
  );
}