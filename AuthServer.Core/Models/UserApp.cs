using Microsoft.AspNetCore.Identity;

//Microsoft.AspNetCore.Identity.EntityFrameworkCore paketini yükledikten sonra IdentityUser'dan miras alıyoruz. IdentityUser özelliklerine ek olarak AppUSer'da tanımladıklarımız da yer alacak.
namespace AuthServer.Core.Models
{
    public class UserApp : IdentityUser
    {
        public string City { get; set; }

    }
}
