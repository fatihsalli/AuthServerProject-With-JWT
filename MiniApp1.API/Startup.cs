using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MiniApp1.API.Requirements;
using SharedLibrary.Configurations;
using SharedLibrary.Extensions;
using static MiniApp1.API.Requirements.BirthDateRequirement;

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
            //CustomTokenOption ile appsetting aras?ndaki ili?kiyi kurduk. TokenOption i?erisindeki bilgileri CustomTokenOption nesnesi ile t?retebilmek i?in bu ili?kiyi kurduk. (Options pattern)
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));
            //DI container i?erisinde direkt olarak nesne t?rettik. Token sistemi i?in.
            var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
            //Extension metot - SharedLibraryde olu?turdu?umuz. Token do?rulama i?in. Birden fazla Api oldu?u i?in SharedLibrary'de extension metot olu?turduk.
            services.AddCustomTokenAuth(tokenOptions);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniApp1.API", Version = "v1" });
            });

            //Policy bazl? do?rulama i?in "BirthDateRequirementHandler" nesnesini t?rettik.
            services.AddSingleton<IAuthorizationHandler, BirthDateRequirementHandler>();

            //Claim bazl? do?rulama i?in bir ?artname olu?turuyoruz. Role bazl? do?rulamadaki gibi direkt olarak yazam?yoruz.Policy olu?turduk.
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("AnkaraPolicy", policy =>
                {   //Birden fazla ?ehir yazabiliriz.
                    //policy.RequireClaim("city", "ankara", "izmir");
                    policy.RequireClaim("city", "ankara");
                });
                //Policy bazl? do?rulama i?in "BirthDateRequirement" class? olu?turduk a?a??da da policy olu?turuyoruz. "new BirthDateRequirement(18)" 18 ya??nda olanlar ve b?y?kler girebilir.
                opts.AddPolicy("AgePolicy", policy =>
                {
                    policy.Requirements.Add(new BirthDateRequirement(18));
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
            //Do?rulama i?in ekledik.
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
