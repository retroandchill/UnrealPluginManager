import {PluginVersionInfo} from "@/api";
import {exeName} from "@/config";

/**
 * A callback function type that is executed with a plugin as its argument.
 *
 * This type is used for defining functions that operate on or process a `PluginOverview` instance.
 *
 * @callback PluginCallback
 * @param plugin - The plugin instance passed to the callback function.
 */
export type PluginCallback = (plugin: PluginVersionInfo) => void;

/**
 * Constructs a command string to install a specified plugin, optionally including its version.
 *
 * @param plugin - The object containing plugin name and version information.
 * @param [includeVersion=false] - A flag indicating whether to include the version in the install command.
 * @return The constructed install command string.
 */
export function getInstallCommand(plugin: PluginVersionInfo, includeVersion: boolean = false) {
  return `${exeName} install ${includeVersion ? plugin.name + " --version " + plugin.version : plugin.name}`
}