﻿// <auto-generated />
using System;
using AIDungeonPrompts.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AIDungeonPrompts.Persistence.Migrations
{
    [DbContext(typeof(AIDungeonPromptsDbContext))]
    [Migration("20201118220819_add_audit_logs")]
    partial class add_audit_logs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.AuditPrompt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("AuditDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateEdited")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Entry")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int>("PromptId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AuditPrompts");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.Prompt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("AuthorsNote")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateEdited")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Memory")
                        .HasColumnType("text");

                    b.Property<bool>("Nsfw")
                        .HasColumnType("boolean");

                    b.Property<string>("PromptContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Quests")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Upvote")
                        .HasColumnType("integer");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Title");

                    b.ToTable("Prompts");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.PromptTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateEdited")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PromptId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PromptId");

                    b.HasIndex("TagId");

                    b.ToTable("PromptTags");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateEdited")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ExtraDetails")
                        .HasColumnType("text");

                    b.Property<int>("PromptId")
                        .HasColumnType("integer");

                    b.Property<int>("ReportReason")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PromptId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateEdited")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.WorldInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateEdited")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Entry")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Keys")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PromptId")
                        .HasColumnType("integer");

                    b.Property<int?>("PromptId1")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PromptId");

                    b.HasIndex("PromptId1");

                    b.ToTable("WorldInfos");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("FriendlyName")
                        .HasColumnType("text");

                    b.Property<string>("Xml")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.PromptTag", b =>
                {
                    b.HasOne("AIDungeonPrompts.Domain.Entities.Prompt", "Prompt")
                        .WithMany("PromptTags")
                        .HasForeignKey("PromptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AIDungeonPrompts.Domain.Entities.Tag", "Tag")
                        .WithMany("PromptTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Prompt");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.Report", b =>
                {
                    b.HasOne("AIDungeonPrompts.Domain.Entities.Prompt", "Prompt")
                        .WithMany("Reports")
                        .HasForeignKey("PromptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Prompt");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.WorldInfo", b =>
                {
                    b.HasOne("AIDungeonPrompts.Domain.Entities.Prompt", "Prompt")
                        .WithMany()
                        .HasForeignKey("PromptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AIDungeonPrompts.Domain.Entities.Prompt", null)
                        .WithMany("WorldInfos")
                        .HasForeignKey("PromptId1");

                    b.Navigation("Prompt");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.Prompt", b =>
                {
                    b.Navigation("PromptTags");

                    b.Navigation("Reports");

                    b.Navigation("WorldInfos");
                });

            modelBuilder.Entity("AIDungeonPrompts.Domain.Entities.Tag", b =>
                {
                    b.Navigation("PromptTags");
                });
#pragma warning restore 612, 618
        }
    }
}
