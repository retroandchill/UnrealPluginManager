/* tslint:disable */
/* eslint-disable */
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
import type { PluginType } from './PluginType';
import {
    PluginTypeFromJSON,
    PluginTypeFromJSONTyped,
    PluginTypeToJSON,
    PluginTypeToJSONTyped,
} from './PluginType';

/**
 * Represents an overview of a plugin dependency, including its metadata and type.
 * @export
 * @interface DependencyOverview
 */
export interface DependencyOverview {
    /**
     * Gets or sets the unique identifier associated with a dependency.
     * This identifier is automatically generated and used to distinguish
     * dependencies within the system.
     * @type {number}
     * @memberof DependencyOverview
     */
    id: number;
    /**
     * Gets or sets the name of the plugin associated with the dependency.
     * The plugin name is a required field and must adhere to the predefined naming rules.
     * @type {string}
     * @memberof DependencyOverview
     */
    pluginName: string | null;
    /**
     * Gets or sets the version range associated with the plugin dependency.
     * This version range defines the compatible versions of the plugin required by the dependency.
     * @type {string}
     * @memberof DependencyOverview
     */
    pluginVersion?: string | null;
    /**
     * Gets or sets a value indicating whether the dependency is optional.
     * When set to true, the dependency is not mandatory, and the system
     * can function without it.
     * @type {boolean}
     * @memberof DependencyOverview
     */
    optional?: boolean;
    /**
     * 
     * @type {PluginType}
     * @memberof DependencyOverview
     */
    type?: PluginType;
}



/**
 * Check if a given object implements the DependencyOverview interface.
 */
export function instanceOfDependencyOverview(value: object): value is DependencyOverview {
    if (!('id' in value) || value['id'] === undefined) return false;
    if (!('pluginName' in value) || value['pluginName'] === undefined) return false;
    return true;
}

export function DependencyOverviewFromJSON(json: any): DependencyOverview {
    return DependencyOverviewFromJSONTyped(json, false);
}

export function DependencyOverviewFromJSONTyped(json: any, ignoreDiscriminator: boolean): DependencyOverview {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'],
        'pluginName': json['pluginName'],
        'pluginVersion': json['pluginVersion'] == null ? undefined : json['pluginVersion'],
        'optional': json['optional'] == null ? undefined : json['optional'],
        'type': json['type'] == null ? undefined : PluginTypeFromJSON(json['type']),
    };
}

export function DependencyOverviewToJSON(json: any): DependencyOverview {
    return DependencyOverviewToJSONTyped(json, false);
}

export function DependencyOverviewToJSONTyped(value?: DependencyOverview | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'pluginName': value['pluginName'],
        'pluginVersion': value['pluginVersion'],
        'optional': value['optional'],
        'type': PluginTypeToJSON(value['type']),
    };
}

