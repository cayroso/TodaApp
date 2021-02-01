﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.App.Models.Chats;
using Data.App.Models.Contacts;
using Data.App.Models.Teams;
using Data.Identity.Models.Users;
using Data.Providers;

namespace Data.App.DbContext
{
    public static class AppDbContextInitializer
    {
        static Random _rnd = new Random((int)DateTime.UtcNow.Ticks);

        public static void Initialize(AppDbContext ctx, IEnumerable<ProvisionUserRole> provisionUserRoles)
        {
            ctx.SaveChanges();

            GenerateUsersAndTeams(ctx, provisionUserRoles);
            GenerateContacts(ctx);
        }

        static void GenerateUsersAndTeams(AppDbContext ctx, IEnumerable<ProvisionUserRole> provisionUserRoles)
        {
            var teamId = NewId();

            var team = new Team
            {
                TeamId = teamId,
                Name = "Default",
                Description = "Default Team",
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                Members = provisionUserRoles.Select(e => new TeamMember
                {
                    MemberId = e.User.UserId,
                }).ToList(),
                Chat = new Chat
                {
                    ChatId = teamId,
                    Title = "Chat: Default",
                    Receivers = provisionUserRoles.Select(e => new ChatReceiver
                    {
                        ReceiverId = e.User.UserId
                    }).ToList()
                }
            };

            ctx.Add(team);
        }

        static void GenerateContacts(AppDbContext ctx)
        {
            var foo = GetNames().Select(e => new Contact
            {
                ContactId = NewId(),
                FirstName = e.Item1,
                MiddleName = "Middle",
                Email = $"{e.Item1}@{e.Item2}.com",
                ReferralSource = "facebook",
                Title = "Mr/Ms",
                Website = $"www.{e.Item1}.com",
                LastName = e.Item2,
                HomePhone = e.Item3,
                MobilePhone = "",
                BusinessPhone = "",
                Fax = "12345",
                Industry = "IT",
                Rating = 3,

                DateOfInitialContact = DateTime.UtcNow,
                Address = "102 Main Street",
                AnnualRevenue = 1.2M,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                Status = Enums.EnumContactStatus.Lead,
            });

            ctx.AddRange(foo);
        }

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
