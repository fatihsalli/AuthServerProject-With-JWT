using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using SharedLibrary.Extensions;

namespace MiniApp1.API
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
            //CustomTokenOption ile appsetting arasýndaki iliþkiyi kurduk. TokenOption içerisindeki bilgileri CustomTokenOption nesnesi ile türetebilmek için bu iliþkiyi kurduk. (Options pattern)
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));
            //DI container içerisinde direkt olarak nesne türettik. Token sistemi için.
            var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
            //Extension metot - SharedLibraryde oluþturduðumuz. Token doðrulama için. Birden fazla Api olduðu için SharedLibrary'de extension metot oluþturduk.
            services.AddCustomTokenAuth(tokenOptions);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniApp1.API", Version = "v1" });
            });
            //Claim bazlý doðrulama için bir þartname oluþturuyoruz. Role bazlý doðrulamadaki gibi direkt olarak yazamýyoruz.Policy oluþturduk.
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("AnkaraPolicy", policy =>
                {   //Birden fazla þehir yazabiliriz.
                    //policy.RequireClaim("city", "ankara", "izmir");
                    policy.RequireClaim("city", "ankara");
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniApp1.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //Doðrulama için ekledik.
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
