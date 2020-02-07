using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    public class LeaderElector
    {
        public string LeaderId { get; set; }
        public DateTime LeaseLockedTime { get; set; }
        public int LeaseLockSecondsTimeout { get; set; }
    }
}