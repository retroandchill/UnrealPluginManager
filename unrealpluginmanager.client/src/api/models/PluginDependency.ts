/* tslint:disable */
/* eslint-disable */
//@ts-nocheck
/**
 * Unreal Plugin Manager API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

/**
 * Represents a dependency of a plugin within the Unreal Plugin Manager framework.
 * @export
 * @interface PluginDependency
 */
export interface PluginDependency {
    /**
     * Gets the name of the plugin.
     * @type {string}
     * @memberof PluginDependency
     */
    pluginName: string;
    /**
     * Gets the version range of the dependent plugin.
     * @type {string}
     * @memberof PluginDependency
     */
    pluginVersion: string;
}

/**
 * Check if a given object implements the PluginDependency interface.
 */
export function instanceOfPluginDependency(value: object): value is PluginDependency {
    if (!('pluginName' in value) || value['pluginName'] === undefined) return false;
    if (!('pluginVersion' in value) || value['pluginVersion'] === undefined) return false;
    return true;
}

export function PluginDependencyFromJSON(json: any): PluginDependency {
    return PluginDependencyFromJSONTyped(json, false);
}

export function PluginDependencyFromJSONTyped(json: any, ignoreDiscriminator: boolean): PluginDependency {
    if (json == null) {
        return json;
    }
    return {
        
        'pluginName': json['pluginName'],
        'pluginVersion': json['pluginVersion'],
    };
}

export function PluginDependencyToJSON(json: any): PluginDependency {
    return PluginDependencyToJSONTyped(json, false);
}

export function PluginDependencyToJSONTyped(value?: PluginDependency | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'pluginName': value['pluginName'],
        'pluginVersion': value['pluginVersion'],
    };
}

