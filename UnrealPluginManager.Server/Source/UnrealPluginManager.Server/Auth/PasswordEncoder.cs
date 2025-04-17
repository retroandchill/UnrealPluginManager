using System.Security.Cryptography;
using System.Text;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Provides functionality to encode passwords securely, including salting and hashing.
/// </summary>
[AutoConstructor]
public partial class PasswordEncoder : IPasswordEncoder {

  private readonly HashAlgorithm _hashAlgorithm;

  /// <inheritdoc />
  public string Encode(string password) {
    var hash = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hash);
  }

  /// <inheritdoc />
  public SaltedPassword EncodeAndSalt(string password) {
    var rng = RandomNumberGenerator.Create();

    var salt = new byte[32];
    rng.GetBytes(salt);
    var saltString = Convert.ToBase64String(salt);
    var saltedPassword = password + saltString;
    return new SaltedPassword(Encode(saltedPassword), saltString);
  }
}