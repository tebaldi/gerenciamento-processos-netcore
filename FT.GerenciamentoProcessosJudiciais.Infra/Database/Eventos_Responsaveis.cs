using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Database
{
    class Eventos_Responsaveis : AbstractMultiMapIndexCreationTask<DadosResponsavel>
    {
        public Eventos_Responsaveis()
        {
            AddMap<ResponsavelAtualizado>(eventos =>
                from e in eventos
                where e.EventName == nameof(ResponsavelAtualizado)
                select new DadosResponsavel
                {
                    AggregateId = e.AggregateId,
                    AggregateVersion = e.AggregateVersion,
                    Cpf = e.Cpf,
                    Email = e.Email,
                    Foto = e.Foto,
                    Nome = e.Nome,
                    Removido = false
                });

            AddMap<ResponsavelRemovido>(eventos =>
                from e in eventos
                where e.EventName == nameof(ResponsavelRemovido)
                select new DadosResponsavel
                {
                    AggregateId = e.AggregateId,
                    AggregateVersion = e.AggregateVersion,
                    Cpf = e.Cpf,
                    Email = e.Email,
                    Foto = e.Foto,
                    Nome = e.Nome,
                    Removido = true
                });

            Reduce = results =>
                from r in results
                group r by r.AggregateId into g
                select new DadosResponsavel
                {
                    AggregateId = g.Key,
                    AggregateVersion = g.OrderByDescending(s => s.AggregateVersion).First().AggregateVersion,
                    Cpf = g.OrderByDescending(s => s.AggregateVersion).First().Cpf,
                    Nome = g.OrderByDescending(s => s.AggregateVersion).First().Nome,
                    Email = g.OrderByDescending(s => s.AggregateVersion).First().Email,
                    Foto = g.OrderByDescending(s => s.AggregateVersion).First().Foto,
                    Removido = g.Any(s => s.Removido)
                };

            OutputReduceToCollection = "Responsaveis";
        }
    }
}
