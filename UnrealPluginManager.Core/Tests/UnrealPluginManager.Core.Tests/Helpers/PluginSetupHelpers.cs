using Semver;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Tests.Helpers;

public static class PluginSetupHelpers {
  public static async Task<Guid> SetupVersionResolutionTree(this IPluginService pluginService) {
    var app = await pluginService.AddPlugin("App", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Sql",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("=2.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "Threads",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("=2.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "Http",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("=4.0.0")
            }
        ]
    });

    await pluginService.AddPlugin("Sql", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(0, 1, 0)
    });
    await pluginService.AddPlugin("Sql", new PluginDescriptor {
        Version = 2,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=1.0.0 <=4.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "Threads",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("1.0.0")
            }
        ]
    });
    await pluginService.AddPlugin("Sql", new PluginDescriptor {
        Version = 3,
        VersionName = new SemVersion(2, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "Threads",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=1.0.0 <=2.0.0")
            }
        ]
    });

    await pluginService.AddPlugin("Threads", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(0, 1, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    });
    await pluginService.AddPlugin("Threads", new PluginDescriptor {
        Version = 2,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    });
    await pluginService.AddPlugin("Threads", new PluginDescriptor {
        Version = 3,
        VersionName = new SemVersion(2, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            }
        ]
    });

    await pluginService.AddPlugin("Http", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(0, 1, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=0.1.0 <=3.0.0")
            }
        ]
    });
    await pluginService.AddPlugin("Http", new PluginDescriptor {
        Version = 2,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=0.1.0 <=3.0.0")
            }
        ]
    });
    await pluginService.AddPlugin("Http", new PluginDescriptor {
        Version = 3,
        VersionName = new SemVersion(2, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=1.0.0 <=4.0.0")
            }
        ]
    });

    await pluginService.AddPlugin("Http", new PluginDescriptor {
        Version = 4,
        VersionName = new SemVersion(3, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    });
    await pluginService.AddPlugin("Http", new PluginDescriptor {
        Version = 5,
        VersionName = new SemVersion(4, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "StdLib",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            }
        ]
    });

    await pluginService.AddPlugin("StdLib", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(0, 1, 0),
    });
    await pluginService.AddPlugin("StdLib", new PluginDescriptor {
        Version = 2,
        VersionName = new SemVersion(1, 0, 0),
    });
    await pluginService.AddPlugin("StdLib", new PluginDescriptor {
        Version = 3,
        VersionName = new SemVersion(2, 0, 0),
    });
    await pluginService.AddPlugin("StdLib", new PluginDescriptor {
        Version = 4,
        VersionName = new SemVersion(3, 0, 0),
    });
    await pluginService.AddPlugin("StdLib", new PluginDescriptor {
        Version = 5,
        VersionName = new SemVersion(4, 0, 0),
    });
    
    return app.PluginId;
  }
  
  public static async Task<Guid> SetupVersionResolutionTreeWithConflict(this IPluginService pluginService) {
    var app = await pluginService.AddPlugin("App", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Sql",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("=2.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "ConflictingDependency",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("=1.0.0")
            }
        ]
    });

    await pluginService.AddPlugin("Sql", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(2, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "ConflictingDependency",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse("=2.0.0") // Conflict: "App" requires 1.0.0, but "Sql" requires 2.0.0
            }
        ]
    });

    await pluginService.AddPlugin("ConflictingDependency", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0)
    });
    await pluginService.AddPlugin("ConflictingDependency", new PluginDescriptor {
        Version = 2,
        VersionName = new SemVersion(2, 0, 0)
    });

    return app.PluginId;
  }
}