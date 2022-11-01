using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Data.UnitOfWorks;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //DI Register (Konumuz Jwt olduðu için Autofac kullanmadýk direkt olarak startupta yazdýk)
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Veritabaný baðlantý
            services.AddDbContext<AppDbContext>(x =>
            {
                //option.MigrationsAssembly("AuthServer/Data");
                //Üstteki gibi yazmak yerine dinamik þekilde vermek için Assembly kullandýk. Böylece Repository ismi deðiþse de bulabilmesi için.
                x.UseSqlServer(Configuration.GetConnectionString("SqlServer"),option=>
                {
                    option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
                });
            });

            //Üyelik sistemi (AddDefaultTokenProviders() þifre sýfýrlama iþlemleri için)
            services.AddIdentity<UserApp, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //CustomTokenOption ile appsetting arasýndaki iliþkiyi kurduk. TokenOption içerisindeki bilgileri CustomTokenOption nesnesi ile türetebilmek için bu iliþkiyi kurduk. (Options pattern)
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));

            //Client ile appsetting arasýndaki iliþkiyi kurduk. Clients içerisindeki bilgileri CustomTokenOption nesnesi ile türetebilmek için bu iliþkiyi kurduk. (Options pattern)
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            //Tokenlar ile ilgili (Doðrulama iþlemi için)
            services.AddAuthentication(options =>
            {
                //Farklý login giriþi sistemleri (Örneðin bir sitede bayi ve müþteri giriþi gibi) olsaydý þemalarý ayýrmamýz gerekirdi. Ama bizde tek login giriþi sistemi olduðu için ayýrmadýk. 
                //options.DefaultAuthenticateScheme = "Bearer"; da yazabilirdik ancak onun yerine default bir þema verdik.

                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //"AddJwtBearer(JwtBearerDefaults.AuthenticationScheme" bu yazan þema ile üstteki þemayý baðlamak için alttaki kodu yazdýk.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                //Json web token kullandýðýmýzý bu þekilde belirttik. Ayný þemayý vererek options ile içeri girdik.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                //DI container içerisinde direkt olarak nesne türettik. Token sistemi için.
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                //Token ile ilgili detaylarý veriyoruz.appsettingste yazdýðýmýz.
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    //Valid kýsmýnda datayý veriyoruz. Validate kýsmýnda kontrol ediyoruz.
                    ValidIssuer = tokenOptions.Issuer,
                    //"www.authserver.com" var mý yok mu diye kontrol ettik. Diðerlerini her MiniApp kendi içinde kontrol edecek.
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //Kontrol - Validate yaptýðýmýz kýsým burasý
                    //Ýmza doðrulama
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    //Bir tokena ömür verdiðimizde 1 saat örneðin, default olarak 5 dklýk pay ekler. Farklý zonelardaki farklý serverlar arasýndaki farktan dolayý defaul olarak 5 dk ekler. Aþaðýda biz bu özelliði kapattýk. (Farklý serverlara kurulan apiler arasýndaki zaman farkýný tolere etmek için ) (Postmanden test yapacaðýmýz için kapattýk)
                    ClockSkew=TimeSpan.Zero
                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer.API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
