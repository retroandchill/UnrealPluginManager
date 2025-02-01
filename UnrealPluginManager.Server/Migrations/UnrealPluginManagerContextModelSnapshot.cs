﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UnrealPluginManager.Core.Database;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    [DbContext(typeof(UnrealPluginManagerContext))]
    partial class UnrealPluginManagerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Engine.EngineVersion", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Major")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Minor")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("PluginId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PluginId");

                    b.ToTable("EngineVersion");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.Property<ulong>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ChildId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ParentId", "ChildId");

                    b.HasIndex("ChildId");

                    b.ToTable("Dependency");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<string>("FriendlyName")
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

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Engine.EngineVersion", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Plugin")
                        .WithMany("CompatibleEngineVersions")
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plugin");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Child")
                        .WithMany("DependsOn")
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Parent")
                        .WithMany("DependedBy")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Child");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Navigation("CompatibleEngineVersions");

                    b.Navigation("DependedBy");

                    b.Navigation("DependsOn");
                });
#pragma warning restore 612, 618
        }
    }
}
