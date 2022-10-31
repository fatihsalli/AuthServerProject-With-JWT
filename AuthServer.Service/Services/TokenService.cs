using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthServer.Service.Services
{
    internal class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;
        //Options kısmında hangi değeri verirsek onu alabiliriz. Direkt constructorda geçmek yerine options ile aldık. CustomTokenOption yerine başka bir modelde olabilirdi SignOption mesela.
        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }
        //Refresh token üretmek için
        private string CreateRefreshToken()
        {
            //Guidde aynı olma ihtimali çok zor ama biz burada Microsoft kütüphanesi kullandık."RandomNumberGenerator"
            //return Guid.NewGuid().ToString();
            var numberByte = new Byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        //Payloaddaki key-value değerleri için bu metodu kullandık. Üyelik sisteminin olduğu apiler için.
        private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> auidences)
        {
            var userList = new List<Claim>
            {
                //Önce key sonra value şeklinde
                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                //new Claim("email", userApp.Email), "email" olarak da bu şekilde yazabilirdik.
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
                //Herhangi bir Api decode ederken Identity kütüphanesinden faylanmak adına aşağıdaki isimlendirmeleri kullandık."name" de diyebilirdik key tarafında.
                new Claim(ClaimTypes.Name, userApp.UserName),
                //Her üretilen tokena bir id vermek için
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            //Her bir auidences için Claim oluşturmak için Select kullandık foreach gibi aslında.
            userList.AddRange(auidences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }

        //Üyelik sisteminin olmadığı apiler içim
        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            //Her bir auidences için Claim oluşturmak için Select kullandık foreach gibi aslında.
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            //
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString()));
           
            return claims;
        }


        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);

            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            //Token imza kısmı
            SigningCredentials signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);

            //appsettings de girdiğimiz kısımları burada tanımladık.
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer:_tokenOption.Issuer,
                expires:accessTokenExpiration,
                notBefore:DateTime.Now,
                claims: GetClaims(userApp, _tokenOption.Audience),
                signingCredentials:signingCredentials);

            //Tokenı oluşturacak olan class
            var handler = new JwtSecurityTokenHandler();

            //Tokenı oluşturan metot
            var token=handler.WriteToken(jwtSecurityToken);

            //TokenDto tipimize çeviriyoruz.
            var tokenDto = new TokenDto
            {
                AccessToken=token,
                RefreshToken=CreateRefreshToken(),
                AccessTokenExpiration=accessTokenExpiration,
                ResfreshTokenExpiration=refreshTokenExpiration
            };

            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration= DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);

            var securityKey=SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            //Token imza kısmı
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
             issuer: _tokenOption.Issuer,
             expires: accessTokenExpiration,
             notBefore: DateTime.Now,
             claims: GetClaimsByClient(client),
             signingCredentials: signingCredentials);

            //Tokenı oluşturacak olan class
            var handler = new JwtSecurityTokenHandler();

            //Tokenı oluşturan metot
            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration= accessTokenExpiration
            };

            return tokenDto;
        }
    }
}
