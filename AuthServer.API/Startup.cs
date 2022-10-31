using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Data.UnitOfWorks;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
