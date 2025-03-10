namespace UnrealPluginManager.Server.Config;

/// <summary>
/// Represents the configuration settings required to connect to a PostgreSQL database.
/// </summary>
/// <remarks>
/// This configuration class provides properties to define the database connection parameters,
/// including the host, database name, username, and password. It also generates a connection string
/// dynamically based on these properties.
/// </remarks>
public class PostgresConfig {

  /// <summary>
  /// Gets or sets the host address of the PostgreSQL server.
  /// </summary>
  /// <remarks>
  /// The Host property specifies the address (hostname or IP) of the PostgreSQL database server
  /// to which the application will connect. This is a required parameter for establishing a
  /// database connection.
  /// </remarks>
  public string Host { get; set; } = "localhost";

  /// <summary>
  /// Gets or sets the port number used to connect to the PostgreSQL server.
  /// </summary>
  /// <remarks>
  /// The Port property specifies the port number through which the application communicates
  /// with the PostgreSQL database server. The default value is 5432, which is the standard
  /// port for PostgreSQL. This property should be adjusted if the server is configured to
  /// use a different port.
  /// </remarks>
  public int Port { get; set; } = 5432;
  
  /// <summary>
  /// Gets or sets the name of the PostgreSQL database to connect to.
  /// </summary>
  /// <remarks>
  /// The Database property specifies the name of the specific database within the PostgreSQL server
  /// that this application will interact with. This is a required parameter for database operations.
  /// </remarks>
  public string Database { get; set; } = "unreal_plugin_manager";
  /// <summary>
  /// Gets or sets the username used for authenticating with the PostgreSQL server.
  /// </summary>
  /// <remarks>
  /// The Username property specifies the name of the user account that will be utilized
  /// to authenticate and establish a connection to the PostgreSQL database. This is a
  /// required parameter for accessing the database securely.
  /// </remarks>
  public string Username { get; set; } = "admin";
  /// <summary>
  /// Gets or sets the password used for authenticating to the PostgreSQL database.
  /// </summary>
  /// <remarks>
  /// The Password property specifies the credential required to connect to the PostgreSQL database
  /// server. Ensure the password is handled securely and not exposed in plain text to maintain security
  /// of the database connection.
  /// </remarks>
  public string Password { get; set; } = "develop";

  /// <summary>
  /// Gets the connection string for connecting to the PostgreSQL database.
  /// </summary>
  /// <remarks>
  /// The ConnectionString property dynamically constructs a formatted connection string based on
  /// the host, database name, username, and password specified in the configuration. This string
  /// is used to establish the database connection.
  /// </remarks>
  public string ConnectionString => $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";

}