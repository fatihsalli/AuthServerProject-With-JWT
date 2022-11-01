using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        //Authorize ile işaretledik token istediğimiz için.
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            //Name'i nasıl buluyor sisteme kullanıcı giriş yaptığında "TokenService-GetClaims-ClaimTypes.Name,userApp.UserName" kısmından buluyor. İsimlendirmeyi doğru şekilde verdiğimizden context üzerinden kendi buluyor.
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name));
        }


    }
}
