using FT.GerenciamentoProcessosJudiciais.Infra.Coordinators;
using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Infra.EventHub;
using FT.GerenciamentoProcessosJudiciais.Infra.Mailing;
using FT.GerenciamentoProcessosJudiciais.Jobs.NotificacaoEmail;
using FT.GerenciamentoProcessosJudiciais.Jobs.PublicacaoEventos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Jobs
{
    class Program
    {
        async static Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
                await host.RunAsync();
        }

        static IConfiguration Configuration;

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, config) =>
               {
                   config
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                   Configuration = config.Build();
               })
                .ConfigureLogging(logging =>
                {
                    var elasticUri = Configuration["ElasticSettings:Uri"];

                    logging
                        .ClearProviders()
                        .AddConsole()
                         .AddSerilog(new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .Enrich.FromLogContext()
                            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                            {
                                AutoRegisterTemplate = true,
                                EmitEventFailure = EmitEventFailureHandling.ThrowException,
                            })
                            .CreateLogger());
                })
               .ConfigureServices((hostContext, services) =>
               {
                   services
                        .AddSingleton(Configuration)
                        .Configure<RavenDbSettings>(Configuration.GetSection(nameof(RavenDbSettings)))
                        .AddSingleton<RavenDbContext>()
                        .AddSingleton<ILeaderElectionCoordinator, LeaderElectionCoordinator>()
                        .AddSingleton<IEventHub, InMemoryEventHub>()
                        .AddSingleton<IEmailSender, EmailSender>()
                        .AddHostedService<ServicoNotificacaoEmail>()
                        .AddHostedService<ServicoPublicacaoEventos>();
               });
    }
}
