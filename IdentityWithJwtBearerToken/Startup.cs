using IdentityWithJwtBearerToken.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityWithJwtBearerToken
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * video aula https://www.youtube.com/watch?v=Lh82WlOvyQk&list=LL&index=4&t=858s
             * 
             * 
             * **/
            var key = Encoding.ASCII.GetBytes("MY_BIG_SECRET_KEY_lkdjflkdjfklj¨&@*(@*$()#)()@(#)@(HJB<MBSDKM<BNASD");

            services.AddAuthentication(x =>
            {
                // requer dependencia Microsoft.AspNetCore.Authentication.JwtBearer
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {

                        OnTokenValidated = context =>
                        {

                            // 23:50 - IdentityWithJwtBearerTokenUser
                            //
                            // deterimna se a pessoa é capaz de acessar os metodos seguros com base no
                            // token que forneceram

                            var userMachine = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityWithJwtBearerTokenUser>>(); // gerenciador de usuario do AspNetCore.Identity

                            //obtem o usuario e as informações de Claims (reinvindicações)
                            var user = userMachine.GetUserAsync(context.HttpContext.User);

                            // se o usuario do contexto vier nulo, significa Unauthorized
                            // 26:52 - depois daqui, partimos para configs no ValuesController
                            if (user == null)
                                context.Fail("UnAuthorized");

                            return Task.CompletedTask;
                        }
                    };

                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //  altera o padrao que é de cinco minutos de expiracao no minimo para zero
                        ClockSkew = TimeSpan.Zero

                    };
                });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
