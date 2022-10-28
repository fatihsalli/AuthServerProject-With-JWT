using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //Response dönmüyoruz çünkü kendi içerimizde kullanacağız.
    public interface ITokenService
    {
        TokenDto CreateToken(UserApp appUser);
        ClientTokenDto CreateTokenByClient(Client client);


    }
}
