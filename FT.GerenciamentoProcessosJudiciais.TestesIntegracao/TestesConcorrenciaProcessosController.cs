using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System.Collections.Concurrent;
using FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos;
using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;

namespace FT.GerenciamentoProcessosJudiciais.TestesIntegracao
{
    public class TestesConcorrenciaProcessosController
    {
        [Fact]
        public void ShouldStressServer()
        {
            var exceptions = new ConcurrentQueue<Exception>();
            var tasks = new List<Func<Task>>();
            for (var i = 0; i < 1000; i++)
                tasks.Add(() => PostNew());

            try
            {
                Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    t =>
                    {
                        try
                        {
                            t().Wait();
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    });

                if (exceptions.Count > 0)
                    Debug.Fail($"requests failed: {exceptions.Count}");
            }
            catch (Exception)
            {
                Debug.Fail($"requests failed: {exceptions.Count}");
            }
        }

        [Fact]
        public void ShouldStressAggregate()
        {
            var exceptions = new ConcurrentQueue<Exception>();
            var tasks = new List<Func<Task>>();
            for (var i = 0; i < 100; i++)
                tasks.Add(() => PutChange());

            try
            {
                Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    t =>
                    {
                        try
                        {
                            t().Wait();
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    });

                if (exceptions.Count > 0)
                    Debug.Fail($"requests failed: {exceptions.Count}");
            }
            catch (Exception)
            {
                Debug.Fail($"requests failed: {exceptions.Count}");
            }
        }

        async Task PostNew()
        {
            try
            {
                var numero = new Random().Next(1, int.MaxValue);
                var controller = "/processos/v1";
                var dados = await controller.ApiPostAsync<ProcessoAtualizado>(new AtualizarProcesso
                {
                    AggregateId = $"proc-{Guid.NewGuid()}",
                    NumeroProcessoUnificado = $"{numero}",
                    PastaFisicaCliente = "pasta",
                    SegredoDeJustica = true,
                    SituacaoProcesso = SituacoesProcesso.EmAndamento.ToString(),
                    DataDistribuicao = DateTime.Today.AddDays(-1),
                    DescricaoProcesso = "descricao",
                    ProcessoPai = new ProcessoPaiAtualizacao(string.Empty, string.Empty),
                    Responsaveis = new[] { new ResponsavelProcessoAtualizacao
                {
                    AggregateId = "id", Nome ="nome", Email ="mail@mail.com"
                }}
                });
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }

        async Task PutChange()
        {
            var numero = new Random().Next(1, int.MaxValue);
            var id = "proc-stress";
            var controller = $"/processos/v1/{id}";
            var dados = await controller.ApiPutAsync<ProcessoAtualizado>(new AtualizarProcesso
            {
                AggregateId = id,
                NumeroProcessoUnificado = $"{numero}",
                PastaFisicaCliente = "pasta",
                SegredoDeJustica = true,
                SituacaoProcesso = SituacoesProcesso.EmAndamento.ToString(),
                DataDistribuicao = DateTime.Today.AddDays(-1),
                DescricaoProcesso = "descricao",
                ProcessoPai = new ProcessoPaiAtualizacao("proc-stress-pai", "12345678"),
                Responsaveis = new[] { new ResponsavelProcessoAtualizacao
                {
                    AggregateId = "id", Nome ="nome", Email ="mail@mail.com"
                }}
            });
        }
    }
}
