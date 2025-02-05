using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Meta;
using UnrealPluginManager.Core.Model.Targets;

namespace UnrealPluginManager.Core.Model.Plugins;

public class PluginReferenceDescriptor {
    		/// <summary>
		/// Name of the plugin
		/// </summary>
		[Required]
		[JsonPropertyName("Name")]
		public string Name { get; set; } = "PlaceholderName";

		/// <summary>
		/// Whether it should be enabled by default
		/// </summary>
		[Required]
		[JsonPropertyName("bEnabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Whether this plugin is optional, and the game should silently ignore it not being present
		/// </summary>
		[Required]
		[JsonPropertyName("bOptional")]
		public bool Optional { get; set; }

		[JsonPropertyName("PluginType")]
		public PluginType PluginType { get; set; } = PluginType.Engine;

		/// <summary>
		/// Description of the plugin for users that do not have it installed.
		/// </summary>
		[JsonPropertyName("Description")]
		public string? Description { get; set; }

		/// <summary>
		/// URL for this plugin on the marketplace, if the user doesn't have it installed.
		/// </summary>
		[JsonPropertyName("MarketplaceURL")]
		public Uri? MarketplaceUrl { get; set; }

		/// <summary>
		/// If enabled, list of platforms for which the plugin should be enabled (or all platforms if blank).
		/// </summary>
		[JsonPropertyName("PlatformAllowList")]
		[DefaultAsEmpty]
		public List<string> PlatformAllowList { get; set; } = [];

		/// <summary>
		/// If enabled, list of platforms for which the plugin should be disabled.
		/// </summary>
		[JsonPropertyName("PlatformDenyList")]
		[DefaultAsEmpty]
		public List<string> PlatformDenyList { get; set; } = [];

		/// <summary>
		/// If enabled, list of target configurations for which the plugin should be enabled (or all target configurations if blank).
		/// </summary>
		[JsonPropertyName("TargetConfigurationAllowList")]
		[DefaultAsEmpty]
		public List<UnrealTargetConfiguration> TargetConfigurationAllowList { get; set; } = [];

		/// <summary>
		/// If enabled, list of target configurations for which the plugin should be disabled.
		/// </summary>
		[JsonPropertyName("TargetConfigurationDenyList")]
		[DefaultAsEmpty]
		public List<UnrealTargetConfiguration> TargetConfigurationDenyList { get; set; } = [];

		/// <summary>
		/// If enabled, list of targets for which the plugin should be enabled (or all targets if blank).
		/// </summary>
		[JsonPropertyName("TargetAllowList")]
		public List<TargetType> TargetAllowList { get; set; } = [];

		/// <summary>
		/// If enabled, list of targets for which the plugin should be disabled.
		/// </summary>
		[JsonPropertyName("TargetDenyList")]
		[DefaultAsEmpty]
		public List<TargetType> TargetDenyList { get; set; } = [];

		/// <summary>
		/// The list of supported platforms for this plugin. This field is copied from the plugin descriptor, and supplements the user's allowed/denied platforms.
		/// </summary>
		[JsonPropertyName("SupportedTargetPlatforms")]
		[DefaultAsEmpty]
		public List<string> SupportedTargetPlatforms { get; set; } = [];

		/// <summary>
		/// When true, empty SupportedTargetPlatforms and PlatformAllowList are interpreted as 'no platforms' with the expectation that explicit platforms will be added in plugin platform extensions
		/// </summary>
		[JsonPropertyName("bHasExplicitPlatforms")]
		public bool HasExplicitPlatforms { get; set; }

		/// <summary>
		/// When set, specifies a specific version of the plugin that this references.
		/// </summary>
		[JsonPropertyName("RequestedVersion")]
		public int? RequestedVersion { get; set; }
		
	
		[JsonPropertyName("VersionMatcher")]
		[JsonConverter(typeof(SemVersionRangeJsonConverter))]
		public SemVersionRange VersionMatcher { get; set; } = SemVersionRange.All;
}