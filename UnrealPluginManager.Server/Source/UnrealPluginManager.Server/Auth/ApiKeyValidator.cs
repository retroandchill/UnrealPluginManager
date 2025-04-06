using Microsoft.AspNetCore.Identity;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Provides functionality to validate API keys for authorization purposes.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IApiKeyValidator"/> interface and defines
/// the logic to determine whether a given API key is valid. It is typically used
/// in the context of server authentication mechanisms and is registered as a service
/// for dependency injection.
/// </remarks>
[AutoConstructor]
public partial class ApiKeyValidator : IApiKeyValidator {

  private readonly UnrealPluginManagerContext _dbContext;
  private readonly IPasswordHasher<ApiKeyOverview> _passwordHasher;

  /// <inheritdoc />
  public bool IsValid(string? apiKey) {
    throw new NotImplementedException();
  }
}