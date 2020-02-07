using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados
{
    public class DadosHierarquiaProcesso
    {
        public int Nivel { get; set; }
        public DadosProcesso Processo { get; set; }
    }
}
