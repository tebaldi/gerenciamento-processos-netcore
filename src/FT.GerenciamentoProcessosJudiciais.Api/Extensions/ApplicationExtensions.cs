using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Infra.Pipelines;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this IApplicationBuilder app, IServiceProvider provider)
        {
            var dbContext = provider.GetService<RavenDbContext>();

            dbContext.Store.Initialize();
        }
    }
}