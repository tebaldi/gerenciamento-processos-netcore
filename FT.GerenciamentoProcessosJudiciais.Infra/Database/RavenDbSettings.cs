using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    public class RavenDbSettings
    {
        public string Urls { get; set; }
        public string DatabaseName { get; set; }

        public override string ToString() => $"{Urls} {DatabaseName}";
    }
}
