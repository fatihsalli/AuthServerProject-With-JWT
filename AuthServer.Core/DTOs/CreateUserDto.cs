using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class CreateUserDto
    {
        //İlk üyelik esnasında kullanıcıyı sıkmamak adına çok fazla data almak yerine minimum gerekli olan datayı alıp eksik kalan alanların sonradan doldurulması daha sağlıklı bir yöntemdir.
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }



    }
}
