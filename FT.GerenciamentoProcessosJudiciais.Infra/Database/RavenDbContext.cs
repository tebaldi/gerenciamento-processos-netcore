using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    public class RavenDbContext
    {
        public static string DatabaseName { get; private set; }
        private static IDocumentStore store;
        private static object _lock = new object();

        private readonly ILogger<RavenDbContext> logger;
        private readonly RavenDbSettings settings;
        public RavenDbContext(ILogger<RavenDbContext> logger, IOptions<RavenDbSettings> options)
        {
            this.logger = logger;
            settings = options.Value;
            DatabaseName = settings.DatabaseName;
        }

        public IDocumentStore Store
        {
            get
            {
                if (store == null)
                {
                    lock (_lock)
                        store = CreateStore();
                }

                return store;
            }
        }


        private IDocumentStore CreateStore()
        {
            if (store != null)
                return store;

            using (logger.BeginScope($"CreateStore: {settings}"))
            {
                try
                {
                    store = new DocumentStore()
                    {
                        Urls = new[] { settings.Urls, },
                        Conventions =
                        {
                            MaxNumberOfRequestsPerSession = 30,
                            UseOptimisticConcurrency = true,
                            FindCollectionName = MapCollectionTypes
                        }
                    }.Initialize();

                    try
                    {
                        store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(DatabaseName)));
                    }
                    catch { }

                    try
                    {
                        IndexCreation.CreateIndexes(
                            Assembly.GetExecutingAssembly(), store, store.Conventions, DatabaseName);
                    }
                    catch { }

                    return store;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        static Func<Type, string> MapCollectionTypes => type => type switch
        {
            var t when typeof(DadosResponsavel).IsAssignableFrom(t) => "Responsaveis",
            var t when typeof(DadosProcesso).IsAssignableFrom(t) => "Processos",
            var t when typeof(AggregateVersionCounter).IsAssignableFrom(t) => "AggregateVersionCounters",
            var t when typeof(LeaderElector).IsAssignableFrom(t) => "LeaderElectors",
            var t when typeof(EventPublished).IsAssignableFrom(t) => "EventsPublished",
            var t when typeof(EmailNotification).IsAssignableFrom(t) => "EmailNotifications",
            _ => "Eventos"
        };
    }
}
