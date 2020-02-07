using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Mailing
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }

        public override string ToString() => $"{Host} {UserName} {Password} {Port} {UseSsl}";
    }
}
