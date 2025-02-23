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


import * as runtime from '../runtime';

export interface GetIconRequest {
    fileName: string;
}

/**
 * 
 */
export class StorageApi extends runtime.BaseAPI {

    /**
     * Retrieves an icon as a stream for the specified file name.
     */
    async getIconRaw(requestParameters: GetIconRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<Blob>> {
        if (requestParameters['fileName'] == null) {
            throw new runtime.RequiredError(
                'fileName',
                'Required parameter "fileName" was null or undefined when calling getIcon().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/files/icons/{fileName}`.replace(`{${"fileName"}}`, encodeURIComponent(String(requestParameters['fileName']))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.BlobApiResponse(response);
    }

    /**
     * Retrieves an icon as a stream for the specified file name.
     */
    async getIcon(requestParameters: GetIconRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<Blob> {
        const response = await this.getIconRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
