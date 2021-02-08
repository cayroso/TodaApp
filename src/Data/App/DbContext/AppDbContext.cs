
using Data.App.Models.Calendars;
using Data.App.Models.Chats;
using Data.App.Models.Drivers;
using Data.App.Models.FileUploads;
using Data.App.Models.Riders;
using Data.App.Models.Trips;
using Data.App.Models.Users;
using Data.Identity.Models;
using Data.Identity.Models.Users;
using Data.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.DbContext
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Get environment	
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory());
            Console.WriteLine($"environment: {environment}");
            Console.WriteLine($"appSettingsPath: {appSettingsPath}");

            // Build config	
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            return new AppDbContext(optionsBuilder.Options, config, null);
        }
    }

    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        #region configuration

        readonly bool _useSQLite;
        readonly string _connString;

        #endregion


        const int KeyMaxLength = 36;
        const int NameMaxLength = 256;
        const int DescMaxLength = 2048;
        const int NoteMaxLength = 4096;

        private Tenant _tenant;

        private readonly IConfiguration _configuration;


        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatReceiver> ChatReceivers { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<FileUpload> FileUploads { get; set; }

        public DbSet<Rider> Riders { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<UserTaskItem> UserTaskItems { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration, ITenantProvider tenantProvider)
            : base(options)
        {
            _configuration = configuration;

            if (tenantProvider != null)
                _tenant = tenantProvider.GetTenant();


            _useSQLite = _configuration.GetValue<bool>("AppSettings:UseSQLite");

            _connString = _useSQLite ? _configuration.GetConnectionString("AppDbContextConnectionSQLite") : _configuration.GetConnectionString("AppDbContextConnectionSQLServer");

            if (_tenant != null)
            {
                _connString = _tenant.DatabaseConnectionString;
            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_useSQLite)
            {
                optionsBuilder.UseSqlite(_connString);
            }
            else
            {
                optionsBuilder.UseSqlServer(_connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            CreateCalendar(builder);

            CreateChats(builder);

            CreateDrivers(builder);

            CreateFileUploads(builder);

            CreateRiders(builder);

            CreateTrips(builder);

            CreateUser(builder);
        }

        void CreateCalendar(ModelBuilder builder)
        {
            builder.Entity<Calendar>(b =>
            {
                b.ToTable("Calendar");
                b.HasKey(e => e.Date);

                b.Property(e => e.MonthName).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DayName).HasMaxLength(KeyMaxLength).IsRequired();
            });

        }

        void CreateChats(ModelBuilder builder)
        {
            builder.Entity<Chat>(b =>
            {
                b.ToTable("Chat");
                b.HasKey(p => p.ChatId);

                b.Property(p => p.ChatId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(p => p.LastChatMessageId).HasMaxLength(KeyMaxLength);
                b.Property(p => p.Title).HasMaxLength(NameMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Receivers)
                    .WithOne(d => d.Chat)
                    .HasForeignKey(f => f.ChatId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(e => e.Messages)
                    .WithOne(d => d.Chat)
                    .HasForeignKey(f => f.ChatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ChatMessage>(b =>
            {
                b.ToTable("ChatMessage");
                b.HasKey(p => p.ChatMessageId);

                b.Property(e => e.ChatMessageId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ChatId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.SenderId).HasMaxLength(KeyMaxLength);
                b.Property(e => e.Content).HasMaxLength(DescMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<ChatReceiver>(b =>
            {
                b.ToTable("ChatReceiver");
                b.HasKey(p => p.ChatReceiverId);

                b.Property(e => e.ChatReceiverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ChatId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ReceiverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.LastChatMessageId).HasMaxLength(KeyMaxLength);

                b.Property(e => e.ConcurrencyStamp).HasMaxLength(KeyMaxLength).IsRequired();
            });


        }

        void CreateDrivers(ModelBuilder builder)
        {
            builder.Entity<Driver>(b =>
            {
                b.ToTable("Driver");
                b.HasKey(e => e.DriverId);

                b.Property(e => e.DriverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasOne(e => e.User).WithOne().HasForeignKey<Driver>(e => e.DriverId);

                b.HasMany(e => e.Trips)
                    .WithOne(d => d.Driver)
                    .HasForeignKey(f => f.DriverId);

                b.HasMany(e => e.Vehicles)
                    .WithOne(d => d.Driver)
                    .HasForeignKey(f => f.DriverId);
            });

            builder.Entity<Vehicle>(b =>
            {
                b.ToTable("Vehicle");
                b.HasKey(e => e.VehicleId);

                b.Property(e => e.VehicleId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DriverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.Trips)
                    .WithOne(d => d.Vehicle)
                    .HasForeignKey(f => f.VehicleId);
            });
        }
        void CreateFileUploads(ModelBuilder builder)
        {
            builder.Entity<FileUpload>(b =>
            {
                b.ToTable("FileUpload");
                b.HasKey(e => e.FileUploadId);

                b.Property(e => e.FileUploadId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Url).HasMaxLength(DescMaxLength);
                b.Property(e => e.FileName).HasMaxLength(DescMaxLength);
                b.Property(e => e.ContentDisposition).HasMaxLength(DescMaxLength);
                b.Property(e => e.ContentType).HasMaxLength(DescMaxLength);


            });
        }

        void CreateRiders(ModelBuilder builder)
        {
            builder.Entity<Rider>(b =>
            {
                b.ToTable("Rider");
                b.HasKey(e => e.RiderId);

                b.Property(e => e.RiderId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasOne(e => e.User).WithOne().HasForeignKey<Rider>(e => e.RiderId);

                b.HasMany(e => e.Bookmarks)
                    .WithOne(d => d.Rider)
                    .HasForeignKey(f => f.RiderId);

                b.HasMany(e => e.Trips)
                    .WithOne(d => d.Rider)
                    .HasForeignKey(f => f.RiderId);
            });
        }

        void CreateTrips(ModelBuilder builder)
        {
            builder.Entity<Trip>(b =>
            {
                b.ToTable("Trip");
                b.HasKey(e => e.TripId);

                b.Property(e => e.TripId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.RiderId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DriverId).HasMaxLength(KeyMaxLength);
                b.Property(e => e.VehicleId).HasMaxLength(KeyMaxLength);
                //b.Property(e => e.TripFeedbackId).HasMaxLength(KeyMaxLength);

                b.Property(e => e.StartAddress).HasMaxLength(DescMaxLength).IsRequired();
                b.Property(e => e.StartAddressDescription).HasMaxLength(DescMaxLength).IsRequired();
                b.Property(e => e.EndAddress).HasMaxLength(DescMaxLength).IsRequired();
                b.Property(e => e.EndAddressDescription).HasMaxLength(DescMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.ExcludedDrivers)
                    .WithOne(d => d.Trip)
                    .HasForeignKey(f => f.TripId);

                b.HasMany(e => e.Locations)
                    .WithOne(d => d.Trip)
                    .HasForeignKey(f => f.TripId);

                b.HasMany(e => e.Timelines)
                    .WithOne(d => d.Trip)
                    .HasForeignKey(f => f.TripId);
            });

            builder.Entity<TripExcludedDriver>(b =>
            {
                b.ToTable("TripExcludedDriver");
                b.HasKey(e => e.TripExcludedDriverId);

                b.Property(e => e.TripExcludedDriverId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.TripId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.DriverId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<TripLocation>(b =>
            {
                b.ToTable("TripLocation");
                b.HasKey(e => e.TripLocationId);

                b.Property(e => e.TripLocationId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.TripId).HasMaxLength(KeyMaxLength).IsRequired();
            });

            builder.Entity<TripTimeline>(b =>
            {
                b.ToTable("TripTimeline");
                b.HasKey(e => e.TripTimelineId);

                b.Property(e => e.TripTimelineId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.TripId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
            });

        }
        //void CreateTeams(ModelBuilder builder)
        //{
        //    builder.Entity<Team>(b =>
        //    {
        //        b.ToTable("Team");
        //        b.HasKey(e => e.TeamId);

        //        b.Property(e => e.TeamId).HasMaxLength(KeyMaxLength).IsRequired();                
        //        b.Property(e => e.Name).HasMaxLength(NameMaxLength);
        //        b.Property(e => e.Description).HasMaxLength(DescMaxLength);

        //        b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

        //        b.HasOne(e => e.Chat).WithOne().HasForeignKey<Team>(e => e.TeamId);

        //        b.HasMany(e => e.Members)
        //            .WithOne(d => d.Team)
        //            .HasForeignKey(d => d.TeamId);
        //    });

        //    builder.Entity<TeamMember>(b =>
        //    {
        //        b.ToTable("TeamMember");
        //        b.HasKey(e => new { e.TeamId, e.MemberId });

        //        b.Property(e => e.TeamId).HasMaxLength(KeyMaxLength).IsRequired();
        //        b.Property(e => e.MemberId).HasMaxLength(KeyMaxLength).IsRequired();
        //    });
        //}


        static void CreateUser(ModelBuilder builder)
        {

            builder.Entity<Role>(b =>
            {
                b.ToTable("Role");
                b.HasKey(e => e.RoleId);

                b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Name).HasMaxLength(NameMaxLength).IsRequired();

                //b.HasMany(e => e.UserRoles)
                //    .WithOne(d => d.Role)
                //    .HasForeignKey(d => d.RoleId);
            });
            builder.Entity<User>(b =>
            {
                b.ToTable("User");
                b.HasKey(e => e.UserId);

                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.FirstName).HasMaxLength(NameMaxLength).IsRequired();
                b.Property(e => e.LastName).HasMaxLength(NameMaxLength).IsRequired();
                b.Property(e => e.Email).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.PhoneNumber).HasMaxLength(KeyMaxLength).IsRequired();

                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.UserTasks)
                    .WithOne(d => d.User)
                    .HasForeignKey(d => d.UserId);

                //b.HasMany(a => a.GivenFeedbacks)
                //    .WithOne(j => j.GivenBy)
                //    .HasForeignKey(j => j.GivenById)
                //    //.OnDelete(DeleteBehavior.Restrict)
                //    ;

                //b.HasMany(a => a.ReceivedFeedbacks)
                //    .WithOne(j => j.ReceivedBy)
                //    .HasForeignKey(j => j.ReceivedById)
                //    //.OnDelete(DeleteBehavior.Restrict)
                //    ;
            });
            builder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRole");
                b.HasKey(e => new { e.UserId, e.RoleId });

                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
            });
            builder.Entity<UserTask>(b =>
            {
                b.ToTable("UserTask");
                b.HasKey(e => e.UserTaskId);

                b.Property(e => e.UserTaskId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.RoleId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.UserId).HasMaxLength(KeyMaxLength);
                b.Property(e => e.Title).HasMaxLength(NameMaxLength).IsRequired();
                b.Property(e => e.Description).HasMaxLength(DescMaxLength);
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();

                b.HasMany(e => e.UserTaskItems)
                    .WithOne(d => d.UserTask)
                    .HasForeignKey(d => d.UserTaskId);
            });

            builder.Entity<UserTaskItem>(b =>
            {
                b.ToTable("UserTaskItem");
                b.HasKey(e => e.UserTaskItemId);

                b.Property(e => e.UserTaskItemId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.UserTaskId).HasMaxLength(KeyMaxLength).IsRequired();
                b.Property(e => e.Title).HasMaxLength(DescMaxLength).IsRequired();
                b.Property(e => e.ConcurrencyToken).HasMaxLength(KeyMaxLength).IsRequired();
            });
        }
    }
}
