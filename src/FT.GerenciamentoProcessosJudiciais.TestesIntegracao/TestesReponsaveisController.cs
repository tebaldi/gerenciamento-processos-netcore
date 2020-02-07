using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FT.GerenciamentoProcessosJudiciais.TestesIntegracao
{
    public class TestesReponsaveisController
    {
        [Fact]
        public async Task ShouldCallApi()
        {
            await ShouldPost();
            await ShouldPut();
            await ShouldGet();
            await ShouldLoad();
            await ShouldDelete();
        }

        async Task ShouldGet()
        {
            var controller = "/responsaveis/v1";
            var dados = await controller.ApiGetAsync<DadosPaginados<DadosResponsavel>>(new ConsultarDadosResponsaveis
            {
            });

            Assert.NotNull(dados);
        }

        async Task ShouldLoad()
        {
            var id = "resp-01";
            var controller = $"/responsaveis/v1/{id}";
            var dados = await controller.ApiGetAsync<DadosResponsavel>(new CarregarDadosResponsavel
            {
                AggregateId = id
            });

            Assert.NotNull(dados);
        }

        async Task ShouldPost()
        {
            var controller = "/responsaveis/v1";
            var dados = await controller.ApiPostAsync<ResponsavelAtualizado>(new AtualizarResponsavel
            {
                AggregateId = $"resp-01",
                Cpf = "805.549.230-17",
                Email = "fabio@gmail.com",
                Nome = "Fabio",
                Foto = "path.jpg"
            });

            Assert.NotNull(dados);
        }

        async Task ShouldPut()
        {
            var id = "resp-01";
            var controller = $"/responsaveis/v1/{id}";
            var dados = await controller.ApiPutAsync<ResponsavelAtualizado>(new AtualizarResponsavel
            {
                AggregateId = id,
                Cpf = "805.549.230-17",
                Email = "fabio@gmail.com",
                Nome = "Fabio",
                Foto = "path atu.jpg"
            });

            Assert.NotNull(dados);
        }

        async Task ShouldDelete()
        {
            var id = "resp-01";
            var controller = $"/responsaveis/v1/{id}";
            var dados = await controller.ApiDeleteAsync<ResponsavelRemovido>(new RemoverResponsavel
            {
                AggregateId = id,
            });

            Assert.NotNull(dados);
        }
    }
}
