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
    class CarregaDadosResponsavel : IRequestHandler<CarregarDadosResponsavel, DadosResponsavel>
    {
        private readonly IRepositorioLeituraResponsaveis repositorio;

        public CarregaDadosResponsavel(IRepositorioLeituraResponsaveis documentDb)
        {
            this.repositorio = documentDb;
        }

        async Task<DadosResponsavel> IRequestHandler<CarregarDadosResponsavel, DadosResponsavel>.Handle(CarregarDadosResponsavel request, CancellationToken cancellationToken)
        {
            var dados = await repositorio.Load(request.AggregateId);
            return dados;
        }
    }
}
