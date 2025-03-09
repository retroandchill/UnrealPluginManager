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
import type { VersionOverview } from './VersionOverview';
import {
    VersionOverviewFromJSON,
    VersionOverviewFromJSONTyped,
    VersionOverviewToJSON,
    VersionOverviewToJSONTyped,
} from './VersionOverview';

/**
 * Represents an overview of a plugin, providing basic information such as its ID, name,
 * optional friendly name, description, and associated versions.
 * @export
 * @interface PluginOverview
 */
export interface PluginOverview {
    /**
     * Gets or sets the unique identifier for the plugin.
     * @type {number}
     * @memberof PluginOverview
     */
    id: number;
    /**
     * Gets or sets the name of the plugin.
     * @type {string}
     * @memberof PluginOverview
     */
    name: string;
    /**
     * Gets or sets the user-friendly name of the plugin, which may be used for display purposes.
     * @type {string}
     * @memberof PluginOverview
     */
    friendlyName?: string | null;
    /**
     * Gets or sets the description of the plugin, providing detailed information about its purpose or functionality.
     * @type {string}
     * @memberof PluginOverview
     */
    description?: string | null;
    /**
     * Gets or sets the name of the author associated with the plugin.
     * @type {string}
     * @memberof PluginOverview
     */
    authorName?: string | null;
    /**
     * Gets or sets the URL associated with the author of the plugin.
     * @type {string}
     * @memberof PluginOverview
     */
    authorWebsite?: string | null;
    /**
     * Gets or sets the collection of versions associated with the plugin.
     * Each version provides a detailed overview including its version number
     * and unique identifier.
     * @type {Array<VersionOverview>}
     * @memberof PluginOverview
     */
    versions: Array<VersionOverview>;
}

/**
 * Check if a given object implements the PluginOverview interface.
 */
export function instanceOfPluginOverview(value: object): value is PluginOverview {
    if (!('id' in value) || value['id'] === undefined) return false;
    if (!('name' in value) || value['name'] === undefined) return false;
    if (!('versions' in value) || value['versions'] === undefined) return false;
    return true;
}

export function PluginOverviewFromJSON(json: any): PluginOverview {
    return PluginOverviewFromJSONTyped(json, false);
}

export function PluginOverviewFromJSONTyped(json: any, ignoreDiscriminator: boolean): PluginOverview {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'],
        'name': json['name'],
        'friendlyName': json['friendlyName'] == null ? undefined : json['friendlyName'],
        'description': json['description'] == null ? undefined : json['description'],
        'authorName': json['authorName'] == null ? undefined : json['authorName'],
        'authorWebsite': json['authorWebsite'] == null ? undefined : json['authorWebsite'],
        'versions': ((json['versions'] as Array<any>).map(VersionOverviewFromJSON)),
    };
}

export function PluginOverviewToJSON(json: any): PluginOverview {
    return PluginOverviewToJSONTyped(json, false);
}

export function PluginOverviewToJSONTyped(value?: PluginOverview | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'name': value['name'],
        'friendlyName': value['friendlyName'],
        'description': value['description'],
        'authorName': value['authorName'],
        'authorWebsite': value['authorWebsite'],
        'versions': ((value['versions'] as Array<any>).map(VersionOverviewToJSON)),
    };
}

