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
 * Provides a base class for representing essential metadata of a plugin.
 * This includes core properties such as the plugin's unique identifier, name,
 * optional friendly name, description, author's name, and website.
 * @export
 * @interface PluginOverviewBase
 */
export interface PluginOverviewBase {
    /**
     * Gets or sets the unique identifier for the plugin.
     * @type {string}
     * @memberof PluginOverviewBase
     */
    id: string;
    /**
     * Gets or sets the name of the plugin.
     * @type {string}
     * @memberof PluginOverviewBase
     */
    name: string;
    /**
     * Gets or sets the user-friendly name of the plugin, which may be used for display purposes.
     * @type {string}
     * @memberof PluginOverviewBase
     */
    friendlyName?: string | null;
    /**
     * Gets or sets the description of the plugin, providing detailed information about its purpose or functionality.
     * @type {string}
     * @memberof PluginOverviewBase
     */
    description?: string | null;
    /**
     * Gets or sets the name of the author associated with the plugin.
     * @type {string}
     * @memberof PluginOverviewBase
     */
    authorName?: string | null;
    /**
     * Gets or sets the URL associated with the author of the plugin.
     * @type {string}
     * @memberof PluginOverviewBase
     */
    authorWebsite?: string | null;
}

/**
 * Check if a given object implements the PluginOverviewBase interface.
 */
export function instanceOfPluginOverviewBase(value: object): value is PluginOverviewBase {
    if (!('id' in value) || value['id'] === undefined) return false;
    if (!('name' in value) || value['name'] === undefined) return false;
    return true;
}

export function PluginOverviewBaseFromJSON(json: any): PluginOverviewBase {
    return PluginOverviewBaseFromJSONTyped(json, false);
}

export function PluginOverviewBaseFromJSONTyped(json: any, ignoreDiscriminator: boolean): PluginOverviewBase {
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
    };
}

export function PluginOverviewBaseToJSON(json: any): PluginOverviewBase {
    return PluginOverviewBaseToJSONTyped(json, false);
}

export function PluginOverviewBaseToJSONTyped(value?: PluginOverviewBase | null, ignoreDiscriminator: boolean = false): any {
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
    };
}

