using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Scripting;

/// <summary>
/// Represents the scope levels available for Verse definitions or functionality.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VerseScope {
    /// <summary>
    /// Created by Epic and only public definitions will be visible to public users
    /// </summary>
    PublicAPI,

    /// <summary>
    /// Created by Epic and is entirely hidden from public users
    /// </summary>
    InternalAPI,

    /// <summary>
    /// Created by a public user
    /// </summary>
    PublicUser,

    /// <summary>
    /// Created by an Epic internal user
    /// </summary>
    InternalUser
}