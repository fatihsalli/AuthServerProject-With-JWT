using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        //Generic metot oluşturduk. Ok,BadRequest vs tekrar tekrar yazmak yerine o işlemi burada yapacağız.
        public IActionResult ActionResultInstance<T>(Response<T> response) where T:class
        {
            //IActionResult'ın bir üst class'ı ObjectResult dönerek Ok,BadRequest yazmamıza gerek kalmadı.
            return new ObjectResult(response)
            {
                StatusCode=response.StatusCode
            };
        }



    }
}
