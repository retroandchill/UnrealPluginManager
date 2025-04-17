namespace UnrealPluginManager.Core.Model.Users;

/// <summary>
/// Represents a combination of a hashed password and its corresponding salt value.
/// </summary>
/// <remarks>
/// The <c>SaltedPassword</c> struct is commonly used for securely storing passwords with an added salt value,
/// which helps prevent common password vulnerabilities such as rainbow table attacks.
/// </remarks>
public record struct SaltedPassword(string Password, string Salt);