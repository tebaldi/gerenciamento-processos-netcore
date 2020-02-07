using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos;
using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using GraphQL;
using GraphQL.Types;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Api.GraphQL
{
    public class GraphMutations
    {
        private readonly IMediator mediator;

        public GraphMutations(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [GraphQLMetadata("atualizarResponsavel")]
        public async Task<dynamic> AtualizarResponsavel(AtualizarResponsavel input)
        {
            var result = await mediator.Send(input);
            return result.IsValido ? result.As<Validacao<ResponsavelAtualizado>.Valido>().Item : default;
        }

        [GraphQLMetadata("removerResponsavel")]
        public async Task<dynamic> RemoverResponsavel(string aggregateId)
        {
            var result = await mediator.Send(new RemoverResponsavel
            {
                AggregateId = aggregateId   
            });
            return result.IsValido ? result.As<Validacao<ResponsavelRemovido>.Valido>().Item : default;
        }
    }
}
