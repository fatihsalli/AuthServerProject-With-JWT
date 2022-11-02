using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        //Startupta hazırladığımız policy-şartnameyi burada bu şekilde kullandık. Şehir bilgisi Ankara olanlar girebilecek sadece.
        [Authorize(Roles = "admin,manager",Policy ="AnkaraPolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            //veri tabanında userName veya userId alanları üzerinden gerekli dataları çekebiliriz.
            var userName = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock => UserName:{userName}-UserId: {userIdClaim.Value}");
        }



    }
}
