using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Interfaces;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.DataAccess
{
    class RepositorioLeituraProcessos : IRepositorioLeituraProcessos
    {
        private readonly RavenDbContext dbContext;

        public RepositorioLeituraProcessos(RavenDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<DadosProcesso> IRepositorioLeituraProcessos.Load(string aggregateId)
        {
            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                var dados = await session.Query<DadosProcesso>()
                    .Where(x => x.AggregateId == aggregateId)
                    .FirstOrDefaultAsync();

                return dados;
            }
        }

        async Task<IEnumerable<DadosHierarquiaProcesso>> IRepositorioLeituraProcessos.LoadHierarquia(string aggregateId)
        {
            var hierarquia = new List<DadosProcesso>();
            var superiores = new List<DadosProcesso>();
            var inferiores = new List<DadosProcesso>();
            var processoBase = default(DadosProcesso);

            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                processoBase = await session.Query<DadosProcesso>()
                    .Where(x => x.AggregateId == aggregateId)
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
                            superiores.Add(superior);

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
                            inferiores.Add(inferior);

                        idInferior = inferior?.ProcessoPai?.AggregateId;
                    }
                }

                hierarquia.AddRange(superiores);
                hierarquia.Add(processoBase);
                hierarquia.AddRange(inferiores);
            }

            var dados = hierarquia.Select(h => new DadosHierarquiaProcesso
            {
                Nivel = hierarquia.IndexOf(h) + 1,
                Processo = h
            }).ToArray();

            return dados;
        }

        async Task<DadosPaginados<DadosProcesso>> IRepositorioLeituraProcessos.Query(
            Expression<Func<DadosProcesso, bool>> predicate, int pageIndex, int pageSize)
        {
            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                var query = session.Query<DadosProcesso>();

                if (predicate != default)
                    query = query.Where(predicate);

                var total = await query.CountAsync();

                if (pageIndex <= 0)
                    pageIndex = 1;

                if (pageSize == 0)
                    pageSize = 10;

                var dados = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new DadosPaginados<DadosProcesso>()
                {
                    Dados = dados,
                    TotalRegistros = total,
                    TotalPaginas = Math.Max(1, total / pageSize)
                };
            }
        }
    }
}
