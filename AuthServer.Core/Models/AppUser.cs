using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Microsoft.AspNetCore.Identity.EntityFrameworkCore paketini yükledikten sonra IdentityUser'dan miras alıyoruz. IdentityUser özelliklerine ek olarak AppUSer'da tanımladıklarımız da yer alacak.
namespace AuthServer.Core.Models
{
    public class AppUser:IdentityUser
    {
        public string City { get; set; }

    }
}
