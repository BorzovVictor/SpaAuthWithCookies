using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ApiCookiesAuth.Data;
using ApiCookiesAuth.Data.Entities;
using ApiCookiesAuth.Data.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCookiesAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        //private readonly SignInManager<AppUser> _signInManager;
        //private readonly UserManager<AppUser> _userManager;
        private SimpleContext _db;
        public AccountController(SimpleContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("SignInWithGoogle")]
        public IActionResult SignInWithGoogle()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                Items = {["LoginProvider"] = "Google"},
                RedirectUri = Url.Action(nameof(HandleExternalSimpleLogin))
            };
            return Challenge(authenticationProperties, "Google");
        }

        [HttpGet]
        [Route("SignInGoogle")]
        public IActionResult SignInGoogle()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                Items = {["LoginProvider"] = "Google"},
                RedirectUri = Url.Action(nameof(HandleExternalSimpleLogin))
            };
            return Challenge(authenticationProperties, "Google");
        }

        [HttpGet]
        [Route("SignInWithFacebook")]
        public IActionResult SignInWithFacebook()
        {
            //var authenticationProperties =
            //    _signInManager.ConfigureExternalAuthenticationProperties("Facebook",
            //        Url.Action(nameof(HandleExternalSimpleLogin)));
            var authenticationProperties = new AuthenticationProperties
            {
                Items = {["LoginProvider"] = "Facebook"},
                RedirectUri = Url.Action(nameof(HandleExternalSimpleLogin))
            };
            return Challenge(authenticationProperties, "Facebook");
        }

        [HttpGet]
        [Route("SignInWithTwitter")]
        public IActionResult SignInWithTwitter()
        {
            //var authenticationProperties =
            //    _signInManager.ConfigureExternalAuthenticationProperties("Twitter",
            //        Url.Action(nameof(HandleExternalSimpleLogin)));
            var authenticationProperties = new AuthenticationProperties
            {
                Items = {["LoginProvider"] = "Twitter"},
                RedirectUri = Url.Action(nameof(HandleExternalSimpleLogin))
            };
            return Challenge(authenticationProperties, "Twitter");
        }

        //public async Task<IActionResult> HandleExternalLogin()
        //{
        //    var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

        //    var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
        //        externalLoginInfo.ProviderKey, isPersistent: false);

        //    if (!result.Succeeded)
        //    {
        //        var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ??
        //                    externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        //        var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ??
        //                   externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

        //        var appUser = await _userManager.FindByEmailAsync(email);

        //        if (appUser == null)
        //        {
        //            appUser = new AppUser
        //            {
        //                UserName = name,
        //                Email = email,
        //                EmailConfirmed = true
        //            };

        //            var createResult = await _userManager.CreateAsync(appUser);
        //            if (!createResult.Succeeded)
        //                throw new Exception(createResult.Errors.Select(e => e.Description)
        //                    .Aggregate((errors, error) => $"{errors}, {error}"));
        //        }

        //        await _userManager.AddLoginAsync(appUser, externalLoginInfo);
        //        var newUserClaims =
        //            externalLoginInfo.Principal.Claims.Append(new Claim("userId", appUser.Id.ToString()));
        //        await _userManager.AddClaimsAsync(appUser, newUserClaims);
        //        await _signInManager.SignInAsync(appUser, isPersistent: false);
        //        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        //    }

        //    return Redirect("/");
        //}


        public async Task<IActionResult> HandleExternalSimpleLogin()
        {
            var externalLoginInfo = await GetExternalSimpleLoginInfoAsync();
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if(email==null)
                throw new ArgumentNullException("email","email couldn't be empty");
            var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            var nameIdentifier = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var authMethod = User.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            if (authMethod == null)
            {
                authMethod = User.FindFirst("urn:twitter:screenname")?.Value != null ? 
                    "Twitter" : 
                    "Unknown provider";
            }

            var twitterMethod = User.FindFirst("urn:twitter:screenname")?.Value;
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    Name = name
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }           
                   var newUserClaims = 
                       externalLoginInfo.Principal.Claims.Append(new Claim("userId", user.Id.ToString())).ToList();
                   newUserClaims.AddRange( new List<Claim>
                   {
                       new Claim(ClaimTypes.AuthenticationMethod, externalLoginInfo.LoginProvider),
                       new Claim("ProviderKey", externalLoginInfo.ProviderKey)
                   });

            await Authenticate(email, newUserClaims);

            return Redirect("/");
        }

        private async Task<ExternalLoginInfo> GetExternalSimpleLoginInfoAsync(
            string expectedXsrf = null)
        {
            AuthenticateResult authenticateResult =
                await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            IDictionary<string, string> items = authenticateResult?.Properties?.Items;
            if (authenticateResult?.Principal == null || items == null || !items.ContainsKey("LoginProvider") ||
                expectedXsrf != null && (!items.ContainsKey("XsrfId") || items["XsrfId"] != expectedXsrf))
                return (ExternalLoginInfo) null;
            string firstValue =
                authenticateResult.Principal.FindFirstValue(
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            string str = items["LoginProvider"];
            if (firstValue == null || str == null)
                return (ExternalLoginInfo) null;
            return new ExternalLoginInfo(authenticateResult.Principal, str, firstValue, str)
            {
                AuthenticationTokens = authenticateResult.Properties.GetTokens()
            };
        }

        private async Task Authenticate(string userName, IEnumerable<Claim> newUserClaims)
        {
            ClaimsIdentity id =
                new ClaimsIdentity(newUserClaims,
                    IdentityConstants.ApplicationScheme,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            // setup auth cookies
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}