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
    class ConsultaDadosProcessos : IRequestHandler<ConsultarDadosProcessos, DadosPaginados<DadosProcesso>>
    {
        private readonly IRepositorioLeituraProcessos repositorio;

        public ConsultaDadosProcessos(IRepositorioLeituraProcessos documentDb)
        {
            this.repositorio = documentDb;
        }

        async Task<DadosPaginados<DadosProcesso>> IRequestHandler<ConsultarDadosProcessos, DadosPaginados<DadosProcesso>>.Handle(ConsultarDadosProcessos request, CancellationToken cancellationToken)
        {
            var specification = new List<Expression<Func<DadosProcesso, bool>>>();

            specification.Add(x => x.Removido == false);

            if (!string.IsNullOrWhiteSpace(request.NumeroProcessoUnificado))
            {
                request.NumeroProcessoUnificado = DadosProcesso.FormatarNumero(request.NumeroProcessoUnificado);
                specification.Add(x => Regex.IsMatch(x.NumeroProcessoUnificado, $"^{request.NumeroProcessoUnificado}$"));
            }

            if (!string.IsNullOrWhiteSpace(request.PastaFisicaCliente))
                specification.Add(x => Regex.IsMatch(x.PastaFisicaCliente, $"(?i){request.PastaFisicaCliente}"));

            if (!string.IsNullOrWhiteSpace(request.Responsavel))
                specification.Add(x => x.Responsaveis.Any(r => Regex.IsMatch(r.Nome, $"(?i){request.Responsavel}")));

            if (!string.IsNullOrWhiteSpace(request.SituacaoProcesso))
                specification.Add(x => Regex.IsMatch(x.SituacaoProcesso, $"^{request.SituacaoProcesso}$"));

            if (request.DataDistribuicaoInicial.HasValue)
                specification.Add(x => x.DataDistribuicao >= request.DataDistribuicaoInicial);

            if (request.DataDistribuicaoFinal.HasValue)
                specification.Add(x => x.DataDistribuicao <= request.DataDistribuicaoFinal);

            var predicatae = specification.Any() ?
                specification.Aggregate((s1, s2) => s1.And(s2))
                : default;

            var dados = await repositorio.Query(predicatae, request.PageIndex, request.PageSize);
            return dados;
        }
    }
}
