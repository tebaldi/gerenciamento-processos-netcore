using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Api.GraphQL
{
    public class GraphSchema : Schema
    {
        public GraphSchema(ILogger<GraphSchema> logger, IDependencyResolver dependencyResolver)
        {
            var schema = For(GraphTypes.MappedTypes, _ =>
            {
                _.DependencyResolver = dependencyResolver;
                _.Types.Include<GraphQueries>("Query");
                _.Types.Include<GraphMutations>("Mutation");
            });
            Query = schema.Query;
            Mutation = schema.Mutation;

            var schemaPrint = new SchemaPrinter(this).Print();
            logger.LogDebug(schemaPrint);
        }
    }
}
