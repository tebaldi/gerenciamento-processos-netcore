using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados
{
    public class DadosProcesso
    {
        public string AggregateId { get; set; }
        public long AggregateVersion { get; set; }
        public string NumeroProcessoUnificado { get; set; }
        public string PastaFisicaCliente { get; set; }
        public string DescricaoProcesso { get; set; }
        public string SituacaoProcesso { get; set; }
        public DateTime DataDistribuicao { get; set; }
        public bool SegredoDeJustica { get; set; }
        public bool Finalizado { get; set; }
        public bool Removido { get; set; }
        public DadosProcessoPai ProcessoPai { get; set; }
        public IEnumerable<DadosResponsavelProcesso> Responsaveis { get; set; }

        public static string FormatarNumero(string numero)
        {
            if (string.IsNullOrEmpty(numero))
                return numero;

            numero = numero.Replace(".", "").Replace("-", "").PadLeft(20, '0');
            return Regex.Replace($"{numero}", @"^(\d{7})(\d{2})(\d{4})(\d{3})(\d{4})$", "$1-$2.$3.$4.$5");//NNNNNNN-DD.AAAA.JTR.OOOO
        }
    }
}
