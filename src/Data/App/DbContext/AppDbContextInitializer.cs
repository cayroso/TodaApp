using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.App.Models.Chats;
using Data.App.Models.Drivers;
using Data.App.Models.Riders;
using Data.App.Models.Users;
using Data.Constants;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.EntityFrameworkCore;

namespace Data.App.DbContext
{
    public static class AppDbContextInitializer
    {
        static Random _rnd = new Random((int)DateTime.UtcNow.Ticks);

        public static void Initialize(IdentityWebContext identityWebContext, AppDbContext ctx, IEnumerable<ProvisionUserRole> provisionUserRoles)
        {
            if (ctx.Users.Any())
                return;

            CreateRoles(ctx);

            CopyIdentityUserToApp(identityWebContext, ctx);

            ctx.SaveChanges();

            //GenerateUsersAndTeams(ctx, provisionUserRoles);
            //GenerateContacts(ctx);
        }

        static void CreateRoles(AppDbContext ctx)
        {
            var roles = ApplicationRoles.Items//.Where(e => e.Id != ApplicationRoles.System.Id)
                .Select(e => new Role
                {
                    RoleId = e.Id,
                    Name = e.Name,
                    //NormalizedName = e.Name.ToUpper()
                });

            ctx.AddRange(roles);
        }

        static void CopyIdentityUserToApp(IdentityWebContext identityWebContext, AppDbContext appDbContext)
        {
            var users = identityWebContext.Users.Include(e => e.UserInformation).ToList();

            var appUsers = new List<User>();

            users.ForEach(u =>
            {
                var appUser = new User
                {
                    UserId = u.Id,
                    FirstName = u.UserInformation.FirstName,
                    MiddleName = u.UserInformation.MiddleName,
                    LastName = u.UserInformation.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                };

                var userRoles = identityWebContext.UserRoles.Where(e => e.UserId == u.Id).ToList();

                appUser.UserRoles = userRoles.Select(e => new UserRole
                {
                    UserId = e.UserId,
                    RoleId = e.RoleId
                }).ToList();

                appUsers.Add(appUser);

                // if driver
                if (userRoles.Any(e => e.RoleId == ApplicationRoles.Driver.Id))
                {
                    var driver = new Driver
                    {
                        DriverId = appUser.UserId,
                    };

                    driver.Vehicles.Add(new Vehicle
                    {
                        DriverId = driver.DriverId,
                        PlateNumber = "Vehicle #1"
                    });
                    driver.Vehicles.Add(new Vehicle
                    {
                        DriverId = driver.DriverId,
                        PlateNumber = "Vehicle #2"
                    });

                    appDbContext.Add(driver);
                }

                // if rider
                if (userRoles.Any(e => e.RoleId == ApplicationRoles.Rider.Id))
                {
                    var rider = new Rider
                    {
                        RiderId = appUser.UserId,
                    };

                    appDbContext.Add(rider);
                }
            });

            appDbContext.AddRange(appUsers);
        }

        //static void GenerateUsersAndTeams(AppDbContext ctx, IEnumerable<ProvisionUserRole> provisionUserRoles)
        //{
        //    var teamId = NewId();

        //    var team = new Team
        //    {
        //        TeamId = teamId,
        //        Name = "Default",
        //        Description = "Default Team",
        //        DateCreated = DateTime.UtcNow,
        //        DateUpdated = DateTime.UtcNow,
        //        Members = provisionUserRoles.Select(e => new TeamMember
        //        {
        //            MemberId = e.User.UserId,
        //        }).ToList(),
        //        Chat = new Chat
        //        {
        //            ChatId = teamId,
        //            Title = "Chat: Default",
        //            Receivers = provisionUserRoles.Select(e => new ChatReceiver
        //            {
        //                ReceiverId = e.User.UserId
        //            }).ToList()
        //        }
        //    };

        //    ctx.Add(team);
        //}

        //static void GenerateContacts(AppDbContext ctx)
        //{
        //    var foo = GetNames().Select(e => new Contact
        //    {
        //        ContactId = NewId(),
        //        FirstName = e.Item1,
        //        MiddleName = "Middle",
        //        Email = $"{e.Item1}@{e.Item2}.com",
        //        ReferralSource = "facebook",
        //        Title = "Mr/Ms",
        //        Website = $"www.{e.Item1}.com",
        //        LastName = e.Item2,
        //        HomePhone = e.Item3,
        //        MobilePhone = "",
        //        BusinessPhone = "",
        //        Fax = "12345",
        //        Industry = "IT",
        //        Rating = 3,

        //        DateOfInitialContact = DateTime.UtcNow,
        //        Address = "102 Main Street",
        //        AnnualRevenue = 1.2M,
        //        DateCreated = DateTime.UtcNow,
        //        DateUpdated = DateTime.UtcNow,
        //        Status = Enums.EnumContactStatus.Lead,
        //    });

        //    ctx.AddRange(foo);
        //}

        static List<Tuple<string, string, string, string>> GetNames()
        {
            var list = new List<Tuple<string, string, string, string>>();

            //list.Add(new Tuple<string, string, string, string>("Juan", "Dela Cruz", "09191234567", "105 Paz Street, Barangay 11, Balayan, Batangas City"));
            //list.Add(new Tuple<string, string, string, string>("Pening", "Garcia", "09191234567", "101 Subdivision 202 Street, Barangay, Town, City, Philippines"));
            //list.Add(new Tuple<string, string, string, string>("Nadia", "Cole", "09191234567", "301 Main Street, Barangay 3, Balayan, Batangas City"));
            //list.Add(new Tuple<string, string, string, string>("Chino", "Pacia", "09191234567", "202 Subdivision 303 Street, Barangay, Town, City, Philippines"));
            //list.Add(new Tuple<string, string, string, string>("Vina", "Ruruth", "09191234567", "501 Main Street, Barangay 5, Balayan, Batangas City"));
            //list.Add(new Tuple<string, string, string, string>("Lina", "Mutac", "09191234567", "601 Main Street, Barangay 6, Balayan, Batangas City"));

            list.Add(new Tuple<string, string, string, string>("Pening", "Garcia", "09191234567", "101 Subdivision 202 Street, Barangay, Town, City, Philippines"));
            list.Add(new Tuple<string, string, string, string>("Chino", "Pacia", "09191234567", "202 Subdivision 303 Street, Barangay, Town, City, Philippines"));

            return list;
        }

        static string NewId()
        {
            return Guid.NewGuid().ToString().ToLower();
        }

        static string NewCouponCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = _rnd;
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }
    }
}
