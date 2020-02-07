using FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso;
using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis;
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
    class RepositorioArmazenamentoResponsaveis : IRepositorioArmazenamentoResponsaveis
    {
        private readonly ILogger<RepositorioArmazenamentoResponsaveis> logger;
        private readonly RavenDbContext dbContext;

        public RepositorioArmazenamentoResponsaveis(ILogger<RepositorioArmazenamentoResponsaveis> logger, RavenDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        async Task<Responsavel> IRepositorioArmazenamentoResponsaveis.CarregarAsync(IdResponsavel aggregateId)
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

                        var responsavel = Responsavel.Construir(ListModule.OfSeq(eventos));

                        return responsavel;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);
                    throw;
                }
            }
        }

        async Task IRepositorioArmazenamentoResponsaveis.ArmazenarAsync(IEnumerable<EventoPadrao> eventos)
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

        async Task<bool> IRepositorioArmazenamentoResponsaveis.VerificarCpfCadastrado(IdResponsavel id, Cpf cpf)
        {
            using (logger.BeginScope($"VerificarCpfCadastrado: {id} {cpf}"))
            {
                try
                {
                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        var dados = await session.Query<DadosResponsavel>()
                            .Where(x => x.Cpf == cpf.Valor && x.AggregateId != id.Valor && !x.Removido)
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

        async Task<bool> IRepositorioArmazenamentoResponsaveis.VerificarProcessoAtrelado(IdResponsavel aggregateId)
        {
            using (logger.BeginScope($"VerificarProcessoAtrelado: {aggregateId}"))
            {
                try
                {
                    using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
                    {
                        var dados = await session.Query<DadosProcesso>()
                            .Where(x => x.Responsaveis.Any(r => r.AggregateId == aggregateId.Valor))
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

        string MapToDocumentId(string aggregateId) => $"responsaveis/{aggregateId}";

        string MapToEventId(string aggregateId, long aggregateVersion) => $"{MapToDocumentId(aggregateId)}/{aggregateVersion}";
    }
}
