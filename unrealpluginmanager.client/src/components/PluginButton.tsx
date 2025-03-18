import {PluginVersionInfo} from "@/api";
import {apiResourcesPath} from "@/config/Globals.ts";
import {PluginCallback} from "@/util";

/**
 * Represents the properties for a plugin button component.
 *
 * This interface defines the structure for the props required by a button
 * that interacts with a plugin functionality.
 */
interface PluginButtonProps {
  /**
   * Represents the main plugin instance which encapsulates a specific functionality or feature.
   * This variable is a reference to the PluginOverview instance that handles initialization,
   * configuration, and the execution of associated plugin tasks.
   *
   * Typically, the `plugin` variable provides methods and properties for managing
   * the lifecycle of the plugin, such as loading, enabling, disabling, or interacting
   * with its functionality.
   */
  plugin: PluginVersionInfo;

  /**
   * Callback function to handle click events.
   *
   * The `onClick` variable is expected to store a reference to a function
   * that is invoked when a corresponding user interaction occurs, typically
   * a mouse or touch click event. It is used to define custom behavior
   * in response to the event.
   *
   * This function is commonly passed as a parameter or directly set as
   * a property on a component or widget that supports event handling.
   *
   * Type: `PluginCallback`.
   */
  onClick?: PluginCallback;
}

/**
 * Renders a button component for a plugin.
 * Displays plugin details such as name, latest release, and author, and triggers a click handler when clicked.
 *
 * @param {PluginButtonProps} props - The properties for the PluginButton component.
 * @param {Object} props.plugin - The plugin object containing information about the plugin.
 * @param {string} props.plugin.name - The name of the plugin.
 * @param {Array<Object>} props.plugin.versions - The array of versions for the plugin.
 * @param {string} props.plugin.authorName - The name of the plugin author.
 * @param {Function} [props.onClick] - Optional onClick handler to be executed when the button is clicked.
 * @return {JSX.Element} A button element displaying plugin details.
 */
function PluginButton(props: Readonly<PluginButtonProps>) {
  return <button onClick={() => {
    if (props.onClick) {
      props.onClick(props.plugin);
    }
  }}>
      <img src={props.plugin.icon ? `${apiResourcesPath}/${props.plugin.icon.storedFilename}` : "Icon128.png"}
           alt="Plugin Icon"/>
    <header style={{textAlign: 'left', padding: 0, margin: 0,}}>
      <h2>{props.plugin.name}</h2>
    </header>
    <ul style={{listStyleType: 'none', padding: 0, margin: 0, textAlign: 'left'}}>
        <li><b>Latest Release:</b> {props.plugin.version}</li>
      <li><b>Author:</b> {props.plugin.authorName ? `${props.plugin.authorName}` : 'Unknown'}</li>
    </ul>
  </button>
}

export default PluginButton;