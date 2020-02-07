using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Infra.Pipelines;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipeline<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestElasticAPMPipeline<,>));
            services.AddMediatR(new[]
            {
                Assembly.Load("FT.GerenciamentoProcessosJudiciais.Dominio"),
                Assembly.Load("FT.GerenciamentoProcessosJudiciais.Relatorios")
            });
            services.AddLogging();
            services.Configure<RavenDbSettings>(configuration.GetSection(nameof(RavenDbSettings)));
            services.AddSingleton<RavenDbContext>();

            Assembly.Load("FT.GerenciamentoProcessosJudiciais.Infra").GetTypes()
               .Where(t => t.Namespace != null && t.Namespace.Contains("DataAccess")).ToList()
               .ForEach(t =>
               {
                   foreach (var i in t.GetInterfaces())
                       services.AddScoped(i, t);
               });
        }
    }
}