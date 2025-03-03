using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Targets;

/// <summary>
/// Represents the type of target available in the Unreal Engine build system.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetType {
  /// <summary>
  /// Cooked monolithic game executable (GameName.exe).  Also used for a game-agnostic engine executable (UnrealGame.exe or RocketGame.exe)
  /// </summary>
  Game,

  /// <summary>
  /// Uncooked modular editor executable and DLLs (UnrealEditor.exe, UnrealEditor*.dll, GameName*.dll)
  /// </summary>
  Editor,

  /// <summary>
  /// Cooked monolithic game client executable (GameNameClient.exe, but no server code)
  /// </summary>
  Client,

  /// <summary>
  /// Cooked monolithic game server executable (GameNameServer.exe, but no client code)
  /// </summary>
  Server,

  /// <summary>
  /// Program (standalone program, e.g. ShaderCompileWorker.exe, can be modular or monolithic depending on the program)
  /// </summary>
  Program
}