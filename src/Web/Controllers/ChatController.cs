using App.Services;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Commands;
using Cayent.Core.CQRS.Common.Chats.Commands.Command;
using Cayent.Core.CQRS.Common.Chats.Queries.Query;
using Cayent.Core.CQRS.Queries;
using Cayent.Core.CQRS.Services;
using Cayent.Core.Data.Chats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
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
        public async Task<IActionResult> SearchChat(CancellationToken cancellationToken, string c, int p, int s, string sf, int so)
        {
            var query = new SearchChatQuery("", TenantId, UserId, "You", c, p, s, sf, so);
            var dto = await _queryHandlerDispatcher.HandleAsync<SearchChatQuery, Paged<SearchChatQuery.Chat>>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> SearchUnreadChat([FromServices] ChatService chatService, string criteria, int pageIndex = 1, int pageSize = 10)
        {
            var paginated = await chatService.SearchUnreadChats(UserId, criteria, pageIndex, pageSize);

            return Ok(paginated);
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(CancellationToken cancellationToken, string chatId)
        {
            var query = new GetChatHeaderByIdQuery("", TenantId, chatId, UserId, "You");
            var dto = await _queryHandlerDispatcher.HandleAsync<GetChatHeaderByIdQuery, GetChatHeaderByIdQuery.ChatHeader>(query, cancellationToken);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> SearchChatMessages(CancellationToken cancellationToken, string chatId, string criteria, int pageIndex = 1, int pageSize = 10)
        {
            var query = new SearchChatMessagesQuery("", TenantId, UserId, chatId, criteria, pageIndex, pageSize);
            var dto = await _queryHandlerDispatcher.HandleAsync<SearchChatMessagesQuery, Paged<SearchChatMessagesQuery.ChatMessage>>(query, cancellationToken);

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
        public async Task<IActionResult> CreateChat(CancellationToken cancellationToken, [FromServices] ICommandHandlerDispatcher commandHandlerDispatcher, string memberId)
        {
            var query = new GetChatByMemberIdQuery("", TenantId, UserId, UserId, memberId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetChatByMemberIdQuery, GetChatByMemberIdQuery.Chat>(query, cancellationToken);

            if (dto != null)
                return Ok(dto.ChatId);

            var chatId = GuidStr();
            var command = new AddChatCommand("", TenantId, UserId, chatId, UserId, memberId);
            await commandHandlerDispatcher.HandleAsync(command, cancellationToken);

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
