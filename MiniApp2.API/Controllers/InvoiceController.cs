using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp2.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoices()
        {
            //veri tabanında userName veya userId alanları üzerinden gerekli dataları çekebiliriz.
            var userName = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Invoice => UserName:{userName}-UserId: {userIdClaim.Value}");
        }



    }
}
