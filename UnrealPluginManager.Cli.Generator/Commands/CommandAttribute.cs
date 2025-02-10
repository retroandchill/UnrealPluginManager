﻿namespace UnrealPluginManager.Cli.Generator.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string name, string? description = null) : Attribute {
    
}