using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Web.Code;
using System.Security.Claims;

namespace Web.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.Claims.Count(e => e.Type == ClaimTypes.Role) == 1)
            {
                var role = User.Claims.First(e => e.Type == ClaimTypes.Role).Value;

                return RedirectToPage("/Index", new { area = role });
            }

            return Page();
        }
    }
}
