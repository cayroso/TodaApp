using App.CQRS.Navbar.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Queries;
using Cayent.Core.Data.Fileuploads;
using Data.App.DbContext;
using Data.Identity.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;

        public AccountsController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpPut("change-theme")]
        public async Task<IActionResult> ChangeThemeAsync([FromServices] IdentityWebContext webContext, string theme)
        {
            var userInfo = await webContext.UserInformations.FirstOrDefaultAsync(e => e.UserId == UserId);

            if (userInfo != null)
            {
                userInfo.Theme = theme;

                await webContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("unread-chats")]
        public async Task<IActionResult> GetNavbarUnreadChats(CancellationToken cancellationToken)
        {
            var query = new GetUnreadChatsQuery("", TenantId, UserId, 1, 10);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetUnreadChatsQuery, Paged<GetUnreadChatsQuery.ChatMessage>>(query, cancellationToken);

            return Ok(dto.Items);
        }

        [HttpPut]
        public async Task<IActionResult> Put(
            [FromServices] IdentityWebContext webDbContext,
            [FromServices] AppDbContext appDbContext,
            [FromBody] UpdateUserInfo info
            )
        {
            var user = await webDbContext.UserInformations.FirstOrDefaultAsync(p => p.UserId == UserId);
            var appUser = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == UserId);

            if (user == null || appUser == null)
            {
                return NotFound("Account not found");
            }

            if (user.FirstName != info.FirstName)
                user.FirstName = appUser.FirstName = info.FirstName;

            if (user.MiddleName != info.MiddleName)
                user.MiddleName = appUser.MiddleName = info.MiddleName;

            if (user.LastName != info.LastName)
                user.LastName = appUser.LastName = info.LastName;

            await webDbContext.SaveChangesAsync();
            await appDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("profile-picture")]
        public async Task<IActionResult> UploadProfilePictureAsync(
            [FromServices] IdentityWebContext webDbContext,
            [FromServices] AppDbContext appDbContext)
        {
            var user = await webDbContext.UserInformations.FirstOrDefaultAsync(p => p.UserId == UserId);
            var appUser = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == UserId);

            if (user == null || appUser == null)
            {
                return NotFound("Account not found");
            }

            //  delete profile picture if existing
            var deleteProfilePicture = await appDbContext.FileUploads.FirstOrDefaultAsync(p => p.FileUploadId == user.ImageId);

            if (deleteProfilePicture != null)
            {
                appDbContext.Remove(deleteProfilePicture);
            }

            var files = Request.Form.Files;

            if (files.Count > 0)
            {
                var file = files.First();

                var bytes = new byte[file.Length];

                using (var stream = file.OpenReadStream())
                {
                    stream.Read(bytes);
                }

                var fileUploadId = GuidStr();

                var fileUpload = new FileUpload
                {
                    FileUploadId = fileUploadId,
                    FileName = file.FileName,
                    ContentDisposition = file.ContentDisposition,
                    ContentType = file.ContentType,
                    Content = bytes,
                    Length = file.Length,
                    DateCreated = DateTime.UtcNow,
                    //Url = $"api/files/{TenantId}/{fileUploadId}",
                    Url = $"api/files/{fileUploadId}",
                };

                //  update profile picture of this account
                user.ImageId = fileUploadId;
                appUser.ImageId = fileUploadId;

                //  save file upload
                await appDbContext.AddAsync(fileUpload);

                await webDbContext.SaveChangesAsync();
                await appDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost("updatePhoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumber(
            [FromServices] IdentityWebContext webDbContext,
            [FromServices] AppDbContext appDbContext,
            [FromBody] UpdatePhoneNumberInfo info)
        {
            var user = await webDbContext.Users.FirstOrDefaultAsync(p => p.Id == UserId);
            var appUser = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == UserId);

            if (user == null || appUser == null)
            {
                return NotFound("Account not found");
            }

            if (user.PhoneNumber == info.PhoneNumber)
            {
                return BadRequest("New and existing phone number are the same.");
            }

            user.PhoneNumber = info.PhoneNumber;
            user.PhoneNumberConfirmed = false;

            appUser.PhoneNumber = info.PhoneNumber;

            await webDbContext.SaveChangesAsync();
            await appDbContext.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> Get(
            [FromServices] IdentityWebContext webDbContext,
            [FromServices] AppDbContext appDbContext)
        {
            var sqlUser = from u in webDbContext.Users.AsNoTracking().Include(e => e.UserInformation)
                          where u.Id == UserId
                          select new
                          {
                              u.Email,
                              u.PhoneNumber,

                              u.UserInformation.FirstName,
                              u.UserInformation.MiddleName,
                              u.UserInformation.LastName,
                              u.UserInformation.Theme,

                              ProfilePictureId = u.UserInformation.ImageId
                          };

            var dtoUser = await sqlUser.FirstOrDefaultAsync();

            return Ok(dtoUser);
        }


        public class UpdateUserInfo
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
        }

        public class UpdatePhoneNumberInfo
        {
            public string AccountId { get; set; }
            public string PhoneNumber { get; set; }
        }
    }


}
