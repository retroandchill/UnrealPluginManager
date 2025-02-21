import {Configuration, PluginsApi} from "../api";

const apiConfig = new Configuration({
    basePath: "https://localhost:60493"
});
export const pluginsApi = new PluginsApi(apiConfig);