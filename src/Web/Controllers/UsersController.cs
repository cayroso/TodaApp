using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;

        public UsersController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Get([FromServices] AppDbContext dbContext)
        {
            var sql = from u in dbContext.Users.AsNoTracking()

                      orderby u.LastName
                      orderby u.FirstName

                      select new
                      {
                          Id = u.UserId,
                          Name = u.FirstLastName,
                      };

            var dto = await sql.ToListAsync();

            return Ok(dto);
        }

    }

}
