import {Configuration, PluginsApi} from "@/api";

/**
 * The base URL of the API used for making HTTP requests.
 * This variable typically contains the protocol, hostname, and port number (if specified),
 * serving as the starting point for constructing API endpoint URLs.
 *
 * Example format: "https://localhost:60493"
 */
export const apiBaseUrl = window.location.origin;

const apiConfig = new Configuration({
  basePath: apiBaseUrl
});

/**
 * Represents an instance of the PluginsApi class.
 *
 * The `pluginsApi` variable is used for interactions with the plugins API
 * and provides methods to manage or retrieve plugin-related information.
 */
export const pluginsApi = new PluginsApi(apiConfig);

/**
 * Represents the complete API path to access resources.
 * It is a concatenation of the base API URL and the resources endpoint.
 */
export const apiResourcesPath = `${apiBaseUrl}/storage`

export const exeName = "uepm";