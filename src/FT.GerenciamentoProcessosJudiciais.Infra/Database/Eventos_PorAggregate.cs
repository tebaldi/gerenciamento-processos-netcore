using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    class Eventos_PorAggregate : AbstractIndexCreationTask<EventoPadrao>
    {
        public Eventos_PorAggregate()
        {
            Map = eventos => from e in eventos
                             select new
                             {
                                 AggregateId = e.AggregateId,
                                 AggregateVersion = e.AggregateVersion
                             };
        }
    }
}
