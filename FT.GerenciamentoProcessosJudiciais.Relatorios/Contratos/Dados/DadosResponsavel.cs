using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados
{
    public class DadosResponsavel
    {
        public string AggregateId { get; set; }
        public long AggregateVersion { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public bool Removido { get; set; }

        public static string FormatarCpf(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "").PadLeft(11, '0');
            long.TryParse(cpf, out var icpf);
            return Regex.Replace($"{icpf}", @"^(\d{3})(\d{3})(\d{3})(\d{2})$", "$1.$2.$3-$4");
        }
    }
}
