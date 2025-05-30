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
    [Migration("20250315164603_AddFileResources")]
    partial class AddFileResources
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

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

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Navigation("Versions");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", b =>
                {
                    b.Navigation("Binaries");

                    b.Navigation("Dependencies");
                });
#pragma warning restore 612, 618
        }
    }
}
