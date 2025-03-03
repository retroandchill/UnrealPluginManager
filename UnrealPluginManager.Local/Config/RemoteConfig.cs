namespace UnrealPluginManager.Local.Config;

/// <summary>
/// Represents credentials used for accessing a remote service or resource.
/// </summary>
/// <remarks>
/// This struct encapsulates authentication information, including a username and token,
/// to facilitate secure communication with a remote endpoint. It includes operator
/// overloads for equality to compare different instances of <c>RemoteCredentials</c>.
/// </remarks>
public struct RemoteCredentials {
  /// <summary>
  /// Gets or sets the username associated with the remote credentials.
  /// </summary>
  /// <remarks>
  /// The username is used for authentication when interacting with a remote service
  /// or resource. It identifies the user or system attempting to establish a secure connection.
  /// </remarks>
  public string Username { get; set; }

  /// <summary>
  /// Gets or sets the token used for authenticating with a remote service or resource.
  /// </summary>
  /// <remarks>
  /// The token serves as an authentication credential, typically used in conjunction with
  /// or as an alternative to a username. It ensures secure access to protected endpoints
  /// or resources during remote communication.
  /// </remarks>
  public string Token { get; set; }
}

/// <summary>
/// Represents the configuration for a remote service or resource.
/// </summary>
/// <remarks>
/// This class encapsulates the endpoint details and optional authentication information required
/// to interact with a remote service. It includes properties for specifying the URI and credentials,
/// and provides implicit conversion from <c>Uri</c> to <c>RemoteConfig</c> for ease of use.
/// </remarks>
public class RemoteConfig {
  /// <summary>
  /// Gets or sets the URL of the remote resource or service.
  /// </summary>
  /// <remarks>
  /// The URL specifies the endpoint used to interact with the remote resource.
  /// It should be a valid URI that identifies the resource or service being accessed.
  /// </remarks>
  public required Uri Url { get; set; }

  /// <summary>
  /// Gets or sets the credentials used for authenticating with a remote service.
  /// </summary>
  /// <remarks>
  /// The credentials comprise authentication data, such as a username and token,
  /// that ensure secure access to a remote endpoint. This property can be null
  /// if no authentication is required for the service.
  /// </remarks>
  public RemoteCredentials? Credentials { get; set; }

  /// <summary>
  /// Defines implicit conversion operators for the <see cref="RemoteConfig"/> class.
  /// </summary>
  /// <remarks>
  /// Includes operator overloads to enable seamless conversion between a <see cref="Uri"/> and a
  /// <see cref="RemoteConfig"/> object. This facilitates scenarios where either type
  /// can be used interchangeably without the need for explicit type casting.
  /// </remarks>
  public static implicit operator RemoteConfig(Uri url) => new() { Url = url };
}