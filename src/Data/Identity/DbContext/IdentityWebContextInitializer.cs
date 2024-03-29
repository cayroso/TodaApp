﻿using Cayent.Core.Data.Identity.Models.Users;
using Data.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Identity.DbContext
{
    public static class IdentityWebContextInitializer
    {
        public static void Initialize(IdentityWebContext ctx)
        {
            if (ctx.Users.Any())
                return;

            CreateRoles(ctx);

            CreateAdministrator(ctx);

            ctx.SaveChanges();
        }

        static void CreateRoles(IdentityWebContext ctx)
        {
            var roles = ApplicationRoles.Items.Select(e => new IdentityRole
            {
                Id = e.Id,
                Name = e.Name,
                NormalizedName = e.Name.ToUpper()
            });

            ctx.AddRange(roles);
        }

        static void CreateAdministrator(IdentityWebContext ctx)
        {
            //var tenant = new Tenant
            //{
            //    TenantId = "administrator",
            //    Name = "Administrator",
            //    Host = "Administrators Host",
            //    DatabaseConnectionString = @"Data Source=App_Data\TenantDB-DV.db;",
            //    PhoneNumber = "+639198262335",
            //    Email = "caydev2010@gmail.com",
            //    Address = "",

            //};

            var email1 = "user1@todaapp.com";
            var token1 = Guid.NewGuid().ToString();
            var admin1 = new IdentityWebUser
            {
                Id = "administrator1",
                UserName = email1,
                NormalizedUserName = email1.ToUpper(),

                Email = email1,
                NormalizedEmail = email1.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "+639198262335",
                PhoneNumberConfirmed = true,

                LockoutEnabled = false,
                LockoutEnd = null,
                PasswordHash = "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg==",
                SecurityStamp = "6SADCY3NMMLOHA2S26ZJCEWGHWSQUYRM",
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                //TenantId = tenant.TenantId,
                ConcurrencyStamp = token1,
                UserInformation = new UserInformation
                {
                    FirstName = "Kerina",
                    LastName = "Talandipa",
                    ConcurrencyToken = token1,
                    //Theme = "https://bootswatch.com/4/slate/bootstrap.min.css"
                }
            };

            var admin1Roles = ApplicationRoles.Items.Select(e => new IdentityUserRole<string>
            {
                UserId = admin1.Id,
                RoleId = e.Id
            }).ToList();

            ctx.AddRange(admin1);
            ctx.AddRange(admin1Roles);

            var email2 = "user2@todaapp.com";
            var admin2 = new IdentityWebUser
            {
                Id = "administrator2",
                UserName = email2,
                NormalizedUserName = email2.ToUpper(),

                Email = email2,
                NormalizedEmail = email2.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "+639198262335",
                PhoneNumberConfirmed = true,

                LockoutEnabled = false,
                LockoutEnd = null,
                PasswordHash = "AQAAAAEAACcQAAAAEKGIieH17t5bYXa5tUfxRwN9UIEwApTKbQBRaUtIHplIUG2OfYxvBS8uvKy5E2Stsg==",
                SecurityStamp = "6SADCY3NMMLOHA2S26ZJCEWGHWSQUYRM",
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                //TenantId = tenant.TenantId,
                ConcurrencyStamp = token1,
                UserInformation = new UserInformation
                {
                    FirstName = "Tina",
                    LastName = "Moran",
                    ConcurrencyToken = token1,
                    //Theme = "https://bootswatch.com/4/sketchy/bootstrap.min.css"
                }
            };
            var admin2Roles = ApplicationRoles.Items.Select(e => new IdentityUserRole<string>
            {
                UserId = admin2.Id,
                RoleId = e.Id
            }).ToList();

            ctx.AddRange(admin2);
            ctx.AddRange(admin2Roles);
        }

    }
}
