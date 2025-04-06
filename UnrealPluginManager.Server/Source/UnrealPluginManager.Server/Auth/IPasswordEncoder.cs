using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Defines the interface for encoding and salting passwords.
/// </summary>
public interface IPasswordEncoder {

  /// <summary>
  /// Encodes a plain text password using a specified hashing algorithm.
  /// </summary>
  /// <param name="password">The plain text password to encode.</param>
  /// <returns>The encoded password as a Base64 string.</returns>
  public string Encode(string password);

  /// <summary>
  /// Encodes a plain text password with a randomly generated salt,
  /// returning the salted and hashed password along with the salt used.
  /// </summary>
  /// <param name="password">The plain text password to encode and salt.</param>
  /// <returns>A <c>SaltedPassword</c> struct containing the encoded password and its corresponding salt.</returns>
  public SaltedPassword EncodeAndSalt(string password);

}