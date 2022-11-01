using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        //api/auth/createtoken
        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result=await _authenticationService.CreateTokenAsync(loginDto);
            //result generic aldığı için içerisinden generic ne olduğunu çıkartıyor.
            return ActionResultInstance(result);
        }

        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var result = _authenticationService.CreateTokenByClient(clientLoginDto);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefteshTokenDto refteshTokenDto)
        {
            var result = await _authenticationService.RevokeRefreshTokenAsync(refteshTokenDto.RefreshTokenCode);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefteshTokenDto refteshTokenDto)
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refteshTokenDto.RefreshTokenCode);
            return ActionResultInstance(result);
        }




    }
}
