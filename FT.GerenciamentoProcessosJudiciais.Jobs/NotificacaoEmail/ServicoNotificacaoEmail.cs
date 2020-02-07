using FT.GerenciamentoProcessosJudiciais.Infra.Coordinators;
using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Infra.Mailing;
using FT.GerenciamentoProcessosJudiciais.Jobs.Bases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Jobs.NotificacaoEmail
{
    class ServicoNotificacaoEmail : BackgroudServiceBase
    {
        readonly ILeaderElectionCoordinator leaderElection;
        readonly IEmailSender emailSender;
        readonly RavenDbContext dbContext;
        readonly Timer timer;
        readonly BlockingCollection<object> queue = new BlockingCollection<object>(boundedCapacity: 1);

        public ServicoNotificacaoEmail(ILogger<ServicoNotificacaoEmail> logger, IConfiguration configuration,
            ILeaderElectionCoordinator leaderElection, IEmailSender emailSender, RavenDbContext dbContext)
            : base(logger)
        {
            this.leaderElection = leaderElection;
            this.emailSender = emailSender;
            this.dbContext = dbContext;

            timer = new Timer(async s =>
            {
                if (queue.TryAdd(s))
                {
                    try
                    {
                        await SendEmailNotificationsAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, ex.Message);
                    }
                    finally
                    {
                        queue.Take();
                    }
                }

            }, new object(), Timeout.Infinite, Timeout.Infinite);
        }

        protected override Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            dbContext.Store.Initialize();
            timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            return base.StopAsync(cancellationToken);
        }

        async Task SendEmailNotificationsAsync()
        {
            using (var scope = Logger.BeginScope($"SendEmailNotificationsAsync"))
            {
                var isLeader = await IsLeader();
                Logger.LogDebug($"isLeader: {isLeader}");

                if (isLeader)
                {
                    var emailsToSend = await SendAndRemoveEmails(50);
                    Logger.LogDebug($"remaining emails: {emailsToSend}");
                }
            }
        }

        async Task<bool> IsLeader()
        {
            var id = $"leaderelectors/{nameof(ServicoNotificacaoEmail)}";
            var nodeId = $"{id}/{Environment.MachineName}/{DateTime.UtcNow.Ticks}";
            var leaderId = await leaderElection.ElectLeader(id, nodeId);

            Logger.LogDebug($"id: {id}");
            Logger.LogDebug($"nodeId: {nodeId}");
            Logger.LogDebug($"leaderId: {leaderId}");

            return nodeId == leaderId;
        }

        async Task<int> SendAndRemoveEmails(int max)
        {
            var emailsToSend = Enumerable.Empty<EmailNotification>().ToList();
            var emailsCount = 0;

            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                var emailsQuery = session.Query<EmailNotification>();

                emailsCount = await emailsQuery.CountAsync();
                Logger.LogDebug($"emailsCount: {emailsCount}");

                if (emailsCount == 0)
                    return 0;

                emailsToSend = await emailsQuery
                    .Take(max)
                    .ToListAsync();
            }

            foreach (var e in emailsToSend)
            {
                await emailSender.SendAsync(to: e.EmailAddress, subject: e.EmailSubject, body: e.EmailBody);
                using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                {
                    session.Delete(e.Id);
                    await session.SaveChangesAsync();
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            Logger.LogDebug($"emailsToSent: {emailsToSend.Count}");

            return Math.Max(0, emailsToSend.Count - emailsCount);
        }
    }
}
