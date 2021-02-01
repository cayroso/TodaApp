using App.CQRS;
using App.CQRS.Users.Common.Queries.Query;
using Data.App.DbContext;
using Data.App.Models.Users;
using Data.Common;
using Data.Constants;
using Data.Identity.DbContext;
using Data.Identity.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Web.Areas.Administrator.Models;
using Web.Controllers;

namespace Web.Areas.Administrator.Controllers
{
    [Authorize(Policy = ApplicationRoles.AdministratorRoleName)]
    [ApiController]
    [Route("api/administrators/[controller]")]
    [Produces("application/json")]
    public class UsersController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public UsersController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] AppDbContext dbContext, string c, int p, int s, string sf, int so)
        {
            var sql = from u in dbContext.Users.AsNoTracking()

                      where string.IsNullOrWhiteSpace(c)
                            || EF.Functions.Like(u.FirstName, $"%{c}%")
                            || EF.Functions.Like(u.LastName, $"%{c}%")

                            || EF.Functions.Like(u.Email, $"%{c}%")
                            || EF.Functions.Like(u.PhoneNumber, $"%{c}%")

                      //join tm in dbContext.TeamMembers on u.UserId equals tm.MemberId into tms
                      //from tm in tms.DefaultIfEmpty()
                      //orderby contact.DateUpdated descending, contact.CreatedBy descending

                      select new
                      {
                          u.UserId,
                          UrlProfilePicture = u.Image.Url,
                          u.Email,
                          u.PhoneNumber,
                          u.FirstName,
                          u.MiddleName,
                          u.LastName,
                          Roles = u.UserRoles.Select(e => e.Role.Name),
                          Teams = u.TeamMembers.Select(e => e.Team.Name)
                      };

            var dto = await sql.ToPagedItemsAsync(p, s);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetUserByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetUserByIdQuery, GetUserByIdQuery.User>(query);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] SignInManager<IdentityWebUser> signInManager,
            [FromServices] UserManager<IdentityWebUser> userManager,
            [FromServices] IdentityWebContext identityWebContext,
            [FromServices] AppDbContext appDbContext,
            [FromServices] IEmailSender emailSender,
            [FromBody] AddUserInfo info)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityWebUser
                {
                    Id = GuidStr(),
                    TenantId = TenantId,
                    UserName = info.Email,
                    Email = info.Email,
                    PhoneNumber = info.PhoneNumber,
                    ConcurrencyStamp = GuidStr()
                };

                var userInfo = new UserInformation
                {
                    UserId = user.Id,
                    FirstName = info.FirstName,
                    LastName = info.LastName,
                    ConcurrencyToken = GuidStr()
                };

                var userRole = new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = ApplicationRoles.Member.Id
                };

                await identityWebContext.AddRangeAsync(userInfo);
                await identityWebContext.AddRangeAsync(userRole);

                var result = await userManager.CreateAsync(user, info.Password);


                if (result.Succeeded)
                {
                    // add to tenant db
                    var appUser = new Data.App.Models.Users.User
                    {
                        UserId = user.Id,
                        FirstName = info.FirstName,
                        LastName = info.LastName,
                        PhoneNumber = info.PhoneNumber,
                        Email = info.Email,
                    };
                    var appUserRole = new Data.App.Models.Users.UserRole
                    {
                        UserId = userRole.UserId,
                        RoleId = userRole.RoleId
                    };

                    //  create chat group

                    await appDbContext.AddRangeAsync(appUser, appUserRole);

                    await identityWebContext.SaveChangesAsync();
                    await appDbContext.SaveChangesAsync();

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = string.Empty },
                        protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(info.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return Ok(user.Id);
                }

                var errors = string.Join(",", result.Errors.Select(e => e.Description));

                return BadRequest(errors);

            }

            return BadRequest();
        }

        [HttpPost("manage-roles")]
        public async Task<IActionResult> AddUserRole(
            [FromServices] IdentityWebContext identityWebContext,
            [FromServices] AppDbContext appDbContext,
            [FromBody] ManageUserRoleInfo info)
        {
            var appUser = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == info.UserId);

            appUser.ThrowIfNull();

            var currentUserRoles = await identityWebContext.UserRoles.Where(e => e.UserId == info.UserId).ToListAsync();

            //  check if this is the last administrator
            var adminCount = await identityWebContext.UserRoles.Where(e => e.RoleId == ApplicationRoles.Administrator.Id).CountAsync();

            if (adminCount <= 1)
            {
                if (currentUserRoles.Any(e => e.RoleId == ApplicationRoles.Administrator.Id) && !info.RoleIds.Any(e => e == ApplicationRoles.Administrator.Id))
                {
                    return BadRequest("Removing the last administrator is not allowed.");
                }
            }

            if (currentUserRoles.Any())
            {
                identityWebContext.RemoveRange(currentUserRoles);
            }

            identityWebContext.UserRoles.AddRange(info.RoleIds.Select(e => new IdentityUserRole<string>
            {
                UserId = info.UserId,
                RoleId = e
            }));

            await identityWebContext.SaveChangesAsync();


            var currentAppUserRoles = await appDbContext.UserRoles.Where(e => e.UserId == info.UserId).ToListAsync();

            if (currentAppUserRoles.Any())
            {
                appDbContext.UserRoles.RemoveRange(currentAppUserRoles);
            }

            appDbContext.UserRoles.AddRange(info.RoleIds.Select(e => new UserRole
            {
                UserId = info.UserId,
                RoleId = e
            }));

            await appDbContext.SaveChangesAsync();


            return Ok();
        }

        public class ManageUserRoleInfo
        {
            [Required]
            [DisplayName("User")]
            public string UserId { get; set; }
            [Required]
            [DisplayName("Roles")]
            public List<string> RoleIds { get; set; }
        }

    }
}
