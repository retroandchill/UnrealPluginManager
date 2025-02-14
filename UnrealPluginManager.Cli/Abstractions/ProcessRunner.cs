﻿using System.Diagnostics;

namespace UnrealPluginManager.Cli.Abstractions;

/// <summary>
/// A class responsible for executing external processes by invoking specified commands with arguments.
/// </summary>
/// <remarks>
/// The ProcessRunner is implemented to run processes asynchronously and return the process exit code.
/// This class is used for interacting with external systems where commands need to be executed
/// programmatically. For instance, it can be employed to run CLI tools or other executables.
/// </remarks>
public class ProcessRunner : IProcessRunner {
    /// <inheritdoc />
    public async Task<int> RunProcess(string command, string[] arguments) {
        var process = new Process();
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = string.Join(" ", arguments);

        process.Start();
        await process.WaitForExitAsync();
        return process.ExitCode;
    }
}