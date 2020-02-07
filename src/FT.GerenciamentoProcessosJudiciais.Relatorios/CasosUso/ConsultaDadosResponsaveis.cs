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
    class ConsultaDadosResponsaveis : IRequestHandler<ConsultarDadosResponsaveis, DadosPaginados<DadosResponsavel>>
    {
        private readonly IRepositorioLeituraResponsaveis repositorio;

        public ConsultaDadosResponsaveis(IRepositorioLeituraResponsaveis documentDb)
        {
            this.repositorio = documentDb;
        }

        async Task<DadosPaginados<DadosResponsavel>> IRequestHandler<ConsultarDadosResponsaveis, DadosPaginados<DadosResponsavel>>.Handle(ConsultarDadosResponsaveis request, CancellationToken cancellationToken)
        {
            var specificationResponsaveis = new List<Expression<Func<DadosResponsavel, bool>>>();
            var specificationProcessos = new List<Expression<Func<DadosProcesso, bool>>>();

            specificationResponsaveis.Add(x => x.Removido == false);

            if (!string.IsNullOrWhiteSpace(request.Cpf))
            {
                request.Cpf = DadosResponsavel.FormatarCpf(request.Cpf);
                specificationResponsaveis.Add(x => Regex.IsMatch(x.Cpf, $"^{request.Cpf}$"));
            }

            if (!string.IsNullOrWhiteSpace(request.Nome))
                specificationResponsaveis.Add(x => Regex.IsMatch(x.Nome, $"(?i){request.Nome}"));

            if (!string.IsNullOrWhiteSpace(request.NumeroProcesso))
            {
                request.NumeroProcesso = DadosProcesso.FormatarNumero(request.NumeroProcesso);
                specificationProcessos.Add(x => Regex.IsMatch(x.NumeroProcessoUnificado, $"^{request.NumeroProcesso}$"));
            }

            var predicateResponsaveis = specificationResponsaveis.Any() ?
                specificationResponsaveis.Aggregate((s1, s2) => s1.And(s2))
                : default;

            var predicataeProcessos = specificationProcessos.Any() ?
                specificationProcessos.Aggregate((s1, s2) => s1.And(s2))
                : default;

            var dados = await repositorio.Query(predicateResponsaveis, predicataeProcessos, request.PageIndex, request.PageSize);
            return dados;
        }
    }
}
