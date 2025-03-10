﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UnrealPluginManager.Server.Database;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    [DbContext(typeof(CloudUnrealPluginManagerContext))]
    partial class CloudUnrealPluginManagerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("Optional")
                        .HasColumnType("boolean")
                        .HasColumnName("optional");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_id");

                    b.Property<string>("PluginName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("plugin_name");

                    b.Property<string>("PluginVersion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("plugin_version");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
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
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AuthorName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("author_name");

                    b.Property<string>("AuthorWebsite")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("author_website");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)")
                        .HasColumnName("description");

                    b.Property<string>("FriendlyName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("friendly_name");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
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
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("Major")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("major");

                    b.Property<string>("Metadata")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("metadata");

                    b.Property<int>("Minor")
                        .HasColumnType("integer")
                        .HasColumnName("minor");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_id");

                    b.Property<int>("Patch")
                        .HasColumnType("integer")
                        .HasColumnName("patch");

                    b.Property<string>("Prerelease")
                        .HasColumnType("text")
                        .HasColumnName("prerelease");

                    b.Property<int?>("PrereleaseNumber")
                        .HasColumnType("integer")
                        .HasColumnName("prerelease_number");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("version_string");

                    b.HasKey("Id")
                        .HasName("pk_plugin_versions");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_plugin_versions_parent_id");

                    b.ToTable("plugin_versions", (string)null);
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.UploadedBinaries", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("EngineVersion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("engine_version");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_id");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("platform");

                    b.HasKey("Id")
                        .HasName("pk_plugin_binaries");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_plugin_binaries_parent_id");

                    b.HasIndex("ParentId", "EngineVersion", "Platform")
                        .IsUnique()
                        .HasDatabaseName("ix_plugin_binaries_parent_id_engine_version_platform");

                    b.ToTable("plugin_binaries", (string)null);
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
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Parent")
                        .WithMany("Versions")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_versions_plugins_parent_id");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.UploadedBinaries", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", "Parent")
                        .WithMany("Binaries")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_plugin_binaries_plugin_versions_parent_id");

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
