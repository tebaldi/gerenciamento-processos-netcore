using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios
{
    public class GerenciamentoProcessosJudiciaisSchema : Schema
    {
        public GerenciamentoProcessosJudiciaisSchema(GerenciamentoProcessosJudiciaisQuery query)
        {
            Query = query;
        }
    }
}
