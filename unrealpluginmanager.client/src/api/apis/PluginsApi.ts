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


import * as runtime from '../runtime';
import type {
  DependencyManifest,
  PluginDependency,
  PluginOverviewPage,
  PluginSummary,
  PluginVersionInfo,
  PluginVersionInfoPage,
  SourcePatchInfo,
} from '../models/index';
import {
  DependencyManifestFromJSON,
  PluginDependencyToJSON,
  PluginOverviewPageFromJSON,
  PluginSummaryFromJSON,
  PluginVersionInfoFromJSON,
  PluginVersionInfoPageFromJSON,
  SourcePatchInfoFromJSON,
} from '../models/index';

export interface AddPluginReadmeRequest {
    pluginId: string;
    versionId: string;
    body?: string;
}

export interface GetCandidateDependenciesRequest {
    pluginDependency: Array<PluginDependency>;
}

export interface GetDependencyTreeRequest {
    pluginId: string;
    body?: string;
}

export interface GetLatestVersionRequest {
    pluginId: string;
    version?: string;
}

export interface GetLatestVersionsRequest {
    match?: string;
    versionRange?: string;
    page?: number;
    size?: number;
}

export interface GetPluginPatchesRequest {
  pluginId: string;
  versionId: string;
}

export interface GetPluginReadmeRequest {
    pluginId: string;
    versionId: string;
}

export interface GetPluginsRequest {
    match?: string;
    page?: number;
    size?: number;
}

export interface SubmitPluginRequest {
  archive?: Blob;
}

export interface UpdatePluginReadmeRequest {
    pluginId: string;
    versionId: string;
    body?: string;
}

/**
 * 
 */
export class PluginsApi extends runtime.BaseAPI {

