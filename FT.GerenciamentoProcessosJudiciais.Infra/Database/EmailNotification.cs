using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    public class EmailNotification
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}