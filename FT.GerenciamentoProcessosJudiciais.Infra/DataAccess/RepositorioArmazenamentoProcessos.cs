using FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso;
using FT.GerenciamentoProcessosJudiciais.Dominio.Processos;
using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;
using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.DataAccess
{
    class RepositorioArmazenamentoProcessos : IRepositorioArmazenamentoProcessos
    {
        private readonly ILogger<RepositorioArmazenamentoProcessos> logger;
        private readonly RavenDbContext dbContext;

        public RepositorioArmazenamentoProcessos(ILogger<RepositorioArmazenamentoProcessos> logger, RavenDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        async Task<Processo> IRepositorioArmazenamentoProcessos.CarregarAsync(IdProcesso aggregateId)
        {
            using (logger.BeginScope($"CarregarAsync: {aggregateId}"))
            {
                try
                {
                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        var id = aggregateId.ToString();

                        var eventos = await session.Query<EventoPadrao, Eventos_PorAggregate>()
                            .Where(x => x.AggregateId == id)
                            .OrderBy(x => x.AggregateVersion)
                            .ToListAsync();

                        var processo = Processo.Construir(ListModule.OfSeq(eventos));

                        return processo;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        async Task IRepositorioArmazenamentoProcessos.ArmazenarAsync(IEnumerable<EventoPadrao> eventos, IEnumerable<NotificacaoEmail> notificacoes)
        {
            using (logger.BeginScope($"ArmazenarAsync: {eventos}"))
            {
                try
                {
                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        foreach (var e in eventos)
                        {
                            if (e == default)
                                continue;

                            var id = MapToDocumentId(e.AggregateId);
                            var exits = await session.Advanced.ExistsAsync(id);
                            if (!exits)
                            {
                                var instance = new AggregateVersionCounter { AggregateId = e.AggregateId };
                                await session.StoreAsync(instance, id);
                            }

                            var counters = session.CountersFor(id);
                            counters.Increment("AggregateVersion", 1);
                            await session.SaveChangesAsync();

                            var version = await counters.GetAsync("AggregateVersion");

                            e.SetVersion(version.GetValueOrDefault());
                            var eventId = MapToEventId(e.AggregateId, e.AggregateVersion);
                            await session.StoreAsync(e, eventId);
                        }

                        foreach (var n in notificacoes)
                        {
                            await session.StoreAsync(new EmailNotification
                            {
                                EmailAddress = n.Email,
                                EmailSubject = n.Assunto,
                                EmailBody = n.Mensagem,
                                CreatedTime = DateTime.UtcNow
                            });
                        }

                        await session.SaveChangesAsync();
                    }
                }
                catch (Raven.Client.Exceptions.ConcurrencyException ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        async Task<bool> IRepositorioArmazenamentoProcessos.VerificarNumeroCadastrado(IdProcesso aggregateId, NumeroProcessoUnificado numero)
        {
            using (logger.BeginScope($"VerificarNumeroCadastrado: {aggregateId} {numero}"))
            {
                try
                {
                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        var dados = await session.Query<DadosProcesso>()
                            .Where(x =>
                                x.NumeroProcessoUnificado == numero.Valor &&
                                x.AggregateId != aggregateId.Valor && !x.Removido)
                            .FirstOrDefaultAsync();

                        return dados != null;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        async Task<bool> IRepositorioArmazenamentoProcessos.VerificarProcessoPaiAtrelado(IdProcesso aggregateId, IdProcesso idProcessoPai)
        {
            using (logger.BeginScope($"VerificarProcessoPaiAtrelado: {idProcessoPai}"))
            {
                try
                {
                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        var dados = await session.Query<DadosProcesso>()
                            .Where(x =>
                                x.AggregateId != aggregateId.Valor &&
                                x.ProcessoPai.AggregateId == idProcessoPai.Valor)
                            .FirstOrDefaultAsync();

                        return dados != null;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        async Task<List<IdProcesso>> IRepositorioArmazenamentoProcessos.CarregarHierarquiaProcessoPorNivel(IdProcesso aggregateId)
        {
            using (logger.BeginScope($"CarregarHierarquiaProcessoPorNivel: {aggregateId}"))
            {
                try
                {
                    var hierarquia = new List<IdProcesso>();
                    var superiores = new List<IdProcesso>();
                    var inferiores = new List<IdProcesso>();
                    var processoBase = default(DadosProcesso);

                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        processoBase = await session.Query<DadosProcesso>()
                            .Where(x => x.AggregateId == aggregateId.Valor)
                            .FirstOrDefaultAsync();
                    }

                    if (processoBase != null)
                    {
                        using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                        {
                            var idSuperior = processoBase.ProcessoPai?.AggregateId;
                            while (!string.IsNullOrEmpty(idSuperior))
                            {
                                var superior = await session.Query<DadosProcesso>()
                                    .Where(x => x.AggregateId == idSuperior)
                                    .FirstOrDefaultAsync();

                                if (superior != null)
                                    superiores.Add(new IdProcesso(superior.AggregateId));

                                idSuperior = superior?.ProcessoPai?.AggregateId;
                            }
                        }

                        using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                        {
                            var idInferior = processoBase.AggregateId;
                            while (!string.IsNullOrEmpty(idInferior))
                            {
                                var inferior = await session.Query<DadosProcesso>()
                                    .Where(x => x.ProcessoPai.AggregateId == idInferior)
                                    .FirstOrDefaultAsync();

                                if (inferior != null)
                                    inferiores.Add(new IdProcesso(inferior.AggregateId));

                                idInferior = inferior?.ProcessoPai?.AggregateId;
                            }
                        }

                        hierarquia.AddRange(superiores);
                        hierarquia.Add(aggregateId);
                        hierarquia.AddRange(inferiores);
                    }

                    return hierarquia;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        string MapToDocumentId(string aggregateId) => $"processos/{aggregateId}";

        string MapToEventId(string aggregateId, long aggregateVersion) => $"{MapToDocumentId(aggregateId)}/{aggregateVersion}";
    }
}
