using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FT.GerenciamentoProcessosJudiciais.Api.Extensions;
using System.Text;
using Newtonsoft.Json.Serialization;
using Elastic.Apm.NetCoreAll;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL;
using Microsoft.AspNetCore.Http.Features;
using FT.GerenciamentoProcessosJudiciais.Api.GraphQL;

namespace FT.GerenciamentoProcessosJudiciais.Api
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
            //services.AddAuthentication().AddJwtBearer(options =>
            //  options.TokenValidationParameters = GenerateParams());

            //services.AddAuthorization(auth =>
            //{
            //    auth.AddPolicy("JwtPolicy", new AuthorizationPolicyBuilder()
            //        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //        .RequireAuthenticatedUser()
            //        .Build());
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services
                .AddSingleton<GraphMutations>()
                .AddSingleton<GraphQueries>()
                .AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService))
                .AddSingleton<GraphSchema>()
                .AddGraphQL(x => { x.ExposeExceptions = true; });

            services.AddInfrastructure(Configuration);

            services.AddControllers();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Gerenciamento ProcessosJudiciais API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            app
               //.UseAuthentication()
               //.UseAuthorization()
               .UseCors("CorsPolicy")
               .UseSwagger()
               .UseSwaggerUI(options =>
               {
                   options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1");
                   options.RoutePrefix = "swagger";
                   options.OAuthClientId("client-id");
                   options.OAuthClientSecret(Convert.ToBase64String(Encoding.UTF8.GetBytes("client-top-secret")));
                   options.OAuthRealm("");
                   options.OAuthAppName("");
               })
               .UseGraphQL<GraphSchema>()
               .UseGraphQLPlayground(new GraphQLPlaygroundOptions { Path = string.Empty })
               .UseAllElasticApm(Configuration)
               .UseRouting()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               })
               .UseInfrastructure(provider);
        }
    }
}
