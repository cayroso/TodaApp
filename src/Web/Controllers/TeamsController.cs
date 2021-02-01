using App.CQRS;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.App.Models.Teams;
using Data.Common;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TeamsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;

        public TeamsController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] AppDbContext dbContext, string c, int p, int s, string sf, int so)
        {
            var sql = from t in dbContext.Teams.AsNoTracking()

                      where string.IsNullOrWhiteSpace(c)
                            || EF.Functions.Like(t.Name, $"%{c}%")
                            || EF.Functions.Like(t.Description, $"%{c}%")

                      //orderby contact.DateUpdated descending, contact.CreatedBy descending

                      select new
                      {
                          t.TeamId,
                          t.Name,
                          t.Description,
                          t.DateCreated,
                          t.DateUpdated,
                          Members = t.Members.Select(e => new
                          {
                              UserId = e.MemberId,
                              Name = e.Member.FirstLastName,
                              UrlProfilePicture = e.Member.Image.Url
                          })
                      };

            var dto = await sql.ToPagedItemsAsync(p, s);

            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromServices] AppDbContext dbContext, [FromBody] AddTeamInfo info)
        {
            var teamId = GuidStr();

            var team = new Team
            {
                TeamId = teamId,
                Name = info.Name,
                Description = info.Description,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,

                Chat = new Chat
                {
                    ChatId = teamId,
                    Title = $"Chat: {info.Name}",
                }
            };

            team.Members.Add(new TeamMember
            {
                MemberId = UserId,
            });

            //  add this user as member
            team.Chat.Receivers.Add(new ChatReceiver
            {
                ReceiverId = UserId
            });

            await dbContext.AddRangeAsync(team);

            await dbContext.SaveChangesAsync();


            return Ok();
        }

        [HttpPost("add-member")]
        public async Task<IActionResult> PostAddMember(
            [FromServices] AppDbContext dbContext,
            [FromServices] ChatService chatService,
            [FromBody] AddTeamMemberInfo info)
        {
            var team = await dbContext.Teams.Include(e => e.Chat).Include(e => e.Members).FirstOrDefaultAsync(e => e.TeamId == info.TeamId);

            team.ThrowIfNull();

            if (!team.Members.Any(e => e.MemberId == info.MemberId))
            {
                team.Members.Add(new TeamMember
                {
                    MemberId = info.MemberId
                });

                team.Chat.Receivers.Add(new ChatReceiver
                {
                    ReceiverId = info.MemberId,
                });


                team.DateUpdated = DateTime.Now;

                var user = await dbContext.Users.FirstAsync(e => e.UserId == info.MemberId);

                await chatService.AddChatMessage(team.Chat.ChatId, user.UserId, $"{user.FirstLastName} was added to the group.", EnumChatMessageType.System);

                await dbContext.SaveChangesAsync();


            }

            return Ok();
        }

        [HttpDelete("{teamId}/remove-member/{memberId}")]
        public async Task<IActionResult> DeleteMember(
            [FromServices] AppDbContext dbContext,
            [FromServices] ChatService chatService,
            string teamId, string memberId)
        {
            var team = await dbContext.Teams
                            .Include(e => e.Chat)
                                .ThenInclude(e => e.Receivers)
                            .Include(e => e.Members)
                            .FirstOrDefaultAsync(e => e.TeamId == teamId);

            team.ThrowIfNull();

            var tm = team.Members.FirstOrDefault(e => e.MemberId == memberId);

            if (tm != null)
            {
                dbContext.Remove(tm);
                team.DateUpdated = DateTime.Now;

                var chatMem = team.Chat.Receivers.FirstOrDefault(e => e.ReceiverId == memberId);

                if (chatMem != null)
                {
                    dbContext.Remove(chatMem);
                }

                var user = await dbContext.Users.FirstAsync(e => e.UserId == memberId);

                await chatService.AddChatMessage(team.Chat.ChatId, user.UserId, $"{user.FirstLastName} was removed from the group.", EnumChatMessageType.System);

                await dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromServices] AppDbContext dbContext, string id)
        {
            var sql = from t in dbContext.Teams.AsNoTracking()

                      where t.TeamId == id

                      select new
                      {
                          t.Name,
                          t.Description,
                          t.DateCreated,
                          t.DateUpdated,
                          Token = t.ConcurrencyToken,
                          Members = t.Members.Select(e => new
                          {
                              e.Member.UserId,
                              e.Member.FirstName,
                              e.Member.LastName
                          })
                      };

            var dto = await sql.FirstOrDefaultAsync();

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromServices] AppDbContext dbContext, string id)
        {
            var team = await dbContext.Teams
                .Include(e => e.Chat)
                    .ThenInclude(e => e.Messages)
                .Include(e => e.Chat)
                    .ThenInclude(e => e.Receivers)
                .FirstOrDefaultAsync(e => e.TeamId == id);

            team.ThrowIfNull();

            dbContext.RemoveRange(team.Chat.Messages);
            dbContext.RemoveRange(team.Chat.Receivers);
            dbContext.RemoveRange(team.Chat);
            dbContext.RemoveRange(team);

            await dbContext.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("my-teams")]
        public async Task<IActionResult> GetMyTeams([FromServices] AppDbContext dbContext, string c, int p, int s, string sf, int so)
        {
            var sql = from tm in dbContext.TeamMembers.AsNoTracking()

                      where tm.MemberId == UserId

                      select new
                      {
                          tm.Team.TeamId,
                          tm.Team.Name,
                          tm.Team.Description,
                          tm.Team.DateCreated,
                          tm.Team.DateUpdated,
                          Token = tm.Team.ConcurrencyToken,
                          Members = tm.Team.Members.Select(e => new
                          {
                              e.Member.UserId,
                              UrlProfilePicture = e.Member.Image.Url,
                              e.Member.FirstName,
                              e.Member.LastName
                          })
                      };

            var dto = await sql.ToPagedItemsAsync(p, s);

            return Ok(dto);
        }
    }

    public class AddTeamInfo
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AddTeamMemberInfo
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string MemberId { get; set; }

    }

    public class RemoveTeamMemberInfo
    {
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string MemberId { get; set; }

    }
}
