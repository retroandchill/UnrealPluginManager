/*
 * UnrealPluginManager.Server
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using UnrealPluginManager.WebClient.Client;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Model.Plugins;


namespace UnrealPluginManager.WebClient.Api {
  using PluginOverviewPage = Page<PluginOverview>;


  /// <summary>
  /// Represents a collection of functions to interact with the API endpoints
  /// </summary>
  public interface IStorageApiSync : IApiAccessor {
    #region Synchronous Operations

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name.
    /// </summary>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <returns>System.IO.Stream</returns>
    System.IO.Stream GetIcon(string pluginName, int operationIndex = 0);

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <returns>ApiResponse of System.IO.Stream</returns>
    ApiResponse<System.IO.Stream> GetIconWithHttpInfo(string pluginName, int operationIndex = 0);

    #endregion Synchronous Operations
  }

  /// <summary>
  /// Represents a collection of functions to interact with the API endpoints
  /// </summary>
  public interface IStorageApiAsync : IApiAccessor {
    #region Asynchronous Operations

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of System.IO.Stream</returns>
    System.Threading.Tasks.Task<System.IO.Stream> GetIconAsync(string pluginName, int operationIndex = 0,
                                                               System.Threading.CancellationToken cancellationToken =
                                                                   default(System.Threading.CancellationToken));

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (System.IO.Stream)</returns>
    System.Threading.Tasks.Task<ApiResponse<System.IO.Stream>> GetIconWithHttpInfoAsync(
        string pluginName, int operationIndex = 0,
        System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    #endregion Asynchronous Operations
  }

  /// <summary>
  /// Represents a collection of functions to interact with the API endpoints
  /// </summary>
  public interface IStorageApi : IStorageApiSync, IStorageApiAsync {
  }

  /// <summary>
  /// Represents a collection of functions to interact with the API endpoints
  /// </summary>
  public partial class StorageApi : IStorageApi {
    private UnrealPluginManager.WebClient.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageApi"/> class.
    /// </summary>
    /// <returns></returns>
    public StorageApi() : this((string)null) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageApi"/> class.
    /// </summary>
    /// <returns></returns>
    public StorageApi(string basePath) {
      this.Configuration = UnrealPluginManager.WebClient.Client.Configuration.MergeConfigurations(
          UnrealPluginManager.WebClient.Client.GlobalConfiguration.Instance,
          new UnrealPluginManager.WebClient.Client.Configuration { BasePath = basePath }
      );
      this.Client = new UnrealPluginManager.WebClient.Client.ApiClient(this.Configuration.BasePath);
      this.AsynchronousClient = new UnrealPluginManager.WebClient.Client.ApiClient(this.Configuration.BasePath);
      this.ExceptionFactory = UnrealPluginManager.WebClient.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageApi"/> class
    /// using Configuration object
    /// </summary>
    /// <param name="configuration">An instance of Configuration</param>
    /// <returns></returns>
    public StorageApi(UnrealPluginManager.WebClient.Client.Configuration configuration) {
      if (configuration == null) throw new ArgumentNullException("configuration");

      this.Configuration = UnrealPluginManager.WebClient.Client.Configuration.MergeConfigurations(
          UnrealPluginManager.WebClient.Client.GlobalConfiguration.Instance,
          configuration
      );
      this.Client = new UnrealPluginManager.WebClient.Client.ApiClient(this.Configuration.BasePath);
      this.AsynchronousClient = new UnrealPluginManager.WebClient.Client.ApiClient(this.Configuration.BasePath);
      ExceptionFactory = UnrealPluginManager.WebClient.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageApi"/> class
    /// using a Configuration object and client instance.
    /// </summary>
    /// <param name="client">The client interface for synchronous API access.</param>
    /// <param name="asyncClient">The client interface for asynchronous API access.</param>
    /// <param name="configuration">The configuration object.</param>
    public StorageApi(UnrealPluginManager.WebClient.Client.ISynchronousClient client,
                      UnrealPluginManager.WebClient.Client.IAsynchronousClient asyncClient,
                      UnrealPluginManager.WebClient.Client.IReadableConfiguration configuration) {
      if (client == null) throw new ArgumentNullException("client");
      if (asyncClient == null) throw new ArgumentNullException("asyncClient");
      if (configuration == null) throw new ArgumentNullException("configuration");

      this.Client = client;
      this.AsynchronousClient = asyncClient;
      this.Configuration = configuration;
      this.ExceptionFactory = UnrealPluginManager.WebClient.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// The client for accessing this underlying API asynchronously.
    /// </summary>
    public UnrealPluginManager.WebClient.Client.IAsynchronousClient AsynchronousClient { get; set; }

    /// <summary>
    /// The client for accessing this underlying API synchronously.
    /// </summary>
    public UnrealPluginManager.WebClient.Client.ISynchronousClient Client { get; set; }

    /// <summary>
    /// Gets the base path of the API client.
    /// </summary>
    /// <value>The base path</value>
    public string GetBasePath() {
      return this.Configuration.BasePath;
    }

    /// <summary>
    /// Gets or sets the configuration object
    /// </summary>
    /// <value>An instance of the Configuration</value>
    public UnrealPluginManager.WebClient.Client.IReadableConfiguration Configuration { get; set; }

    /// <summary>
    /// Provides a factory method hook for the creation of exceptions.
    /// </summary>
    public UnrealPluginManager.WebClient.Client.ExceptionFactory ExceptionFactory {
      get {
        if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1) {
          throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
        }

        return _exceptionFactory;
      }
      set { _exceptionFactory = value; }
    }

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name. 
    /// </summary>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <returns>System.IO.Stream</returns>
    public System.IO.Stream GetIcon(string pluginName, int operationIndex = 0) {
      UnrealPluginManager.WebClient.Client.ApiResponse<System.IO.Stream> localVarResponse =
          GetIconWithHttpInfo(pluginName);
      return localVarResponse.Data;
    }

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name. 
    /// </summary>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <returns>ApiResponse of System.IO.Stream</returns>
    public UnrealPluginManager.WebClient.Client.ApiResponse<System.IO.Stream> GetIconWithHttpInfo(
        string pluginName, int operationIndex = 0) {
      // verify the required parameter 'pluginName' is set
      if (pluginName == null) {
        throw new UnrealPluginManager.WebClient.Client.ApiException(
            400, "Missing required parameter 'pluginName' when calling StorageApi->GetIcon");
      }

      UnrealPluginManager.WebClient.Client.RequestOptions localVarRequestOptions =
          new UnrealPluginManager.WebClient.Client.RequestOptions();

      string[] _contentTypes = new string[] {
      };

      // to determine the Accept header
      string[] _accepts = new string[] {
          "image/png"
      };

      var localVarContentType = UnrealPluginManager.WebClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
      if (localVarContentType != null) {
        localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
      }

      var localVarAccept = UnrealPluginManager.WebClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
      if (localVarAccept != null) {
        localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
      }

      localVarRequestOptions.PathParameters.Add("pluginName",
                                                UnrealPluginManager.WebClient.Client.ClientUtils.ParameterToString(
                                                    pluginName)); // path parameter

      localVarRequestOptions.Operation = "StorageApi.GetIcon";
      localVarRequestOptions.OperationIndex = operationIndex;


      // make the HTTP request
      var localVarResponse =
          this.Client.Get<System.IO.Stream>("/files/icons/{pluginName}", localVarRequestOptions, this.Configuration);
      if (this.ExceptionFactory != null) {
        Exception _exception = this.ExceptionFactory("GetIcon", localVarResponse);
        if (_exception != null) {
          throw _exception;
        }
      }

      return localVarResponse;
    }

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name. 
    /// </summary>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of System.IO.Stream</returns>
    public async System.Threading.Tasks.Task<System.IO.Stream> GetIconAsync(
        string pluginName, int operationIndex = 0,
        System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) {
      UnrealPluginManager.WebClient.Client.ApiResponse<System.IO.Stream> localVarResponse =
          await GetIconWithHttpInfoAsync(pluginName, operationIndex, cancellationToken).ConfigureAwait(false);
      return localVarResponse.Data;
    }

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name. 
    /// </summary>
    /// <exception cref="UnrealPluginManager.WebClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="pluginName">The name of the plugin to search for</param>
    /// <param name="operationIndex">Index associated with the operation.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (System.IO.Stream)</returns>
    public async System.Threading.Tasks.Task<UnrealPluginManager.WebClient.Client.ApiResponse<System.IO.Stream>>
        GetIconWithHttpInfoAsync(string pluginName, int operationIndex = 0,
                                 System.Threading.CancellationToken cancellationToken =
                                     default(System.Threading.CancellationToken)) {
      // verify the required parameter 'pluginName' is set
      if (pluginName == null) {
        throw new UnrealPluginManager.WebClient.Client.ApiException(
            400, "Missing required parameter 'pluginName' when calling StorageApi->GetIcon");
      }


      UnrealPluginManager.WebClient.Client.RequestOptions localVarRequestOptions =
          new UnrealPluginManager.WebClient.Client.RequestOptions();

      string[] _contentTypes = new string[] {
      };

      // to determine the Accept header
      string[] _accepts = new string[] {
          "image/png"
      };

      var localVarContentType = UnrealPluginManager.WebClient.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
      if (localVarContentType != null) {
        localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
      }

      var localVarAccept = UnrealPluginManager.WebClient.Client.ClientUtils.SelectHeaderAccept(_accepts);
      if (localVarAccept != null) {
        localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
      }

      localVarRequestOptions.PathParameters.Add("pluginName",
                                                UnrealPluginManager.WebClient.Client.ClientUtils.ParameterToString(
                                                    pluginName)); // path parameter

      localVarRequestOptions.Operation = "StorageApi.GetIcon";
      localVarRequestOptions.OperationIndex = operationIndex;


      // make the HTTP request
      var localVarResponse = await this.AsynchronousClient
          .GetAsync<System.IO.Stream>("/files/icons/{pluginName}", localVarRequestOptions, this.Configuration,
                                      cancellationToken).ConfigureAwait(false);

      if (this.ExceptionFactory != null) {
        Exception _exception = this.ExceptionFactory("GetIcon", localVarResponse);
        if (_exception != null) {
          throw _exception;
        }
      }

      return localVarResponse;
    }
  }
}