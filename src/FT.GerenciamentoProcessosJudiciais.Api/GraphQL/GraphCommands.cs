using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using GraphQL.Types;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Api.GraphQL
{
    public class GraphCommands : ObjectGraphType
    {
        public GraphCommands(IMediator mediator)
        {
            FieldAsync<ResponsavelGraphType>("atualizarResponsavel", arguments: new QueryArguments(new[]
            {
                new QueryArgument<NonNullGraphType<ResponsavelInputGraphType>>{ Name ="responsavel"}
            }), resolve: async context =>
            {
                var dados = context.GetArgument<DadosResponsavel>("responsavel");
                var response = await mediator.Send(new AtualizarResponsavel
                {
                    AggregateId = dados.AggregateId,
                    Email = dados.Email,
                    Cpf = dados.Cpf,
                    Nome = dados.Nome,
                    Foto = dados.Foto
                });
                if (response.IsInvalido)
                    throw new InvalidOperationException($"{response}");
                else
                    return dados;
            });
        }
    }
}
