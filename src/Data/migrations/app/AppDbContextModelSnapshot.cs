﻿// <auto-generated />
using System;
using Data.App.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.migrations.app
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Data.App.Models.Calendars.Calendar", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Day")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DayName")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DayOfYear")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MonthName")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quarter")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Date");

                    b.ToTable("Calendar");
                });

            modelBuilder.Entity("Data.App.Models.Chats.Chat", b =>
                {
                    b.Property<string>("ChatId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastChatMessageId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("ChatId");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatMessage", b =>
                {
                    b.Property<string>("ChatMessageId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ChatId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("ChatMessageType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateSent")
                        .HasColumnType("TEXT");

                    b.Property<string>("SenderId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("ChatMessageId");

                    b.HasIndex("ChatId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatReceiver", b =>
                {
                    b.Property<string>("ChatReceiverId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ChatId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastChatMessageId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ReceiverId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("ChatReceiverId");

                    b.HasIndex("ChatId");

                    b.HasIndex("ReceiverId");

                    b.ToTable("ChatReceiver");
                });

            modelBuilder.Entity("Data.App.Models.Drivers.Driver", b =>
                {
                    b.Property<string>("DriverId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Availability")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("DriverId");

                    b.ToTable("Driver");
                });

            modelBuilder.Entity("Data.App.Models.Drivers.Vehicle", b =>
                {
                    b.Property<string>("VehicleId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("DriverId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("VehicleId");

                    b.HasIndex("DriverId");

                    b.ToTable("Vehicle");
                });

            modelBuilder.Entity("Data.App.Models.FileUploads.FileUpload", b =>
                {
                    b.Property<string>("FileUploadId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("ContentDisposition")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContentType")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<long>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.HasKey("FileUploadId");

                    b.ToTable("FileUpload");
                });

            modelBuilder.Entity("Data.App.Models.Riders.Rider", b =>
                {
                    b.Property<string>("RiderId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("RiderId");

                    b.ToTable("Rider");
                });

            modelBuilder.Entity("Data.App.Models.Riders.RiderBookmark", b =>
                {
                    b.Property<string>("RiderBookmarkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("RiderId")
                        .HasColumnType("TEXT");

                    b.HasKey("RiderBookmarkId");

                    b.HasIndex("RiderId");

                    b.ToTable("RiderBookmark");
                });

            modelBuilder.Entity("Data.App.Models.Trips.Trip", b =>
                {
                    b.Property<string>("TripId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("CancelReason")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("DriverComment")
                        .HasColumnType("TEXT");

                    b.Property<string>("DriverId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("DriverRating")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EndAddress")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("EndAddressDescription")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<double>("EndX")
                        .HasColumnType("REAL");

                    b.Property<double>("EndY")
                        .HasColumnType("REAL");

                    b.Property<decimal>("Fare")
                        .HasColumnType("TEXT");

                    b.Property<string>("RiderComment")
                        .HasColumnType("TEXT");

                    b.Property<string>("RiderId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("RiderRating")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StartAddress")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("StartAddressDescription")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<double>("StartX")
                        .HasColumnType("REAL");

                    b.Property<double>("StartY")
                        .HasColumnType("REAL");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VehicleId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("TripId");

                    b.HasIndex("DriverId");

                    b.HasIndex("RiderId");

                    b.HasIndex("VehicleId");

                    b.ToTable("Trip");
                });

            modelBuilder.Entity("Data.App.Models.Trips.TripLocation", b =>
                {
                    b.Property<string>("TripLocationId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<double>("GeoX")
                        .HasColumnType("REAL");

                    b.Property<double>("GeoY")
                        .HasColumnType("REAL");

                    b.Property<string>("TripId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("TripLocationType")
                        .HasColumnType("INTEGER");

                    b.HasKey("TripLocationId");

                    b.HasIndex("TripId");

                    b.ToTable("TripLocation");
                });

            modelBuilder.Entity("Data.App.Models.Trips.TripTimeline", b =>
                {
                    b.Property<string>("TripTimelineId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTimeline")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TripId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("TripTimelineId");

                    b.HasIndex("TripId");

                    b.HasIndex("UserId");

                    b.ToTable("TripTimeline");
                });

            modelBuilder.Entity("Data.App.Models.Users.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Data.App.Models.Users.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("ImageId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTask", b =>
                {
                    b.Property<string>("UserTaskId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateActualCompleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateAssigned")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCompleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserTaskId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTask");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTaskItem", b =>
                {
                    b.Property<string>("UserTaskItemId")
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyToken")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCompleted")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDone")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserTaskId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("TEXT");

                    b.HasKey("UserTaskItemId");

                    b.HasIndex("UserTaskId");

                    b.ToTable("UserTaskItem");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatMessage", b =>
                {
                    b.HasOne("Data.App.Models.Chats.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Chat");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Data.App.Models.Chats.ChatReceiver", b =>
                {
                    b.HasOne("Data.App.Models.Chats.Chat", "Chat")
                        .WithMany("Receivers")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Receiver");
                });

            modelBuilder.Entity("Data.App.Models.Drivers.Driver", b =>
                {
                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Data.App.Models.Drivers.Driver", "DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Drivers.Vehicle", b =>
                {
                    b.HasOne("Data.App.Models.Drivers.Driver", "Driver")
                        .WithMany("Vehicles")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Driver");
                });

            modelBuilder.Entity("Data.App.Models.Riders.Rider", b =>
                {
                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Data.App.Models.Riders.Rider", "RiderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Riders.RiderBookmark", b =>
                {
                    b.HasOne("Data.App.Models.Riders.Rider", "Rider")
                        .WithMany("Bookmarks")
                        .HasForeignKey("RiderId");

                    b.Navigation("Rider");
                });

            modelBuilder.Entity("Data.App.Models.Trips.Trip", b =>
                {
                    b.HasOne("Data.App.Models.Drivers.Driver", "Driver")
                        .WithMany("Trips")
                        .HasForeignKey("DriverId");

                    b.HasOne("Data.App.Models.Riders.Rider", "Rider")
                        .WithMany("Trips")
                        .HasForeignKey("RiderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Drivers.Vehicle", "Vehicle")
                        .WithMany("Trips")
                        .HasForeignKey("VehicleId");

                    b.Navigation("Driver");

                    b.Navigation("Rider");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("Data.App.Models.Trips.TripLocation", b =>
                {
                    b.HasOne("Data.App.Models.Trips.Trip", "Trip")
                        .WithMany("Locations")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("Data.App.Models.Trips.TripTimeline", b =>
                {
                    b.HasOne("Data.App.Models.Trips.Trip", "Trip")
                        .WithMany("Timelines")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trip");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.User", b =>
                {
                    b.HasOne("Data.App.Models.FileUploads.FileUpload", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserRole", b =>
                {
                    b.HasOne("Data.App.Models.Users.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTask", b =>
                {
                    b.HasOne("Data.App.Models.Users.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.App.Models.Users.User", "User")
                        .WithMany("UserTasks")
                        .HasForeignKey("UserId");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTaskItem", b =>
                {
                    b.HasOne("Data.App.Models.Users.UserTask", "UserTask")
                        .WithMany("UserTaskItems")
                        .HasForeignKey("UserTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserTask");
                });

            modelBuilder.Entity("Data.App.Models.Chats.Chat", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Receivers");
                });

            modelBuilder.Entity("Data.App.Models.Drivers.Driver", b =>
                {
                    b.Navigation("Trips");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("Data.App.Models.Drivers.Vehicle", b =>
                {
                    b.Navigation("Trips");
                });

            modelBuilder.Entity("Data.App.Models.Riders.Rider", b =>
                {
                    b.Navigation("Bookmarks");

                    b.Navigation("Trips");
                });

            modelBuilder.Entity("Data.App.Models.Trips.Trip", b =>
                {
                    b.Navigation("Locations");

                    b.Navigation("Timelines");
                });

            modelBuilder.Entity("Data.App.Models.Users.User", b =>
                {
                    b.Navigation("UserRoles");

                    b.Navigation("UserTasks");
                });

            modelBuilder.Entity("Data.App.Models.Users.UserTask", b =>
                {
                    b.Navigation("UserTaskItems");
                });
#pragma warning restore 612, 618
        }
    }
}
