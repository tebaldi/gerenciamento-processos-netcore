using FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    class Eventos_Processos : AbstractMultiMapIndexCreationTask<DadosProcesso>
    {
        public Eventos_Processos()
        {
            AddMap<ProcessoAtualizado>(eventos =>
                from e in eventos
                where e.EventName == nameof(ProcessoAtualizado)
                select new DadosProcesso
                {
                    AggregateId = e.AggregateId,
                    AggregateVersion = e.AggregateVersion,
                    NumeroProcessoUnificado = e.NumeroProcessoUnificado,
                    PastaFisicaCliente = e.PastaFisicaCliente,
                    DescricaoProcesso = e.DescricaoProcesso,
                    SituacaoProcesso = e.SituacaoProcesso,
                    DataDistribuicao = e.DataDistribuicao,
                    SegredoDeJustica = e.SegredoDeJustica,
                    Finalizado = e.Finalizado,
                    ProcessoPai = new DadosProcessoPai
                    {
                        AggregateId = e.ProcessoPai.AggregateId,
                        NumeroProcessoUnificado = e.ProcessoPai.NumeroProcessoUnificado
                    },
                    Responsaveis = e.Responsaveis.Select(r => new DadosResponsavelProcesso
                    {
                        AggregateId = r.AggregateId,
                        Email = r.Email,
                        Nome = r.Nome
                    }),
                    Removido = false
                });

            AddMap<ProcessoRemovido>(eventos =>
                from e in eventos
                where e.EventName == nameof(ProcessoRemovido)
                select new DadosProcesso
                {
                    AggregateId = e.AggregateId,
                    AggregateVersion = e.AggregateVersion,
                    NumeroProcessoUnificado = e.NumeroProcessoUnificado,
                    PastaFisicaCliente = e.PastaFisicaCliente,
                    DescricaoProcesso = e.DescricaoProcesso,
                    SituacaoProcesso = e.SituacaoProcesso,
                    DataDistribuicao = e.DataDistribuicao,
                    SegredoDeJustica = e.SegredoDeJustica,
                    Finalizado = e.Finalizado,
                    ProcessoPai = new DadosProcessoPai
                    {
                        AggregateId = e.ProcessoPai.AggregateId,
                        NumeroProcessoUnificado = e.ProcessoPai.NumeroProcessoUnificado
                    },
                    Responsaveis = e.Responsaveis.Select(r => new DadosResponsavelProcesso
                    {
                        AggregateId = r.AggregateId,
                        Email = r.Email,
                        Nome = r.Nome
                    }),
                    Removido = true
                });

            Reduce = results =>
                from r in results
                group r by r.AggregateId into g
                select new DadosProcesso
                {
                    AggregateId = g.Key,
                    AggregateVersion = g.OrderByDescending(s => s.AggregateVersion).First().AggregateVersion,
                    NumeroProcessoUnificado = g.OrderByDescending(s => s.AggregateVersion).First().NumeroProcessoUnificado,
                    PastaFisicaCliente = g.OrderByDescending(s => s.AggregateVersion).First().PastaFisicaCliente,
                    DescricaoProcesso = g.OrderByDescending(s => s.AggregateVersion).First().DescricaoProcesso,
                    SituacaoProcesso = g.OrderByDescending(s => s.AggregateVersion).First().SituacaoProcesso,
                    DataDistribuicao = g.OrderByDescending(s => s.AggregateVersion).First().DataDistribuicao,
                    SegredoDeJustica = g.OrderByDescending(s => s.AggregateVersion).First().SegredoDeJustica,
                    Finalizado = g.OrderByDescending(s => s.AggregateVersion).First().Finalizado,
                    ProcessoPai = g.OrderByDescending(s => s.AggregateVersion).First().ProcessoPai,
                    Responsaveis = g.OrderByDescending(s => s.AggregateVersion).First().Responsaveis,
                    Removido = g.Any(s => s.Removido)
                };

            OutputReduceToCollection = "Processos";
        }
    }
}
