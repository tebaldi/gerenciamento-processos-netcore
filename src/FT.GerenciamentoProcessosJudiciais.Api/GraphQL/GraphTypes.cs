using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using GraphQL.Types;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Api.GraphQL
{
    public class GraphTypes
    {
        public static IEnumerable<IGraphType> GetTypes()
        {
            var responsavelType = new ObjectGraphType
            {
                Name = "Responsavel"
            };
            responsavelType.Field<StringGraphType>().Name("nome");
            yield return responsavelType;
        }

        public static string MappedTypes => @"
type Responsavel {
    aggregateId: ID
    aggregateVersion: ID
    nome: String
    cpf: String
    email: String
    removido: Boolean
}

input ConsultarDadosResponsaveis {
    pageSize: Int
    pageIndex: Int
}

input AtualizarResponsavel {
    aggregateId: ID!
    nome: String!
    cpf: String!
    email: String!
}

type Query {
    responsaveis(input:ConsultarDadosResponsaveis): [Responsavel]
}

type Mutation {
    atualizarResponsavel(input:AtualizarResponsavel): Responsavel
    removerResponsavel(aggregateId:ID): Responsavel
  }
";
    }
}
