using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FT.GerenciamentoProcessosJudiciais.TestesIntegracao
{
    public class TestesGraphQL
    {
        [Fact]
        public async Task ShouldQueryAndMutate()
        {
            await ShouldQueryResponsaveis();
        }

        async Task ShouldQueryResponsaveis()
        {
            var controller = "/graphql";
            var data = new
            {
                query = "responsaveis(input:$consultarReponsavel) { nome aggregateId aggregateVersion }",
                variables = "{ \"consultarReponsavel\":{\"pageSize\": 5} }"
            };
            var dados = await controller.ApiPostAsync<IEnumerable<DadosResponsavel>>(data);
            Assert.NotNull(dados);
        }
    }
}
