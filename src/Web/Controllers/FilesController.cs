using Data.App.DbContext;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FilesController : BaseController
    {
        [HttpGet("{id}")]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetFileAsStreamAsync([FromServices] AppDbContext appDbContext, string id)
        {

            var data = await appDbContext.FileUploads
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.FileUploadId == id);

            if (data == null)
                return NotFound();

            var stream = new MemoryStream(data.Content.Length);

            stream.Write(data.Content, 0, data.Content.Length);
            stream.Position = 0;

            return File(stream, data.ContentType, data.FileName, true);
        }

        [HttpGet("{tenantId}/{id}")]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetFileAsStreamAsync(
            [FromServices] IConfiguration configuration,
            [FromServices] DbContextOptions<AppDbContext> options,
            [FromServices] IdentityWebContext dbContext, string tenantId, string id)
        {

            var tenant = await dbContext
                .Tenants
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.TenantId == tenantId);

            if (tenant == null)
                return NotFound();

            var dummyProvider = new DummyTenantProvider(tenant);
            var appDbContext = new AppDbContext(options, configuration, dummyProvider);

            var data = await appDbContext.FileUploads
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.FileUploadId == id);

            if (data == null)
                return NotFound();

            if (data.Content == null)
                return NotFound();

            var stream = new MemoryStream(data.Content.Length);

            stream.Write(data.Content, 0, data.Content.Length);
            stream.Position = 0;

            return File(stream, data.ContentType, data.FileName, true);
        }
    }
}
