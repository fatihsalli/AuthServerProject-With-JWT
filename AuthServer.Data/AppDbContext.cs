using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    //DbContext yerine IdentityDbContext'ten miras aldık. Kendi oluşturduğumuz "UserApp" i ve IdentityRole (Identity kütüphanesinden gelen) ve Id'yi nin tipini belirttik.
    public class AppDbContext : IdentityDbContext<UserApp,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
                
        }

        public DbSet<Product> Products { get; set; }
        //Access tokenları değil Refresh tokenları serverda tutacağız.
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }



    }
}
