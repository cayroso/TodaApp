using App.CQRS;
using App.CQRS.Contacts.Common.Queries.Query;
using App.CQRS.Documents.Common.Queries.Query;
using Data.App.DbContext;
using Data.App.Models.Contacts;
using Data.App.Models.Documents;
using Data.App.Models.FileUploads;
using Data.Common;
using Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.Member.Models;
using Web.Controllers;

namespace Web.Areas.Member.Controllers
{
    [Authorize(Policy = ApplicationRoles.MemberRoleName)]
    [ApiController]
    [Route("api/members/[controller]")]
    [Produces("application/json")]
    public class DocumentsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public DocumentsController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] AppDbContext appDbContext, string c, int p, int s, string sf, int so)
        {
            var sql = from doc in appDbContext.Documents
                      orderby doc.DateUpdated descending, doc.DateCreated descending

                      where string.IsNullOrWhiteSpace(c)
                            || EF.Functions.Like(doc.Name, $"%{c}%")
                            || EF.Functions.Like(doc.Description, $"%{c}%")
                            || EF.Functions.Like(doc.FileUpload.FileName, $"%{c}%")
                            || EF.Functions.Like(doc.FileUpload.Url, $"%{c}%")
                      select new
                      {
                          doc.DocumentId,
                          doc.Name,
                          doc.Description,
                          doc.DateCreated,
                          doc.DateUpdated,

                          UploadedById = doc.UploadedById,
                          UploadedBy = doc.UploadedBy.FirstLastName,

                          doc.FileUpload.Url,
                          doc.FileUpload.FileName,
                          doc.FileUpload.ContentType,
                          doc.FileUpload.Length
                      };

            var dto = await sql.ToPagedItemsAsync(p, s);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromServices] AppDbContext appDbContext, string id)
        {
            var query = new GetDocumentByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetDocumentByIdQuery, GetDocumentByIdQuery.Document>(query);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromServices] AppDbContext appDbContext)
        {
            var infoFile = Request.Form.Files.FirstOrDefault(e => e.Name == "info");
            var attachmentFile = Request.Form.Files.FirstOrDefault(e => e.Name == "files");

            if (infoFile != null && infoFile.Length > 0 && attachmentFile != null)
            {
                var streamReader = new StreamReader(infoFile.OpenReadStream());

                var infoJson = streamReader.ReadToEnd();

                var info = JsonConvert.DeserializeObject<AddDocumentInfo>(infoJson);

                var bytes = new byte[attachmentFile.Length];

                using (var stream = attachmentFile.OpenReadStream())
                {
                    stream.Read(bytes);
                }

                var fileUploadId = GuidStr();

                var fileUpload = new FileUpload
                {
                    FileUploadId = fileUploadId,
                    FileName = attachmentFile.FileName,
                    ContentDisposition = attachmentFile.ContentDisposition,
                    ContentType = attachmentFile.ContentType,
                    Content = bytes,
                    Length = attachmentFile.Length,
                    DateCreated = DateTime.UtcNow,
                    Url = $"api/files/{TenantId}/{fileUploadId}",
                };

                var data = new Document
                {
                    DocumentId = fileUpload.FileUploadId,
                    Name = info.Name,
                    Description = info.Description,
                    UploadedById = UserId,
                    FileUpload = fileUpload,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                };

                await appDbContext.AddAsync(data);

                await appDbContext.SaveChangesAsync();

                return Ok(data.DocumentId);
            }

            return BadRequest();
        }

        [HttpDelete("{id}/{token}")]
        public async Task<IActionResult> Delete([FromServices] AppDbContext appDbContext, string id, string token)
        {

            if (ModelState.IsValid)
            {
                var data = await appDbContext.Documents.Include(e => e.FileUpload).Include(e => e.DocumentAccessHistories)
                    .FirstOrDefaultAsync(e => e.DocumentId == id);

                data.ThrowIfNullOrAlreadyUpdated(token, GuidStr());

                //appDbContext.RemoveRange(data.DocumentAccessHistories);

                //appDbContext.Remove(data.FileUpload);

                appDbContext.Remove(data);

                await appDbContext.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();

        }
    }
}
