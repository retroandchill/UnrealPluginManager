import {PluginOverview} from "../api";
import { Component } from 'react';
import Icon128 from "../assets/icon128.png";
import {iconsPath} from "../config/Globals.ts";

/**
 * A callback function type that is executed with a plugin as its argument.
 *
 * This type is used for defining functions that operate on or process a `PluginOverview` instance.
 *
 * @callback PluginCallback
 * @param {PluginOverview} plugin - The plugin instance passed to the callback function.
 */
export type PluginCallback = (plugin: PluginOverview) => void;

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
    plugin: PluginOverview;
    
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

interface PluginButtonState {
    icon: string;
}

/**
 * A React component that represents a button for a plugin. When clicked, it triggers an action
 * defined by the onClick handler passed in the props. It displays basic plugin details such as
 * the latest version, author name (if available), and description.
 *
 * @extends Component
 *
 * @template PluginButtonProps, PluginButtonState
 */
export class PluginButton extends Component<PluginButtonProps, PluginButtonState> {
    
    /**
     * Constructs a new instance of the PluginButton.
     *
     * @param {PluginButtonProps} props - The properties used to initialize the PluginButton.=
     */
    constructor(props: PluginButtonProps) {
        super(props)
        this.state = { icon: Icon128 }
    }

    componentDidMount() {
        if (this.props.plugin.icon != undefined) {
            this.setState({icon: `${iconsPath}/${this.props.plugin.icon}`})
        } else {
            this.setState({icon: Icon128})
        }
    }
    
    render() {
        let latestVersion = this.props.plugin.versions[this.props.plugin.versions.length - 1];

        return <button onClick={() => { if (this.props.onClick) { this.props.onClick(this.props.plugin); } }}>
            <img  src={this.state.icon} alt="Plugin Icon"/>
            <header style={{ textAlign: 'left', padding: 0, margin: 0, }}>
                <h2>{this.props.plugin.name}</h2>
            </header>
            <ul style={{ listStyleType: 'none', padding: 0, margin: 0, textAlign: 'left' }}>
                <li><b>Latest Release:</b> {latestVersion.version}</li>
                <li><b>Author:</b> {this.props.plugin.authorName ? `${this.props.plugin.authorName}` : 'Unknown'}</li>
            </ul>
        </button>
    }
    
}