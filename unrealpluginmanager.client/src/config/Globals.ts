import {Configuration, PluginsApi} from "@/api";
import {UserManager, WebStorageStateStore} from "oidc-client-ts";

/**
 * The base URL of the API used for making HTTP requests.
 * This variable typically contains the protocol, hostname, and port number (if specified),
 * serving as the starting point for constructing API endpoint URLs.
 *
 * Example format: "https://localhost:60493"
 */
export const apiBaseUrl = window.location.origin;

const apiConfig = new Configuration({
  basePath: `${apiBaseUrl}/api`
});

/**
 * Represents an instance of the PluginsApi class.
 *
 * The `pluginsApi` variable is used for interactions with the plugins API
 * and provides methods to manage or retrieve plugin-related information.
 */
export const pluginsApi = new PluginsApi(apiConfig);

export const userManager = new UserManager({
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  redirect_uri: `${window.location.origin}${window.location.pathname}`,
  post_logout_redirect_uri: window.location.origin,
  userStore: new WebStorageStateStore({store: window.sessionStorage}),
  monitorSession: true
});

export function onSigninCallback() {
  window.history.replaceState({}, document.title, window.location.pathname);
}

/**
 * Represents the complete API path to access resources.
 * It is a concatenation of the base API URL and the resources endpoint.
 */
export const apiResourcesPath = `${apiBaseUrl}/storage`

export const exeName = "uepm";