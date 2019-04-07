using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCookiesAuth.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return Content("Running...");
        }

        [Route("api/home/isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return new ObjectResult(User.Identity.IsAuthenticated);
        }

        [Route("api/home/fail")]
        public IActionResult Fail()
        {
            return Unauthorized();
        }

        [Route("api/home/name")]
        [Authorize]
        public IActionResult Name()
        {
            var claimsPrincipal = User;
            var givenName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            var authMethod = User.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            if (authMethod == null)
            {
                authMethod = User.FindFirst("urn:twitter:screenname")?.Value != null ? "Twitter" : "Unknown provider";
            }

            var twitterMethod = User.FindFirst("urn:twitter:screenname")?.Value;
            return Ok($"{givenName} from {authMethod}");
        }


        [Route("api/home/data")]
        [Authorize]
        public IActionResult Data()
        {
            var lst = User.Claims.Select(c => $"{c.Type.PadRight(20)} = {c.Value}").ToList();
            return Ok(lst);
        }

        [Route("/home/[action]")]
        public IActionResult Denied()
        {
            var authMethod = User.FindFirst(ClaimTypes.AuthenticationMethod).Value ?? "unknown provider";
            return Content($"You need to allow this application access in {authMethod} order to be able to login");
        }
    }
}