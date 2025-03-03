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
import type {
  DependencyManifest,
  PluginDependency,
  PluginDetails,
  PluginOverviewPage,
  PluginSummary,
} from '../models/index';
import {
  DependencyManifestFromJSON,
  DependencyManifestToJSON,
  PluginDependencyFromJSON,
  PluginDependencyToJSON,
  PluginDetailsFromJSON,
  PluginDetailsToJSON,
  PluginOverviewPageFromJSON,
  PluginOverviewPageToJSON,
  PluginSummaryFromJSON,
  PluginSummaryToJSON,
} from '../models/index';

export interface AddPluginRequest {
  engineVersion?: string;
  pluginFile?: Blob;
}

export interface DownloadPluginRequest {
  pluginName: string;
  engineVersion?: string;
  platforms?: Array<string>;
}

export interface GetCandidateDependenciesRequest {
  pluginDependency: Array<PluginDependency>;
}

export interface GetDependencyTreeRequest {
  pluginName: string;
}

export interface GetPluginsRequest {
  match?: string;
  page?: number;
  size?: number;
}

/**
 *
 */
export class PluginsApi extends runtime.BaseAPI {

  /**
   * Adds a plugin by uploading a plugin file and specifying the target Unreal Engine version.
   */
  async addPluginRaw(requestParameters: AddPluginRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<PluginDetails>> {
    const queryParameters: any = {};

    if (requestParameters['engineVersion'] != null) {
      queryParameters['engineVersion'] = requestParameters['engineVersion'];
    }

    const headerParameters: runtime.HTTPHeaders = {};

    const consumes: runtime.Consume[] = [
      {contentType: 'multipart/form-data'},
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

    if (requestParameters['pluginFile'] != null) {
      formParams.append('pluginFile', requestParameters['pluginFile'] as any);
    }

    const response = await this.request({
      path: `/api/plugins`,
      method: 'POST',
      headers: headerParameters,
      query: queryParameters,
      body: formParams,
    }, initOverrides);

    return new runtime.JSONApiResponse(response, (jsonValue) => PluginDetailsFromJSON(jsonValue));
  }

  /**
   * Adds a plugin by uploading a plugin file and specifying the target Unreal Engine version.
   */
  async addPlugin(requestParameters: AddPluginRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<PluginDetails> {
    const response = await this.addPluginRaw(requestParameters, initOverrides);
    return await response.value();
  }

  /**
   * Downloads a plugin file as a ZIP archive for the specified plugin, engine version, and target platforms.
   */
  async downloadPluginRaw(requestParameters: DownloadPluginRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<Blob>> {
    if (requestParameters['pluginName'] == null) {
      throw new runtime.RequiredError(
          'pluginName',
          'Required parameter "pluginName" was null or undefined when calling downloadPlugin().'
      );
    }

    const queryParameters: any = {};

    if (requestParameters['engineVersion'] != null) {
      queryParameters['engineVersion'] = requestParameters['engineVersion'];
    }

    if (requestParameters['platforms'] != null) {
      queryParameters['platforms'] = requestParameters['platforms'];
    }

    const headerParameters: runtime.HTTPHeaders = {};

    const response = await this.request({
      path: `/api/plugins/{pluginName}/download`.replace(`{${"pluginName"}}`, encodeURIComponent(String(requestParameters['pluginName']))),
      method: 'GET',
      headers: headerParameters,
      query: queryParameters,
    }, initOverrides);

    return new runtime.BlobApiResponse(response);
  }

  /**
   * Downloads a plugin file as a ZIP archive for the specified plugin, engine version, and target platforms.
   */
  async downloadPlugin(requestParameters: DownloadPluginRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<Blob> {
    const response = await this.downloadPluginRaw(requestParameters, initOverrides);
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

    const response = await this.request({
      path: `/api/plugins/dependencies/candidates`,
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
    if (requestParameters['pluginName'] == null) {
      throw new runtime.RequiredError(
          'pluginName',
          'Required parameter "pluginName" was null or undefined when calling getDependencyTree().'
      );
    }

    const queryParameters: any = {};

    const headerParameters: runtime.HTTPHeaders = {};

    const response = await this.request({
      path: `/api/plugins/{pluginName}`.replace(`{${"pluginName"}}`, encodeURIComponent(String(requestParameters['pluginName']))),
      method: 'GET',
      headers: headerParameters,
      query: queryParameters,
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

    const response = await this.request({
      path: `/api/plugins`,
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

}
