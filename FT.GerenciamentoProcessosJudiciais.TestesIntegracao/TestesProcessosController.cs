using FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos;
using FT.GerenciamentoProcessosJudiciais.Dominio.Valores;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FT.GerenciamentoProcessosJudiciais.TestesIntegracao
{
    public class TestesProcessosController
    {
        [Fact]
        public async Task ShouldCallApi()
        {
            await ShouldPost();
            await ShouldPut();
            await ShouldGet();
            await ShouldLoad();
            await ShouldLoadHierarquia();
            await ShouldDelete();
        }

        async Task ShouldGet()
        {
            var controller = "/processos/v1";
            var dados = await controller.ApiGetAsync<DadosPaginados<DadosProcesso>>(new ConsultarDadosProcessos
            {
            });

            Assert.NotNull(dados);
        }

        async Task ShouldLoad()
        {
            var id = "proc-01";
            var controller = $"/processos/v1/{id}";
            var dados = await controller.ApiGetAsync<DadosProcesso>(new CarregarDadosProcesso
            {
                AggregateId = id
            });

            Assert.NotNull(dados);
        }

        async Task ShouldLoadHierarquia()
        {
            var id = "proc-01";
            var controller = $"/processos/v1/{id}/hierarquia";
            var dados = await controller.ApiGetAsync<IEnumerable<DadosHierarquiaProcesso>>(new CarregarDadosHierarquiaProcesso
            {
                AggregateId = id
            });

            Assert.NotNull(dados);
        }

        async Task ShouldPost()
        {
            var controller = "/processos/v1";
            var dados = await controller.ApiPostAsync<ProcessoAtualizado>(new AtualizarProcesso
            {
                AggregateId = $"proc-01",
                NumeroProcessoUnificado = "8055492-30.1722.123.4567",
                PastaFisicaCliente = "pasta",
                SegredoDeJustica = true,
                SituacaoProcesso = SituacoesProcesso.EmAndamento.ToString(),
                DataDistribuicao = DateTime.Today.AddDays(-1),
                DescricaoProcesso = "descricao",
                ProcessoPai = new ProcessoPaiAtualizacao { AggregateId = "idpai", NumeroProcessoUnificado = "7055492-30.1722.123.4567" },
                Responsaveis = new[] { new ResponsavelProcessoAtualizacao
                {
                    AggregateId = "id", Nome ="nome", Email ="mail@mail.com"
                }}
            });

            Assert.NotNull(dados);
        }

        async Task ShouldPut()
        {
            var id = "proc-01";
            var controller = $"/processos/v1/{id}";
            var dados = await controller.ApiPutAsync<ProcessoAtualizado>(new AtualizarProcesso
            {
                AggregateId = id,
                NumeroProcessoUnificado = "8055492-30.1722.123.4567",
                PastaFisicaCliente = "pasta",
                SegredoDeJustica = true,
                SituacaoProcesso = SituacoesProcesso.EmAndamento.ToString(),
                DataDistribuicao = DateTime.Today.AddDays(-1),
                DescricaoProcesso = "descricao atualizada",
                ProcessoPai = new ProcessoPaiAtualizacao { AggregateId = "idpai", NumeroProcessoUnificado = "7055492-30.1722.123.4567" },
                Responsaveis = new[]
                {
                    new ResponsavelProcessoAtualizacao
                    {
                        AggregateId = "id", Nome ="nome", Email ="mail@mail.com"
                    },
                    new ResponsavelProcessoAtualizacao
                    {
                        AggregateId = "id2", Nome ="nome 2", Email ="mail2@mail.com"
                    }
                }
            });

            Assert.NotNull(dados);
        }

        async Task ShouldDelete()
        {
            var id = "proc-01";
            var controller = $"/processos/v1/{id}";
            var dados = await controller.ApiDeleteAsync<ProcessoRemovido>(new RemoverProcesso
            {
                AggregateId = id,
            });

            Assert.NotNull(dados);
        }
    }
}
