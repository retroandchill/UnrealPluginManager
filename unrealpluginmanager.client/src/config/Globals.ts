import {Configuration, PluginsApi, StorageApi} from "../api";

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
 *
 * @type {PluginsApi}
 * @param {Object} apiConfig - The configuration object used to initialize the PluginsApi instance.
 */
export const pluginsApi = new PluginsApi(apiConfig);
/**
 * Represents an instance of the StorageApi class used to interact with storage functionalities.
 *
 * This variable serves as an interface for managing storage-related operations using the provided API configuration.
 *
 * @type {StorageApi}
 * @param {Object} apiConfig - The configuration object for the StorageApi instance, including required credentials, endpoints, and additional options.
 */
export const storageApi = new StorageApi(apiConfig);

/**
 * Defines the file path for accessing the icons.
 * This variable constructs the complete URL path for icons by appending
 * '/files/icons' to the base URL provided by the apiBaseUrl variable.
 *
 * @type {string}
 */
export const iconsPath = `${apiBaseUrl}/files/icons`