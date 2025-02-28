import {Component} from 'react';
import './App.css';
import {PluginDisplayGrid} from "./components";

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 *
 * @extends Component
 * @template {}, AppState
 */
class App extends Component {
    
    render() {
        return <div>
            <PluginDisplayGrid/>
        </div>;
    }
    
}

export default App;