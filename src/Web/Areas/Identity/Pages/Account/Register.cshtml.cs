using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Data.Identity.Models.Users;
using Data.Identity.DbContext;
using Microsoft.Extensions.Configuration;
using Data.Providers;
using Data.Identity.Models;
using Data.Constants;

namespace Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityWebUser> _signInManager;
        private readonly UserManager<IdentityWebUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityWebUser> userManager,
            SignInManager<IdentityWebUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Business Name")]
            public string BusinessName { get; set; }
            [Required]
            [Display(Name = "Business Address")]
            public string BusinessAddress { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(
            [FromServices] IdentityWebContext identityWebContext,
            [FromServices] IConfiguration configuration,
            [FromServices] IAppDbContextFactory appDbContextFactory,
            string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (!Input.BusinessName.All(p => char.IsLetterOrDigit(p) || char.IsWhiteSpace(p)))
                {
                    ModelState.AddModelError("", "Business Name should be letters, numbers, or spaces only");

                    return Page();
                }
                var businessName = Input.BusinessName.Replace(" ", string.Empty);
                if (businessName.Length <= 0)
                {
                    ModelState.AddModelError("", "Please, dont try that.");

                    return Page();
                }

                var tenantId = Guid.NewGuid().ToString();

                //  take 8 chars from business name
                if (businessName.Length < 8)
                {
                    businessName += "".PadRight(8 - businessName.Length, '0');
                }

                var bu = businessName.Substring(0, 8).ToUpper();
                if (bu.Length < 8)
                {
                    bu = bu.PadRight(8 - bu.Length, '0');
                }
                bu = $"{bu}-{tenantId}";

                var serverName = configuration.GetValue<string>("AppSettings:SQLServer");
                var connString = appDbContextFactory.GenerateConnectionString(serverName, bu.ToUpper());
                //  add tenant
                var tenant = new Tenant
                {
                    TenantId = tenantId,
                    Host = bu,
                    Name = Input.BusinessName,
                    DatabaseConnectionString = connString,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    Address = Input.BusinessAddress
                };

                var userId = Guid.NewGuid().ToString();
                var token = Guid.NewGuid().ToString();

                var userRoles = new List<IdentityUserRole<string>>(new[]{
                    new IdentityUserRole<string> {
                        UserId = userId,
                        RoleId = ApplicationRoles.Administrator.Id
                    },
                    new IdentityUserRole<string> {
                        UserId = userId,
                        RoleId = ApplicationRoles.Driver.Id
                    },
                    new IdentityUserRole<string> {
                        UserId = userId,
                        RoleId = ApplicationRoles.Commuter.Id
                    },
                });

                var user = new IdentityWebUser
                {
                    Id = userId,
                    TenantId = tenantId,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    ConcurrencyStamp = token
                };

                var userInfo = new UserInformation
                {
                    //UserInformationId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    //PhoneNumber = Input.PhoneNumber,
                    //Default = true,
                    ConcurrencyToken = token
                };

                tenant.Users.Add(user);

                await identityWebContext.AddRangeAsync(tenant, userInfo);
                await identityWebContext.AddRangeAsync(userRoles);

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    var provisionUserRole = new ProvisionUserRole
                    {
                        User = new Data.App.Models.Users.User
                        {
                            UserId = user.Id,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            Email = Input.Email,
                            PhoneNumber = Input.PhoneNumber,
                            ConcurrencyToken = token
                        },
                        RoleIds = userRoles.Select(e => e.RoleId).ToList()
                    };

                    appDbContextFactory.Provision(tenant, new List<ProvisionUserRole>(new[] { provisionUserRole }), true);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
