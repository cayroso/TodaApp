using App.CQRS;
using App.CQRS.Chats.Common.Commands.Command;
using App.CQRS.Chats.Common.Queries.Query;
using App.Services;
using Data.App.Models.Chats;
using Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChatController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;

        public ChatController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet("")]
        public async Task<IActionResult> SearchChat(string c, int p, int s, string sf, int so)
        {
            var query = new SearchChatQuery("", TenantId, UserId, "You", c, p, s, sf, so);
            var dto = await _queryHandlerDispatcher.HandleAsync<SearchChatQuery, Paged<SearchChatQuery.Chat>>(query);

            return Ok(dto);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> SearchUnreadChat([FromServices] ChatService chatService, string criteria, int pageIndex = 1, int pageSize = 10)
        {
            var paginated = await chatService.SearchUnreadChats(UserId, criteria, pageIndex, pageSize);

            return Ok(paginated);
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat([FromServices] ChatService chatService, string chatId)
        {
            var query = new GetChatHeaderByIdQuery("", TenantId, chatId, UserId, "You");
            var dto = await _queryHandlerDispatcher.HandleAsync<GetChatHeaderByIdQuery, GetChatHeaderByIdQuery.ChatHeader>(query);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> SearchChatMessages(string chatId, string criteria, int pageIndex = 1, int pageSize = 10)
        {
            var query = new SearchChatMessagesQuery("", TenantId, UserId, chatId, criteria, pageIndex, pageSize);
            var dto = await _queryHandlerDispatcher.HandleAsync<SearchChatMessagesQuery, Paged<SearchChatMessagesQuery.ChatMessage>>(query);

            return Ok(dto);
        }

        [HttpPost("message")]
        public async Task<IActionResult> AddChatMessage(
            [FromServices] ChatService chatService,
            [FromBody] AddChatMessageInfo info)
        {
            await chatService.AddChatMessage(info.ChatId, UserId, info.Content, EnumChatMessageType.User);

            return Ok();
        }

        [HttpPost("add/{memberId}")]
        public async Task<IActionResult> CreateChat([FromServices] ICommandHandlerDispatcher commandHandlerDispatcher, string memberId)
        {
            var query = new GetChatByMemberIdQuery("", TenantId, UserId, UserId, memberId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetChatByMemberIdQuery, GetChatByMemberIdQuery.Chat>(query);

            if (dto != null)
                return Ok(dto.ChatId);

            var chatId = GuidStr();
            var command = new AddChatCommand("", TenantId, UserId, chatId, UserId, memberId);
            await commandHandlerDispatcher.HandleAsync(command);

            return Ok(chatId);
        }

        [HttpPost("{chatId}/markAsRead")]
        public async Task<IActionResult> MarkChatAsRead([FromServices] ChatService chatService, string chatId)
        {
            await chatService.MarkChatAsRead(UserId, chatId);

            return Ok();
        }

        [HttpPost("{chatId}/remove/{memberId}")]
        public async Task<IActionResult> RemoveChatMember([FromServices] ChatService chatService, string chatId, string memberId)
        {
            await chatService.RemoveChatMember(chatId, memberId);

            return Ok();
        }

        [HttpPost("{chatId}/remove")]
        public async Task<IActionResult> RemoveFromChat([FromServices] ChatService chatService, string chatId)
        {
            await chatService.RemoveChatMember(chatId, UserId);

            return Ok();
        }
    }

    public class AddChatMessageInfo
    {
        public string ChatId { get; set; }
        public string Content { get; set; }
    }
}
