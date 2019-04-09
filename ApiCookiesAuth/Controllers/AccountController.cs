using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiCookiesAuth.Data.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiCookiesAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("SignInWithGoogle")]
        public IActionResult SignInWithGoogle()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, "Google");
        }

        [HttpGet]
        [Route("SignInWithFacebook")]
        public IActionResult SignInWithFacebook()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, "Facebook");
        }

        [HttpGet]
        [Route("SignInWithTwitter")]
        public IActionResult SignInWithTwitter()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Twitter", Url.Action(nameof(HandleExternalLogin)));
            return Challenge(authenticationProperties, "Twitter");
        }

        public async Task<IActionResult> HandleExternalLogin()
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false); 

            if (!result.Succeeded)
            {
                var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? 
                            externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ??
                           externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

                var appUser = await _userManager.FindByEmailAsync(email);
                
                if (appUser == null)
                {
                    appUser = new AppUser {
                        UserName = name,
                        Email = email,
                        EmailConfirmed = true                 
                    };

                    var createResult = await _userManager.CreateAsync(appUser);
                    if (!createResult.Succeeded)
                        throw new Exception(createResult.Errors.Select(e => e.Description).Aggregate((errors, error) => $"{errors}, {error}"));
                }
                await _userManager.AddLoginAsync(appUser, externalLoginInfo);
                var newUserClaims = externalLoginInfo.Principal.Claims.Append(new Claim("userId", appUser.Id.ToString()));
                await _userManager.AddClaimsAsync(appUser, newUserClaims);
                await _signInManager.SignInAsync(appUser, isPersistent: false);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
            return Redirect("/");                        
        }

        [HttpGet]
        [Route("IsUserAuthenticated")]
        public async Task<bool> IsUserAuthenticated()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            return _signInManager.IsSignedIn(info.Principal);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

    }
}