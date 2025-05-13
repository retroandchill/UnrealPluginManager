  [Test]
  public async Task TestBasicAddAndGet() {
    using var scope = _serviceProvider.CreateScope();
    var pluginService = scope.ServiceProvider.GetRequiredService<IPluginService>();
    var plugin1 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin1",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin1/v1.0.0")
        }
    }, null, null);

    var plugin2 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin2",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin2/v1.0.0")
        },
        Dependencies = [
            new PluginDependency {
                PluginName = "Plugin1",
                PluginVersion = SemVersionRange.Parse(">=1.0.0")
            },
            new PluginDependency {
                PluginName = "Paper2D",
                PluginVersion = SemVersionRange.Parse(">=1.0.0"),
                IsEngine = true
            }
        ]
    }, null, null);

    var plugin3 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin3",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin3/v1.0.0")
        },
        Dependencies = [
            new PluginDependency {
                PluginName = "Plugin2",
                PluginVersion = SemVersionRange.AtLeast(new SemVersion(1, 0, 0))
            }
        ]
    }, null, null);

    var plugin4 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin3",
        Version = new SemVersion(1, 2, 1),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin3/v1.2.1")
        },
        Dependencies = [
            new PluginDependency {
                PluginName = "Plugin2",
                PluginVersion = SemVersionRange.AtLeast(new SemVersion(1, 0, 0))
            }
        ]
    }, null, null);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin4",
        Version = new SemVersion(1, 2, 1),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin4/v1.2.1")
        },
        Dependencies = []
    }, null, null);
