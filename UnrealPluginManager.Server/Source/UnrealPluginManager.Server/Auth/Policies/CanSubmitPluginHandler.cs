using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Auth.Validators;

namespace UnrealPluginManager.Server.Auth.Policies;

/// <summary>
/// CanSubmitPluginRequirement represents a policy requirement
/// for determining if a user is authorized to submit a plugin.
/// This requirement serves as a marker for the authorization process
/// to enforce specific rules related to the presence and validity of a plugin file,
/// user authentication status, and associated claims or database entries.
/// </summary>
public class CanSubmitPluginRequirement : IAuthorizationRequirement;

/// <summary>
/// CanSubmitPluginHandler is an authorization handler for the CanSubmitPluginRequirement authorization policy.
/// This class checks if a user is authorized to submit a plugin based on specific conditions, such as
/// the presence of a valid .uplugin file in an uploaded archive, authentication method, and user claims or database entries.
/// </summary>
[AutoConstructor]
public partial class CanSubmitPluginHandler : GeneralAuthorizationHandler<CanSubmitPluginRequirement> {
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IPluginAuthValidator _pluginAuthValidator;
  private readonly IJsonService _jsonService;


  /// <inheritdoc />
  protected override async Task HandleInternal(AuthorizationHandlerContext context,
                                               CanSubmitPluginRequirement requirement) {
    using var usingEvaluation = new ContextEvaluation(context, requirement);
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext == null || !httpContext.Request.HasFormContentType ||
        context.User.Identity?.IsAuthenticated != true) {
      context.Fail();
      return;
    }

    var form = await httpContext.Request.ReadFormAsync();
    if (form.Files.Count == 0) {
      return;
    }

    var file = form.Files[0];
    await using var stream = file.OpenReadStream();
    try {
      using var zipFile = new ZipArchive(stream);
      var upluginFile = zipFile.Entries
          .FirstOrDefault(x => x.Name == "plugin.json");
      if (upluginFile is null) {
        return;
      }

      await using var upluginFileStream = upluginFile.Open();
      var manifest = await _jsonService.DeserializeAsync<PluginManifest>(upluginFileStream);
      var pluginName = manifest.Name;
      if (!await _pluginAuthValidator.CanEditPlugin(context, pluginName)) {
        context.Fail();
      }
    } catch (InvalidDataException) {
      // Pass it through
    }
  }
}