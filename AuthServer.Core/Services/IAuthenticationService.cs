using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        //Asenkron metot threadleri daha efektif kullanmamızı sağlayan metottur.
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);
        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
        //Kullanıcı log-out yaptığında Refresh tokenı sonlandırma durumu için aşağıdaki metot tanımlandı. Ya da Refreshtoken çalındığı durumda bu metot ile server tarafında refresh tokenı nulla set edebiliriz.
        Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken);
        //Client ile birlikte üyelik durumu olmadan bir token alabiliriz. Aşaığıdaki metot bu durum için tanımlanmıştır. İçerisinde bir RefreshToken yok çünkü ClientId ve ClientSecret ile ben istediğim zaman Access Token alabilirim. Biz Client tarafında Client Id ve Secret bilgilerini dizin şeklinde AuthServer.Api-app.settings içinde tutacağız ancak 5 ten fazla olması durumunda serverda tutmak gerekir.
        Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto);


    }
}
