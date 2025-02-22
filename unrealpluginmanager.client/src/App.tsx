import {Component} from 'react';
import './App.css';
import {PluginOverview} from './api';
import {Page} from './util'
import PluginButton from "./components";
import {pluginsApi} from "./config/Globals.ts";

/**
 * AppState interface represents the state of the application.
 */
interface AppState {
    /**
     * Represents an optional variable that holds a paginated collection of plugin overviews.
     * The `plugins` variable may contain multiple plugins with their associated details, structured
     * as a pageable object.
     *
     * @type {Page<PluginOverview>}
     */
    plugins?: Page<PluginOverview>;
}

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 *
 * @extends Component
 * @template {}, AppState
 */
class App extends Component<{}, AppState> {
    
    /**
     * Constructor for initializing the component with props and setting the initial state.
     *
     * @param {Object} props - The properties passed to the component.
     * @return {void}
     */
    constructor(props: {}) {
        super(props);
        this.state = {};
    }
    
    componentDidMount() {
        this.populatePluginList();
    }
    
    render() {
        const contents = this.state.plugins === undefined
            ? <p><em>Loading...</em></p>
            : <table className="table table-striped" aria-labelledby="tableLabel">
                <tbody>
                {this.state.plugins.items.map(plugin => <PluginButton key={plugin.id} plugin={plugin} onClick={(_) => {}}/>)}
                </tbody>
            </table>;

        return (
            <div>
                <h1 id="tableLabel">Plugins List</h1>
                {contents}
            </div>
        );
    }
    
    private async populatePluginList() {
        const response = await pluginsApi.getPlugins()
        this.setState({plugins: response})
    }
}

export default App;