using Semver;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Tests.Helpers;

public static class PluginSetupHelpers {
  public static async Task<Guid> SetupVersionResolutionTree(this IPluginService pluginService) {
    var app = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "App",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/App"),
            Sha = "TestSha"
        },
        Dependencies = [
            new PluginDependencyManifest() {
                Name = "Sql",
                Version = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependencyManifest {
                Name = "Threads",
                Version = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependencyManifest {
                Name = "Http",
                Version = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            },
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse("=4.0.0")
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Sql",
        Version = new SemVersion(0, 1, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Sql/v0.1.0"),
            Sha = "SqlTestSha0"
        },
        Dependencies = []
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Sql",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Sql/v1.0.0"),
            Sha = "SqlTestSha1"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=1.0.0 <=4.0.0")
            },
            new PluginDependencyManifest {
                Name = "Threads",
                Version = SemVersionRange.Parse("1.0.0")
            }
        ]
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Sql",
        Version = new SemVersion(2, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Sql/v2.0.0"),
            Sha = "SqlTestSha2"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            },
            new PluginDependencyManifest {
                Name = "Threads",
                Version = SemVersionRange.Parse(">=1.0.0 <=2.0.0")
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Threads",
        Version = new SemVersion(0, 1, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Threads/v0.1.0"),
            Sha = "ThreadsTestSha0"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Threads",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Threads/v1.0.0"),
            Sha = "ThreadsTestSha1"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Threads",
        Version = new SemVersion(2, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Threads/v2.0.0"),
            Sha = "ThreadsTestSha2"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Http",
        Version = new SemVersion(0, 1, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Http/v0.1.0"),
            Sha = "HttpTestSha0"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=0.1.0 <=3.0.0")
            }
        ]
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Http",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Http/v1.0.0"),
            Sha = "HttpTestSha1"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=0.1.0 <=3.0.0")
            }
        ]
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Http",
        Version = new SemVersion(2, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Http/v2.0.0"),
            Sha = "HttpTestSha2"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=1.0.0 <=4.0.0")
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Http",
        Version = new SemVersion(3, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Http/v3.0.0"),
            Sha = "HttpTestSha3"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Http",
        Version = new SemVersion(4, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Http/v4.0.0"),
            Sha = "HttpTestSha4"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "StdLib",
                Version = SemVersionRange.Parse(">=2.0.0 <=4.0.0")
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "StdLib",
        Version = new SemVersion(0, 1, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/StdLib/v0.1.0"),
            Sha = "StdLibTestSha0"
        },
        Dependencies = []
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "StdLib",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/StdLib/v1.0.0"),
            Sha = "StdLibTestSha1"
        },
        Dependencies = []
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "StdLib",
        Version = new SemVersion(2, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/StdLib/v2.0.0"),
            Sha = "StdLibTestSha2"
        },
        Dependencies = []
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "StdLib",
        Version = new SemVersion(3, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/StdLib/v3.0.0"),
            Sha = "StdLibTestSha3"
        },
        Dependencies = []
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "StdLib",
        Version = new SemVersion(4, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/StdLib/v4.0.0"),
            Sha = "StdLibTestSha4"
        },
        Dependencies = []
    }, []);

    return app.PluginId;
  }

  public static async Task<Guid> SetupVersionResolutionTreeWithConflict(this IPluginService pluginService) {
    var app = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "App",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/App/conflict"),
            Sha = "AppConflictSha"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "Sql",
                Version = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependencyManifest {
                Name = "ConflictingDependency",
                Version = SemVersionRange.Parse("=1.0.0")
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Sql",
        Version = new SemVersion(2, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Sql/conflict"),
            Sha = "SqlConflictSha"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "ConflictingDependency",
                Version = SemVersionRange.Parse("=2.0.0") // Conflict: "App" requires 1.0.0, but "Sql" requires 2.0.0
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "ConflictingDependency",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/ConflictingDependency/v1.0.0"),
            Sha = "ConflictDepSha1"
        },
        Dependencies = []
    }, []);
    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "ConflictingDependency",
        Version = new SemVersion(2, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/ConflictingDependency/v2.0.0"),
            Sha = "ConflictDepSha2"
        },
        Dependencies = []
    }, []);

    return app.PluginId;
  }
}