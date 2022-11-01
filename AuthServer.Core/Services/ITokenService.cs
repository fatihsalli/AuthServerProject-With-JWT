using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;

namespace AuthServer.Core.Services
{
    //Response dönmüyoruz çünkü kendi içerimizde kullanacağız.
    public interface ITokenService
    {
        TokenDto CreateToken(UserApp appUser);
        ClientTokenDto CreateTokenByClient(Client client);


    }
}
