using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
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
    public class GraphQueries
    {
        private readonly IMediator mediator;
        public GraphQueries(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [GraphQLMetadata("responsaveis")]
        public async Task<IEnumerable<dynamic>> GetResponsaveis(ConsultarDadosResponsaveis input)
        {
            var result = await mediator.Send(input);
            return result.Dados;
        }
    }
}
