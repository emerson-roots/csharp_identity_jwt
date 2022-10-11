using System;
using IdentityWithJwtBearerToken.Areas.Identity.Data;
using IdentityWithJwtBearerToken.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IdentityWithJwtBearerToken.Areas.Identity.IdentityHostingStartup))]
namespace IdentityWithJwtBearerToken.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityWithJwtBearerTokenContext>(options =>
                //options.UseSqlServer(
                //    context.Configuration.GetConnectionString("IdentityWithJwtBearerTokenContextConnection")));


                // exemplo de uso om sqlite
                // https://jcooney.net/post/2016/11/13/SqliteIdentity.html
                options.UseSqlite(
                        context.Configuration.GetConnectionString("SqliteExemploDiretorioConnection")));

                // 43:55 ao gerar o startup, ele gera automaticamente com padrão  options.SignIn.RequireConfirmedAccount = true
                // isso faz com que somente contas confirmadas na tabela de dados consigam fazer login
                //
                // alteramos essa configuração para FALSE conforme video

                services.AddDefaultIdentity<IdentityWithJwtBearerTokenUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<IdentityWithJwtBearerTokenContext>();
            });
        }
    }
}