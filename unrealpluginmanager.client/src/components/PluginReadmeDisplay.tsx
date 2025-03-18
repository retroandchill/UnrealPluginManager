import { pluginsApi } from "@/config";
import {useEffect, useState } from "react";
import Markdown from "react-markdown";

/**
 * Represents properties required to display the README of a specific plugin version.
 *
 * This interface is used to pass identifiers necessary for fetching and displaying
 * the README file associated with a particular plugin and its version.
 *
 * Properties:
 * - `pluginId`: A string representing the unique identifier of the plugin.
 * - `versionId`: A string representing the unique identifier of the specific version of the plugin.
 */
interface PluginReadmeDisplayProps {
  /**
   * Represents the unique identifier for a plugin.
   * This value is typically used to load, manage, or reference a specific plugin within an application or system.
   * It should be a string that uniquely corresponds to a plugin within the defined context.
   */
  pluginId: string;
  
  /**
   * Represents the unique identifier for a specific version of an entity or object.
   * This identifier is typically used for version tracking and comparison purposes.
   */
  versionId: string;
}

/**
 * Displays the README content of a plugin in Markdown format.
 *
 * @param props - The properties object.
 * @param props.pluginId - The ID of the plugin for which the README is being fetched.
 * @param props.versionId - The specific version ID of the plugin for which the README is being fetched.
 * @return A React component rendering the plugin's README in Markdown format or a loading message if the content is not yet available.
 */
function PluginReadmeDisplay({pluginId, versionId} : Readonly<PluginReadmeDisplayProps>) {
  const [readme, setReadme] = useState<string>()

  useEffect(() => {
    pluginsApi.getPluginReadme({pluginId: pluginId, versionId: versionId})
        .then(r => setReadme(r))
  });
  
  return (
      <Markdown>{readme ?? "Loading..."}</Markdown>
  );
}

export default PluginReadmeDisplay;