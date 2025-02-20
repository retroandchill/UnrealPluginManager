import {PluginOverview} from "../api";
import {JSX} from 'react';

/**
 * A callback function type that is executed with a plugin as its argument.
 *
 * This type is used for defining functions that operate on or process a `PluginOverview` instance.
 *
 * @callback PluginCallback
 * @param {PluginOverview} plugin - The plugin instance passed to the callback function.
 */
type PluginCallback = (plugin: PluginOverview) => void;

/**
 * Renders a button element associated with a plugin, displaying details such as the plugin name, the latest version, author, and description.
 *
 * @param {PluginOverview} plugin - An object containing the details about the plugin, including its name, versions, author, and description.
 * @param {PluginCallback} onClick - A callback function that is triggered when the button is clicked, taking the plugin as its argument.
 * @return {JSX.Element} A button element with the plugin's details and a click event handler.
 */
export function pluginButton(plugin: PluginOverview, onClick: PluginCallback): JSX.Element {
    let latestVersion = plugin.versions[plugin.versions.length - 1];
    
    return <button onClick={() => onClick(plugin)}>
        <header style={{ textAlign: 'left', padding: 0, margin: 0, }}>
            <h2>{plugin.name}</h2>
        </header>
        {/* TODO: We need to add the icons to the server and fetch them and add them to an image here */}
        <ul style={{ listStyleType: 'none', padding: 0, margin: 0, textAlign: 'left' }}>
            <li>Latest Release: {latestVersion.version}</li>
            <li>{plugin.authorName ? ` Author: ${plugin.authorName}` : ''}</li>
            <li>{plugin.description}</li>
        </ul>
    </button>
}