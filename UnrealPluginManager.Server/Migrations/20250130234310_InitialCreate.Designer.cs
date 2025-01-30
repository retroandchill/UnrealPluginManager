﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UnrealPluginManager.Core.Database;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    [DbContext(typeof(UnrealPluginManagerContext))]
    [Migration("20250130234310_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

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
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Plugins");
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
                    b.Navigation("DependedBy");

                    b.Navigation("DependsOn");
                });
#pragma warning restore 612, 618
        }
    }
}
