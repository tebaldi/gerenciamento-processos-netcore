using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    public class Eventos_PorCreatedTime : AbstractIndexCreationTask<EventoPadrao>
    {
        public Eventos_PorCreatedTime()
        {
            Map = eventos => from e in eventos
                             select new
                             {
                                 e.EventCreatedTime
                             };
        }
    }
}
