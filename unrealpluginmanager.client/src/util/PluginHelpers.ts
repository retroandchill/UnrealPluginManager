import {PluginVersionInfo} from "@/api";
import {exeName} from "@/config";

/**
 * A callback function type that is executed with a plugin as its argument.
 *
 * This type is used for defining functions that operate on or process a `PluginOverview` instance.
 *
 * @callback PluginCallback
 * @param {PluginVersionInfo} plugin - The plugin instance passed to the callback function.
 */
export type PluginCallback = (plugin: PluginVersionInfo) => void;

export function getInstallCommand(plugin: PluginVersionInfo, includeVersion: boolean = false) {
  return `${exeName} install ${includeVersion ? plugin.name : plugin.name + " --version " + plugin.version}`
}