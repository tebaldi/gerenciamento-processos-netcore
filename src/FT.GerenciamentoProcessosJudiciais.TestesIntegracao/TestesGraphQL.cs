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
            await ShouldMutateResponsavel();
            await ShouldQueryResponsaveis();
        }

        async Task ShouldQueryResponsaveis()
        {
            var dados = await @"
query ($consultarReponsavel:ConsultarDadosResponsaveis){
    responsaveis(input:$consultarReponsavel) { nome aggregateId aggregateVersion }
}
".GraphQLAsync(new
            {
                consultarReponsavel = new { pageSize = 5 }
            });

            Assert.NotNull(dados);
        }

        async Task ShouldMutateResponsavel()
        {
            var dados = await @"
mutation ($responsavel:AtualizarResponsavel){
    atualizarResponsavel(input:$responsavel) { nome }
}
".GraphQLAsync(new
            {
                responsavel = new { aggregateId = "resp GT", nome = "Resp GT", email = "mail@mail.com", cpf = "123" }
            });

            Assert.NotNull(dados);
        }
    }
}
