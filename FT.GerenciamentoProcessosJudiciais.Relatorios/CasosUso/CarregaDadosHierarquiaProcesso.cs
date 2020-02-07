using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.CasosUso
{
    class CarregaDadosHierarquiaProcesso : IRequestHandler<CarregarDadosHierarquiaProcesso, IEnumerable<DadosHierarquiaProcesso>>
    {
        private readonly IRepositorioLeituraProcessos repositorio;

        public CarregaDadosHierarquiaProcesso(IRepositorioLeituraProcessos documentDb)
        {
            this.repositorio = documentDb;
        }

        async Task<IEnumerable<DadosHierarquiaProcesso>> IRequestHandler<CarregarDadosHierarquiaProcesso, IEnumerable<DadosHierarquiaProcesso>>.Handle(CarregarDadosHierarquiaProcesso request, CancellationToken cancellationToken)
        {
            var dados = await repositorio.LoadHierarquia(request.AggregateId);
            return dados;
        }
    }
}
