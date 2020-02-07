using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;
using FT.GerenciamentoProcessosJudiciais.Infra.Coordinators;
using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Infra.EventHub;
using FT.GerenciamentoProcessosJudiciais.Jobs.Bases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Subscriptions;
using Raven.Client.Exceptions.Documents.Subscriptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Jobs.PublicacaoEventos
{
    class ServicoPublicacaoEventos : BackgroudServiceBase
    {
        readonly IEventHub eventHub;
        readonly RavenDbContext dbContext;

        public ServicoPublicacaoEventos(ILogger<ServicoPublicacaoEventos> logger,
            IEventHub eventHub, RavenDbContext dbContext)
            : base(logger)
        {
            this.eventHub = eventHub;
            this.dbContext = dbContext;
        }

        protected override Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            dbContext.Store.Initialize();
            var subscriptionName = $"{nameof(ServicoPublicacaoEventos)}Subscription";
            try
            {
                dbContext.Store.Subscriptions.Create(new SubscriptionCreationOptions<EventoPadrao>()
                {
                    Name = subscriptionName
                }, RavenDbContext.DatabaseName);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
            }

            return PublishAsync(subscriptionName);
        }

        async Task PublishAsync(string subscriptionName)
        {
            using (var scope = Logger.BeginScope($"PublishAsync {subscriptionName}"))
            {
                try
                {
                    var publisher = dbContext.Store.Subscriptions.GetSubscriptionWorker<EventoPadrao>(
                        new SubscriptionWorkerOptions(subscriptionName)
                        {
                            TimeToWaitBeforeConnectionRetry = TimeSpan.FromSeconds(5),
                            MaxErroneousPeriod = TimeSpan.FromSeconds(60),
                            Strategy = SubscriptionOpeningStrategy.OpenIfFree
                        }, RavenDbContext.DatabaseName);

                    await publisher.Run(async batch =>
                    {
                        var itens = batch.Items.Select(i =>
                            $"{i.Result.EventName} {i.Result.AggregateId} {i.Result.AggregateVersion}"
                        ).Aggregate((i1, i2) => $"{i1}\n{i2}");
                        Logger.LogDebug($"NumberOfItemsInBatch {batch.NumberOfItemsInBatch}\n{itens}");

                        foreach (var i in batch.Items)
                            await eventHub.Publish(i.Result);
                    });
                }
                catch (SubscriptionInUseException ex)
                {
                    var delay = TimeSpan.FromSeconds(60);
                    Logger.LogInformation($"SubscriptionInUseException {ex.Message}\nWill retry in {delay.TotalSeconds} seconds");
                    await Task.Delay(delay);
                    await PublishAsync(subscriptionName);
                }
                catch(SubscriptionClosedException ex)
                {
                    var delay = TimeSpan.FromSeconds(60);
                    Logger.LogInformation($"SubscriptionClosedException {ex.Message}\nWill retry in {delay.TotalSeconds} seconds");
                    await Task.Delay(delay);
                    await PublishAsync(subscriptionName);
                }
            }
        }
    }
}