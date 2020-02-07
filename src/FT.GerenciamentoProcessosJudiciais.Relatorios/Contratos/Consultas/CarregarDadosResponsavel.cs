﻿using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas
{
    public class CarregarDadosResponsavel : IRequest<DadosResponsavel>
    {
        public string AggregateId { get; set; }
    }
}
