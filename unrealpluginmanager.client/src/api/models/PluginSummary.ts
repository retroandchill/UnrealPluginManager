/* tslint:disable */
/* eslint-disable */
//@ts-nocheck
/**
 * UnrealPluginManager.Server
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { mapValues } from '../runtime';
/**
 * Represents a summary of a plugin, including its name, version, and optional description.
 * @export
 * @interface PluginSummary
 */
export interface PluginSummary {
    /**
     * Gets the unique identifier of the plugin.
     * This property is required and serves as the primary identifier for the plugin within the system.
     * @type {number}
     * @memberof PluginSummary
     */
    pluginId: number;
    /**
     * Gets the name of the plugin.
     * This property is required and uniquely identifies the plugin within the context of the plugin system.
     * @type {string}
     * @memberof PluginSummary
     */
    name: string;
    /**
     * Gets or sets an optional user-friendly name for the plugin.
     * This property provides a more descriptive or colloquial name that can be displayed in user interfaces
     * and used as an alternative to the plugin's primary name when needed.
     * @type {string}
     * @memberof PluginSummary
     */
    friendlyName: string | null;
    /**
     * Gets the unique identifier of the plugin version.
     * This property ensures that each version of a plugin is distinctly identifiable within the system.
     * @type {number}
     * @memberof PluginSummary
     */
    versionId: number;
    /**
     * Gets the semantic version of the plugin.
     * This property adheres to the semantic versioning format and is used to specify the version details of the plugin.
     * @type {string}
     * @memberof PluginSummary
     */
    version: string;
}

/**
 * Check if a given object implements the PluginSummary interface.
 */
export function instanceOfPluginSummary(value: object): value is PluginSummary {
    if (!('pluginId' in value) || value['pluginId'] === undefined) return false;
    if (!('name' in value) || value['name'] === undefined) return false;
    if (!('friendlyName' in value) || value['friendlyName'] === undefined) return false;
    if (!('versionId' in value) || value['versionId'] === undefined) return false;
    if (!('version' in value) || value['version'] === undefined) return false;
    return true;
}

export function PluginSummaryFromJSON(json: any): PluginSummary {
    return PluginSummaryFromJSONTyped(json, false);
}

export function PluginSummaryFromJSONTyped(json: any, ignoreDiscriminator: boolean): PluginSummary {
    if (json == null) {
        return json;
    }
    return {
        
        'pluginId': json['pluginId'],
        'name': json['name'],
        'friendlyName': json['friendlyName'],
        'versionId': json['versionId'],
        'version': json['version'],
    };
}

export function PluginSummaryToJSON(json: any): PluginSummary {
    return PluginSummaryToJSONTyped(json, false);
}

export function PluginSummaryToJSONTyped(value?: PluginSummary | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'pluginId': value['pluginId'],
        'name': value['name'],
        'friendlyName': value['friendlyName'],
        'versionId': value['versionId'],
        'version': value['version'],
    };
}

