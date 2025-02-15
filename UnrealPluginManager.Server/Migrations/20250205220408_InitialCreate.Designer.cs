﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Server.Database;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    [DbContext(typeof(CloudUnrealPluginManagerContext))]
    [Migration("20250205220408_InitialCreate")]
    partial class InitialCreate
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("Version");

                    b.HasKey("Id");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Dependency", b =>
                {
                    b.HasOne("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", "Parent")
                        .WithMany("Dependencies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("UnrealPluginManager.Core.Database.Entities.Plugins.Plugin", b =>
                {
                    b.Navigation("Dependencies");
                });
#pragma warning restore 612, 618
        }
    }
}
