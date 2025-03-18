import {useEffect, useState} from 'react';
import Markdown from 'react-markdown'
import {PluginVersionInfo} from "@/api";
import {pluginsApi} from "@/config";

interface PluginPageProps {
  plugin: PluginVersionInfo
}

function PluginPage({ plugin } : Readonly<PluginPageProps>) {
  const [readme, setReadme] = useState("");
  
  useEffect(() => {
    pluginsApi.getPluginReadme({pluginId: plugin.pluginId, versionId: plugin.versionId})
        .then((text) => setReadme(text))
  });
  
  return (
      <Markdown>{readme}</Markdown>
  );
}

export default PluginPage;