using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas
{
    public class ConsultarDadosResponsaveis : IRequest<DadosPaginados<DadosResponsavel>>
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string NumeroProcesso { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
