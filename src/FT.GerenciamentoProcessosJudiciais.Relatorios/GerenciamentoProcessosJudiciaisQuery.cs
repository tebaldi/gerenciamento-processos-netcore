using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using GraphQL.Types;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios
{
    public class GerenciamentoProcessosJudiciaisQuery : ObjectGraphType
    {
        public GerenciamentoProcessosJudiciaisQuery(IMediator mediator)
        {
            FieldAsync<ListGraphType<ResponsavelGraphType>>("responsaveis", arguments: new QueryArguments(new[]
            {
                new QueryArgument<IntGraphType>{ Name = "pageIndex" },
                new QueryArgument<IntGraphType>{ Name = "pageSize" },
            }), resolve: async context =>
            {
                var result = await mediator.Send(new ConsultarDadosResponsaveis
                {
                    PageIndex = context.GetArgument<int?>("pageIndex") ?? 0,
                    PageSize = context.GetArgument<int?>("pageSize") ?? 1000
                });
                return result.Dados;
            });

            FieldAsync<ResponsavelGraphType>("responsavel", arguments: new QueryArguments(new[]
            {
                new QueryArgument<StringGraphType>{ Name = "aggregateId" },
            }), resolve: async context =>
            {
                var result = await mediator.Send(new CarregarDadosResponsavel
                {
                    AggregateId = context.GetArgument<string>("aggregateId")
                });
                return result;
            });
        }
    }

    public class ResponsavelGraphType : ObjectGraphType<DadosResponsavel>
    {
        public ResponsavelGraphType()
        {
            Name = "Responsavel";
            Field(x => x.AggregateId);
            Field(x => x.AggregateVersion);
            Field(x => x.Cpf);
            Field(x => x.Nome);
            Field(x => x.Email);
            Field(x => x.Foto);
        }
    }
}
