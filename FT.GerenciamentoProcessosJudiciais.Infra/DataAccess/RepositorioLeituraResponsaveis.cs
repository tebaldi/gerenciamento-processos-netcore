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
    class RepositorioLeituraResponsaveis : IRepositorioLeituraResponsaveis
    {
        private readonly RavenDbContext dbContext;

        public RepositorioLeituraResponsaveis(RavenDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<DadosResponsavel> IRepositorioLeituraResponsaveis.Load(string aggregateId)
        {
            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                var dados = await session.Query<DadosResponsavel>()
                    .Where(x => x.AggregateId == aggregateId)
                    .FirstOrDefaultAsync();

                return dados;
            }
        }

        async Task<DadosPaginados<DadosResponsavel>> IRepositorioLeituraResponsaveis.Query(
            Expression<Func<DadosResponsavel, bool>> predicateResponsavel,
            Expression<Func<DadosProcesso, bool>> predicateProcesso, int pageIndex, int pageSize)
        {
            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                var query = session.Query<DadosResponsavel>();

                if (predicateResponsavel != default)
                    query = query.Where(predicateResponsavel);

                if (predicateProcesso != null)
                {
                    var resonsaveisProcessos = await session.Query<DadosProcesso>()
                        .Where(predicateProcesso)
                        .Select(x => x.Responsaveis)
                        .ToListAsync();

                    var ids = new List<string>();
                    foreach (var r in resonsaveisProcessos)
                        ids.AddRange(r.Select(g => g.AggregateId));

                    if (ids.Any())
                        query = query.Where(x => x.AggregateId.In(ids.Distinct()));
                }

                var total = await query.CountAsync();

                if (pageIndex <= 0)
                    pageIndex = 1;

                if (pageSize == 0)
                    pageSize = 10;

                var dados = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

                return new DadosPaginados<DadosResponsavel>()
                {
                    Dados = dados,
                    TotalRegistros = total,
                    TotalPaginas = Math.Max(1, total / pageSize)
                };
            }
        }
    }
}
