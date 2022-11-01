using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AuthServer.Data
{
    //DbContext yerine IdentityDbContext'ten miras aldık. Kendi oluşturduğumuz "UserApp" i ve IdentityRole (Identity kütüphanesinden gelen) ve Id'yi nin tipini belirttik.
    public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        //Access tokenları değil Refresh tokenları serverda tutacağız.
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Tüm Configuration dosyalarını nasıl buluyor "ApplyConfigurationsFromAssembly" methodu ile "IEntityTypeConfiguration" den miras alan Assemblyleri buluyor. Assembly.GetExecutingAssembly() demek de çalıştığımız klasörde ara demektir. 
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

    }
}
