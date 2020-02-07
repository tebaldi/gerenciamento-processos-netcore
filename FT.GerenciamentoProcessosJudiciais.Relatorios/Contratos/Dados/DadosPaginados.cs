using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados
{
    public class DadosPaginados<T>
    {
        public IEnumerable<T> Dados { get; set; }

        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
