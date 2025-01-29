﻿using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database;

public class UnrealPluginManagerContext(DbContextOptions<UnrealPluginManagerContext> options) : DbContext(options) {
    public DbSet<Plugin> Plugins { get; init; }
}