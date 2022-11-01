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
            //DI Register (Konumuz Jwt oldu�u i�in Autofac kullanmad�k direkt olarak startupta yazd�k)
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Veritaban� ba�lant�
            services.AddDbContext<AppDbContext>(x =>
            {
                //option.MigrationsAssembly("AuthServer/Data");
                //�stteki gibi yazmak yerine dinamik �ekilde vermek i�in Assembly kulland�k. B�ylece Repository ismi de�i�se de bulabilmesi i�in.
                x.UseSqlServer(Configuration.GetConnectionString("SqlServer"),option=>
                {
                    option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
                });
            });

            //�yelik sistemi (AddDefaultTokenProviders() �ifre s�f�rlama i�lemleri i�in)
            services.AddIdentity<UserApp, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //CustomTokenOption ile appsetting aras�ndaki ili�kiyi kurduk. TokenOption i�erisindeki bilgileri CustomTokenOption nesnesi ile t�retebilmek i�in bu ili�kiyi kurduk. (Options pattern)
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));

            //Client ile appsetting aras�ndaki ili�kiyi kurduk. Clients i�erisindeki bilgileri CustomTokenOption nesnesi ile t�retebilmek i�in bu ili�kiyi kurduk. (Options pattern)
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            //Tokenlar ile ilgili (Do�rulama i�lemi i�in)
            services.AddAuthentication(options =>
            {
                //Farkl� login giri�i sistemleri (�rne�in bir sitede bayi ve m��teri giri�i gibi) olsayd� �emalar� ay�rmam�z gerekirdi. Ama bizde tek login giri�i sistemi oldu�u i�in ay�rmad�k. 
                //options.DefaultAuthenticateScheme = "Bearer"; da yazabilirdik ancak onun yerine default bir �ema verdik.

                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //"AddJwtBearer(JwtBearerDefaults.AuthenticationScheme" bu yazan �ema ile �stteki �emay� ba�lamak i�in alttaki kodu yazd�k.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                //Json web token kulland���m�z� bu �ekilde belirttik. Ayn� �emay� vererek options ile i�eri girdik.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                //DI container i�erisinde direkt olarak nesne t�rettik. Token sistemi i�in.
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                //Token ile ilgili detaylar� veriyoruz.appsettingste yazd���m�z.
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    //Valid k�sm�nda datay� veriyoruz. Validate k�sm�nda kontrol ediyoruz.
                    ValidIssuer = tokenOptions.Issuer,
                    //"www.authserver.com" var m� yok mu diye kontrol ettik. Di�erlerini her MiniApp kendi i�inde kontrol edecek.
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //Kontrol - Validate yapt���m�z k�s�m buras�
                    //�mza do�rulama
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    //Bir tokena �m�r verdi�imizde 1 saat �rne�in, default olarak 5 dkl�k pay ekler. Farkl� zonelardaki farkl� serverlar aras�ndaki farktan dolay� defaul olarak 5 dk ekler. A�a��da biz bu �zelli�i kapatt�k. (Farkl� serverlara kurulan apiler aras�ndaki zaman fark�n� tolere etmek i�in ) (Postmanden test yapaca��m�z i�in kapatt�k)
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
