using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas
{
    public class ConsultarDadosProcessos : IRequest<DadosPaginados<DadosProcesso>>
    {
        public string NumeroProcessoUnificado { get; set; }
        public string PastaFisicaCliente { get; set; }
        public string SituacaoProcesso { get; set; }
        public string Responsavel { get; set; }
        public DateTime? DataDistribuicaoInicial { get; set; }
        public DateTime? DataDistribuicaoFinal { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
