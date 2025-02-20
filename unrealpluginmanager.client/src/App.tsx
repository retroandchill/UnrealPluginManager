import {useEffect, useState} from 'react';
import './App.css';
import {Configuration, PluginOverview, PluginsApi} from './api';
import {Page} from './util'
import {pluginButton} from "./components";

function App() {
    const apiConfig = new Configuration({
        basePath: "https://localhost:60493"
    });
    let pluginsApi = new PluginsApi(apiConfig);
    const [plugins, setPlugins] = useState<Page<PluginOverview>>();

    useEffect(() => {
        populatePluginList();
    }, []);

    const contents = plugins === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a
            href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em>
        </p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <tbody>
            {plugins.items.map(plugin => pluginButton(plugin, (_) => {}) )}
            </tbody>
        </table>;

    return (
        <div>
        <h1 id="tableLabel">Plugins List</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );

    async function populatePluginList() {
        const response = await pluginsApi.getPlugins()
        setPlugins(response)
    }
}

export default App;