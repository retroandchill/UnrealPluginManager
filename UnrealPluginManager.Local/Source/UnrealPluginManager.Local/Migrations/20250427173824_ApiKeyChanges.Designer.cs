﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UnrealPluginManager.Local.Database;

#nullable disable

namespace UnrealPluginManager.Local.Migrations
{
    [DbContext(typeof(LocalUnrealPluginManagerContext))]
    [Migration("20250427173824_ApiKeyChanges")]
    partial class ApiKeyChanges
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<bool>("Optional")
                        .HasColumnType("INTEGER")
                        .HasColumnName("optional");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT")
                        .HasColumnName("parent_id");

                    b.Property<string>("PluginName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("plugin_name");

                    b.Property<string>("PluginVersion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("plugin_version");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_dependency");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_dependency_parent_id");

                    b.ToTable("dependency", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("AuthorName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("author_name");

                    b.Property<string>("AuthorWebsite")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("author_website");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT")
                        .HasColumnName("description");

                    b.Property<string>("FriendlyName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("friendly_name");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_plugins");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_plugins_name");

                    b.ToTable("plugins", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<Guid?>("IconId")
                        .HasColumnType("TEXT")
                        .HasColumnName("icon_id");

                    b.Property<int>("Major")
                        .HasColumnType("INTEGER")
                        .HasColumnName("major");

                    b.Property<string>("Metadata")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("metadata");

                    b.Property<int>("Minor")
                        .HasColumnType("INTEGER")
                        .HasColumnName("minor");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT")
                        .HasColumnName("parent_id");

                    b.Property<int>("Patch")
                        .HasColumnType("INTEGER")
                        .HasColumnName("patch");

                    b.Property<string>("Prerelease")
                        .HasColumnType("TEXT")
                        .HasColumnName("prerelease");

                    b.Property<int?>("PrereleaseNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("prerelease_number");

                    b.Property<Guid?>("ReadmeId")
                        .HasColumnType("TEXT")
                        .HasColumnName("readme_id");

                    b.Property<Guid>("SourceId")
                        .HasColumnType("TEXT")
                        .HasColumnName("source_id");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("version_string");

                    b.HasKey("Id")
                        .HasName("pk_plugin_versions");

                    b.HasIndex("IconId")
                        .HasDatabaseName("ix_plugin_versions_icon_id");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_plugin_versions_parent_id");

                    b.HasIndex("ReadmeId")
                        .HasDatabaseName("ix_plugin_versions_readme_id");

                    b.HasIndex("SourceId")
                        .HasDatabaseName("ix_plugin_versions_source_id");

                    b.ToTable("plugin_versions", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.UploadedBinaries", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<string>("EngineVersion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("engine_version");

                    b.Property<Guid>("FileId")
                        .HasColumnType("TEXT")
                        .HasColumnName("file_id");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT")
                        .HasColumnName("parent_id");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("platform");

                    b.HasKey("Id")
                        .HasName("pk_plugin_binaries");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_plugin_binaries_file_id");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_plugin_binaries_parent_id");

                    b.HasIndex("ParentId", "EngineVersion", "Platform")
                        .IsUnique()
                        .HasDatabaseName("ix_plugin_binaries_parent_id_engine_version_platform");

                    b.ToTable("plugin_binaries", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Storage.FileResource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<string>("OriginalFilename")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("original_filename");

                    b.Property<string>("StoredFilename")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("stored_filename");

                    b.HasKey("Id")
                        .HasName("pk_file_resources");

                    b.ToTable("file_resources", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.AllowedPlugin", b =>
                {
                    b.Property<Guid>("ApiKeyId")
                        .HasColumnType("TEXT")
                        .HasColumnName("api_key_id");

                    b.Property<Guid>("PluginId")
                        .HasColumnType("TEXT")
                        .HasColumnName("plugin_id");

                    b.HasKey("ApiKeyId", "PluginId")
                        .HasName("pk_allowed_plugins");

                    b.HasIndex("PluginId")
                        .HasDatabaseName("ix_allowed_plugins_plugin_id");

                    b.ToTable("allowed_plugins", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.ApiKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("display_name");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("expires_at");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("TEXT")
                        .HasColumnName("external_id");

                    b.Property<string>("PluginGlob")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("plugin_glob");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_api_keys");

                    b.HasIndex("ExternalId")
                        .IsUnique()
                        .HasDatabaseName("ix_api_keys_external_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_api_keys_user_id");

                    b.ToTable("api_keys", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.PluginOwner", b =>
                {
                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT")
                        .HasColumnName("owner_id");

                    b.Property<Guid>("PluginId")
                        .HasColumnType("TEXT")
                        .HasColumnName("plugin_id");

                    b.HasKey("OwnerId", "PluginId")
                        .HasName("pk_plugin_owners");

                    b.HasIndex("PluginId")
                        .HasDatabaseName("ix_plugin_owners_plugin_id");

                    b.ToTable("plugin_owners", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .HasMaxLength(31)
                        .HasColumnType("TEXT")
                        .HasColumnName("password");

                    b.Property<Guid?>("ProfilePictureId")
                        .HasColumnType("TEXT")
                        .HasColumnName("profile_picture_id");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(31)
                        .HasColumnType("TEXT")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("ProfilePictureId")
                        .HasDatabaseName("ix_users_profile_picture_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", "Parent")
                        .WithMany("Dependencies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_dependency_plugin_versions_parent_id");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Storage.FileResource", "Icon")
                        .WithMany()
                        .HasForeignKey("IconId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("fk_plugin_versions_file_resources_icon_id");

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Parent")
                        .WithMany("Versions")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_versions_plugins_parent_id");

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Storage.FileResource", "Readme")
                        .WithMany()
                        .HasForeignKey("ReadmeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("fk_plugin_versions_file_resources_readme_id");

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Storage.FileResource", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_versions_file_resources_source_id");

                    b.Navigation("Icon");

                    b.Navigation("Parent");

                    b.Navigation("Readme");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.UploadedBinaries", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Storage.FileResource", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_binaries_file_resources_file_id");

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", "Parent")
                        .WithMany("Binaries")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_binaries_plugin_versions_parent_id");

                    b.Navigation("File");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.AllowedPlugin", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Users.ApiKey", "ApiKey")
                        .WithMany()
                        .HasForeignKey("ApiKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_allowed_plugins_api_keys_api_key_id");

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Plugin")
                        .WithMany()
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_allowed_plugins_plugins_plugin_id");

                    b.Navigation("ApiKey");

                    b.Navigation("Plugin");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.ApiKey", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Users.User", "User")
                        .WithMany("ApiKeys")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_api_keys_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.PluginOwner", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Users.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_owners_users_owner_id");

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Plugin")
                        .WithMany()
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_owners_plugins_plugin_id");

                    b.Navigation("Owner");

                    b.Navigation("Plugin");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.User", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Storage.FileResource", "ProfilePicture")
                        .WithMany()
                        .HasForeignKey("ProfilePictureId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("fk_users_file_resources_profile_picture_id");

                    b.Navigation("ProfilePicture");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Navigation("Versions");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", b =>
                {
                    b.Navigation("Binaries");

                    b.Navigation("Dependencies");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Users.User", b =>
                {
                    b.Navigation("ApiKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