    /**
     * Adds or updates the README content for the specified plugin version.
     */
    async addPluginReadmeRaw(requestParameters: AddPluginReadmeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        if (requestParameters['pluginId'] == null) {
            throw new runtime.RequiredError(
                'pluginId',
                'Required parameter "pluginId" was null or undefined when calling addPluginReadme().'
            );
        }

        if (requestParameters['versionId'] == null) {
            throw new runtime.RequiredError(
                'versionId',
                'Required parameter "versionId" was null or undefined when calling addPluginReadme().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'text/markdown';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["X-API-Key"] = await this.configuration.apiKey("X-API-Key"); // apiKey authentication
        }

        if (this.configuration && this.configuration.accessToken) {
            // oauth required
            headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", ["CanEditPlugin"]);
        }

        const response = await this.request({
            path: `/plugins/{pluginId}/{versionId}/readme`.replace(`{${"pluginId"}}`, encodeURIComponent(String(requestParameters['pluginId']))).replace(`{${"versionId"}}`, encodeURIComponent(String(requestParameters['versionId']))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters['body'] as any,
        }, initOverrides);

        if (this.isJsonMime(response.headers.get('content-type'))) {
            return new runtime.JSONApiResponse<string>(response);
        } else {
            return new runtime.TextApiResponse(response) as any;
        }
    }

    /**
     * Adds or updates the README content for the specified plugin version.
     */
    async addPluginReadme(requestParameters: AddPluginReadmeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.addPluginReadmeRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Retrieves a dependency manifest containing potential versions for the given list of plugin dependencies.
     */
    async getCandidateDependenciesRaw(requestParameters: GetCandidateDependenciesRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<DependencyManifest>> {
        if (requestParameters['pluginDependency'] == null) {
            throw new runtime.RequiredError(
                'pluginDependency',
                'Required parameter "pluginDependency" was null or undefined when calling getCandidateDependencies().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

      if (this.configuration && this.configuration.accessToken) {
        // oauth required
        headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
      }

        const response = await this.request({
            path: `/plugins/dependencies/candidates`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters['pluginDependency']!.map(PluginDependencyToJSON),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => DependencyManifestFromJSON(jsonValue));
    }

    /**
     * Retrieves a dependency manifest containing potential versions for the given list of plugin dependencies.
     */
    async getCandidateDependencies(requestParameters: GetCandidateDependenciesRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<DependencyManifest> {
        const response = await this.getCandidateDependenciesRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Retrieves the dependency tree for a specified plugin.
     */
    async getDependencyTreeRaw(requestParameters: GetDependencyTreeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<Array<PluginSummary>>> {
        if (requestParameters['pluginId'] == null) {
            throw new runtime.RequiredError(
                'pluginId',
                'Required parameter "pluginId" was null or undefined when calling getDependencyTree().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

      if (this.configuration && this.configuration.accessToken) {
        // oauth required
        headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
      }

        const response = await this.request({
            path: `/plugins/{pluginId}/latest/dependencies`.replace(`{${"pluginId"}}`, encodeURIComponent(String(requestParameters['pluginId']))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters['body'] as any,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(PluginSummaryFromJSON));
    }

    /**
     * Retrieves the dependency tree for a specified plugin.
     */
    async getDependencyTree(requestParameters: GetDependencyTreeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<Array<PluginSummary>> {
        const response = await this.getDependencyTreeRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Retrieves detailed information about the latest version of the specified plugin,  optionally constrained by a version range.
     */
    async getLatestVersionRaw(requestParameters: GetLatestVersionRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<PluginVersionInfo>> {
        if (requestParameters['pluginId'] == null) {
            throw new runtime.RequiredError(
                'pluginId',
                'Required parameter "pluginId" was null or undefined when calling getLatestVersion().'
            );
        }

        const queryParameters: any = {};

        if (requestParameters['version'] != null) {
            queryParameters['version'] = requestParameters['version'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

      if (this.configuration && this.configuration.accessToken) {
        // oauth required
        headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
      }

        const response = await this.request({
            path: `/plugins/{pluginId}/latest`.replace(`{${"pluginId"}}`, encodeURIComponent(String(requestParameters['pluginId']))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => PluginVersionInfoFromJSON(jsonValue));
    }

    /**
     * Retrieves detailed information about the latest version of the specified plugin,  optionally constrained by a version range.
     */
    async getLatestVersion(requestParameters: GetLatestVersionRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<PluginVersionInfo> {
        const response = await this.getLatestVersionRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Retrieves a paginated list of the latest plugin versions filtered by the specified criteria.
     */
    async getLatestVersionsRaw(requestParameters: GetLatestVersionsRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<PluginVersionInfoPage>> {
        const queryParameters: any = {};

        if (requestParameters['match'] != null) {
            queryParameters['match'] = requestParameters['match'];
        }

        if (requestParameters['versionRange'] != null) {
            queryParameters['versionRange'] = requestParameters['versionRange'];
        }

        if (requestParameters['page'] != null) {
            queryParameters['page'] = requestParameters['page'];
        }

        if (requestParameters['size'] != null) {
            queryParameters['size'] = requestParameters['size'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

      if (this.configuration && this.configuration.accessToken) {
        // oauth required
        headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
      }

        const response = await this.request({
            path: `/plugins/latest`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => PluginVersionInfoPageFromJSON(jsonValue));
    }

    /**
     * Retrieves a paginated list of the latest plugin versions filtered by the specified criteria.
     */
    async getLatestVersions(requestParameters: GetLatestVersionsRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<PluginVersionInfoPage> {
        const response = await this.getLatestVersionsRaw(requestParameters, initOverrides);
        return await response.value();
    }

  /**
   * Retrieves a list of source patch information for the specified plugin version.
   */
  async getPluginPatchesRaw(requestParameters: GetPluginPatchesRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<Array<SourcePatchInfo>>> {
    if (requestParameters['pluginId'] == null) {
      throw new runtime.RequiredError(
          'pluginId',
          'Required parameter "pluginId" was null or undefined when calling getPluginPatches().'
      );
    }

    if (requestParameters['versionId'] == null) {
      throw new runtime.RequiredError(
          'versionId',
          'Required parameter "versionId" was null or undefined when calling getPluginPatches().'
      );
    }

    const queryParameters: any = {};

    const headerParameters: runtime.HTTPHeaders = {};

    if (this.configuration && this.configuration.accessToken) {
      // oauth required
      headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
    }

    const response = await this.request({
      path: `/plugins/{pluginId}/{versionId}/patches`.replace(`{${"pluginId"}}`, encodeURIComponent(String(requestParameters['pluginId']))).replace(`{${"versionId"}}`, encodeURIComponent(String(requestParameters['versionId']))),
      method: 'GET',
      headers: headerParameters,
      query: queryParameters,
    }, initOverrides);

    return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(SourcePatchInfoFromJSON));
  }

  /**
   * Retrieves a list of source patch information for the specified plugin version.
   */
  async getPluginPatches(requestParameters: GetPluginPatchesRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<Array<SourcePatchInfo>> {
    const response = await this.getPluginPatchesRaw(requestParameters, initOverrides);
    return await response.value();
  }

    /**
     * Retrieves the readme content for a specific version of a plugin.
     */
    async getPluginReadmeRaw(requestParameters: GetPluginReadmeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        if (requestParameters['pluginId'] == null) {
            throw new runtime.RequiredError(
                'pluginId',
                'Required parameter "pluginId" was null or undefined when calling getPluginReadme().'
            );
        }

        if (requestParameters['versionId'] == null) {
            throw new runtime.RequiredError(
                'versionId',
                'Required parameter "versionId" was null or undefined when calling getPluginReadme().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

      if (this.configuration && this.configuration.accessToken) {
        // oauth required
        headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
      }

        const response = await this.request({
            path: `/plugins/{pluginId}/{versionId}/readme`.replace(`{${"pluginId"}}`, encodeURIComponent(String(requestParameters['pluginId']))).replace(`{${"versionId"}}`, encodeURIComponent(String(requestParameters['versionId']))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        if (this.isJsonMime(response.headers.get('content-type'))) {
            return new runtime.JSONApiResponse<string>(response);
        } else {
            return new runtime.TextApiResponse(response) as any;
        }
    }

    /**
     * Retrieves the readme content for a specific version of a plugin.
     */
    async getPluginReadme(requestParameters: GetPluginReadmeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.getPluginReadmeRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Retrieves a paginated list of plugin overviews based on the specified filter and pagination settings.
     */
    async getPluginsRaw(requestParameters: GetPluginsRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<PluginOverviewPage>> {
        const queryParameters: any = {};

        if (requestParameters['match'] != null) {
            queryParameters['match'] = requestParameters['match'];
        }

        if (requestParameters['page'] != null) {
            queryParameters['page'] = requestParameters['page'];
        }

        if (requestParameters['size'] != null) {
            queryParameters['size'] = requestParameters['size'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

      if (this.configuration && this.configuration.accessToken) {
        // oauth required
        headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", []);
      }

        const response = await this.request({
            path: `/plugins`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => PluginOverviewPageFromJSON(jsonValue));
    }

    /**
     * Retrieves a paginated list of plugin overviews based on the specified filter and pagination settings.
     */
    async getPlugins(requestParameters: GetPluginsRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<PluginOverviewPage> {
        const response = await this.getPluginsRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Submits a new plugin version along with optional icon and README information.
     */
    async submitPluginRaw(requestParameters: SubmitPluginRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<PluginVersionInfo>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["X-API-Key"] = await this.configuration.apiKey("X-API-Key"); // apiKey authentication
        }

        if (this.configuration && this.configuration.accessToken) {
            // oauth required
            headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", ["CanSubmitPlugin"]);
        }

        const consumes: runtime.Consume[] = [
            { contentType: 'multipart/form-data' },
        ];
        // @ts-ignore: canConsumeForm may be unused
        const canConsumeForm = runtime.canConsumeForm(consumes);

        let formParams: { append(param: string, value: any): any };
        let useForm = false;
        // use FormData to transmit files using content-type "multipart/form-data"
        useForm = canConsumeForm;
        if (useForm) {
            formParams = new FormData();
        } else {
            formParams = new URLSearchParams();
        }

      if (requestParameters['archive'] != null) {
        formParams.append('archive', requestParameters['archive'] as any);
        }

        const response = await this.request({
            path: `/plugins`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: formParams,
        }, initOverrides);

      return new runtime.JSONApiResponse(response, (jsonValue) => PluginVersionInfoFromJSON(jsonValue));
    }

    /**
     * Submits a new plugin version along with optional icon and README information.
     */
    async submitPlugin(requestParameters: SubmitPluginRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<PluginVersionInfo> {
        const response = await this.submitPluginRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     * Updates the README content for a specific plugin version.
     */
    async updatePluginReadmeRaw(requestParameters: UpdatePluginReadmeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        if (requestParameters['pluginId'] == null) {
            throw new runtime.RequiredError(
                'pluginId',
                'Required parameter "pluginId" was null or undefined when calling updatePluginReadme().'
            );
        }

        if (requestParameters['versionId'] == null) {
            throw new runtime.RequiredError(
                'versionId',
                'Required parameter "versionId" was null or undefined when calling updatePluginReadme().'
            );
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'text/markdown';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["X-API-Key"] = await this.configuration.apiKey("X-API-Key"); // apiKey authentication
        }

        if (this.configuration && this.configuration.accessToken) {
            // oauth required
            headerParameters["Authorization"] = await this.configuration.accessToken("oauth2", ["CanEditPlugin"]);
        }

        const response = await this.request({
            path: `/plugins/{pluginId}/{versionId}/readme`.replace(`{${"pluginId"}}`, encodeURIComponent(String(requestParameters['pluginId']))).replace(`{${"versionId"}}`, encodeURIComponent(String(requestParameters['versionId']))),
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters['body'] as any,
        }, initOverrides);

        if (this.isJsonMime(response.headers.get('content-type'))) {
            return new runtime.JSONApiResponse<string>(response);
        } else {
            return new runtime.TextApiResponse(response) as any;
        }
    }

    /**
     * Updates the README content for a specific plugin version.
     */
    async updatePluginReadme(requestParameters: UpdatePluginReadmeRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.updatePluginReadmeRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
