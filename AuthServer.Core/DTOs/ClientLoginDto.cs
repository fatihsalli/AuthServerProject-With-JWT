using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class ClientLoginDto
    {
        //Kullanıcı olmayan durumlarda "ClientId" bir nevi "LoginDto" daki email, "ClientSecret" ise "Password" gibi düşünebiliriz.
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
