using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Student.Repository;

namespace Student.Web.Migrations
{
    [DbContext(typeof(StudentContext))]
    [Migration("20161008094459_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Student.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CreationDate");

                    b.Property<Guid?>("SubjectId");

                    b.Property<string>("Text");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("Student.Models.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupCount");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Student.Models.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("SubjectId");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("Student.Models.Lecture", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("EndTime");

                    b.Property<int>("LectureType");

                    b.Property<string>("Lecturer");

                    b.Property<string>("Location");

                    b.Property<long>("StartTime");

                    b.Property<Guid?>("SubgroupId");

                    b.HasKey("Id");

                    b.HasIndex("SubgroupId");

                    b.ToTable("Lecture");
                });

            modelBuilder.Entity("Student.Models.Subgroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("GroupId");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Subgroup");
                });

            modelBuilder.Entity("Student.Models.Subject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CourseId");

                    b.Property<string>("Name");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Subject");
                });

            modelBuilder.Entity("Student.Models.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("SubgroupId");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SubgroupId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscription");
                });

            modelBuilder.Entity("Student.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Pasword");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Student.Models.Comment", b =>
                {
                    b.HasOne("Student.Models.Subject")
                        .WithMany("Comments")
                        .HasForeignKey("SubjectId");

                    b.HasOne("Student.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Student.Models.Group", b =>
                {
                    b.HasOne("Student.Models.Subject", "Subject")
                        .WithMany("Groups")
                        .HasForeignKey("SubjectId");
                });

            modelBuilder.Entity("Student.Models.Lecture", b =>
                {
                    b.HasOne("Student.Models.Subgroup", "Subgroup")
                        .WithMany("Lectures")
                        .HasForeignKey("SubgroupId");
                });

            modelBuilder.Entity("Student.Models.Subgroup", b =>
                {
                    b.HasOne("Student.Models.Group", "Group")
                        .WithMany("Subgroups")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Student.Models.Subject", b =>
                {
                    b.HasOne("Student.Models.Course")
                        .WithMany("Subjects")
                        .HasForeignKey("CourseId");
                });

            modelBuilder.Entity("Student.Models.Subscription", b =>
                {
                    b.HasOne("Student.Models.Subgroup", "Subgroup")
                        .WithMany()
                        .HasForeignKey("SubgroupId");

                    b.HasOne("Student.Models.User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId");
                });
        }
    }
}
