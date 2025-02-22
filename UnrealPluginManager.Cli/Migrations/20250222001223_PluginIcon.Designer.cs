﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UnrealPluginManager.Cli.Database;

#nullable disable

namespace UnrealPluginManager.Cli.Migrations
{
    [DbContext(typeof(LocalUnrealPluginManagerContext))]
    [Migration("20250222001223_PluginIcon")]
    partial class PluginIcon
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Optional")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PluginName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("PluginVersion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Dependency");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthorName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorWebsite")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<string>("FriendlyName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginBinaries", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EngineVersion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EngineVersion");

                    b.HasIndex("ParentId");

                    b.ToTable("UploadedPlugins");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("Version");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("VersionString")
                        .IsUnique();

                    b.ToTable("PluginVersions");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", "Parent")
                        .WithMany("Dependencies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginBinaries", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", "Parent")
                        .WithMany("Binaries")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.PluginVersion", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Parent")
                        .WithMany("Versions")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
