using System.CommandLine;
using UnrealPluginManager.Cli.Utils;

var rootCommand = new RootCommand();
rootCommand.SetUpCommands();

var opt = new Option<string>("--engine-path");