﻿// <auto-generated />
using ConfigStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ConfigStore.Api.Migrations
{
    [DbContext(typeof(ConfigStoreContext))]
    partial class ConfigStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ConfigStore.Api.Data.Models.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Key");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("ConfigStore.Api.Data.Models.ServiceEnvironment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Key");

                    b.Property<string>("Name");

                    b.Property<int>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("ServiceId", "Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Environments");
                });

            modelBuilder.Entity("ConfigStore.Api.Data.Models.ApplicationService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApplicationId");

                    b.Property<Guid>("Key");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("ApplicationId", "Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ConfigStore.Api.Data.Models.ServiceEnvironment", b =>
                {
                    b.HasOne("ConfigStore.Api.Data.Models.ApplicationService", "Service")
                        .WithMany("Environments")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ConfigStore.Api.Data.Models.ApplicationService", b =>
                {
                    b.HasOne("ConfigStore.Api.Data.Models.Application", "Application")
                        .WithMany("Services")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
