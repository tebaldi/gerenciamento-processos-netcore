module TesteProcesso

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

[<Fact>]
let ``Deve Construir Processo Vazio`` () =
    let aggregate = Processo.Construir([])
    match aggregate with
        | Ativo state ->
            Assert.Equal("", state.aggregateId.Valor)
            Assert.Equal("", state.estado.numero.Valor)
            Assert.Equal("", state.estado.descricao.Valor)
            Assert.Equal("", state.estado.pasta.Valor)
            Assert.Equal("", state.estado.processoPai.aggregateId.Valor)
            Assert.Equal("", state.estado.processoPai.numero.Valor)
            Assert.Equal(DateTime.MinValue.ToUniversalTime(), state.estado.dataDistribuicao.Valor)
            Assert.Equal(SituacoesProcesso.NaoInformada, state.estado.situacao.Valor)
            Assert.False(state.estado.segredo)
            Assert.Empty(state.estado.responsaveis)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Deve Construir Processo Atualizado`` () =
    let evento = new ProcessoAtualizado(
                    "test-id", "", "8055492-30.1722.123.4567", "pasta", "descricao", "EmAndamento",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiAtualizado("test-pid", "7055492-30.1722.123.4567"),
                    [|ResponsavelProcessoAtualizado("r-id", "nome","emial@email.com")|])
    let aggregate = Processo.Construir([evento])
    match aggregate with
        | Ativo state ->
            Assert.Equal(evento.AggregateId, state.aggregateId.Valor)
            Assert.Equal(evento.NumeroProcessoUnificado, state.estado.numero.Valor)
            Assert.Equal(evento.DescricaoProcesso, state.estado.descricao.Valor)
            Assert.Equal(evento.PastaFisicaCliente, state.estado.pasta.Valor)
            Assert.Equal(evento.ProcessoPai.AggregateId, state.estado.processoPai.aggregateId.Valor)
            Assert.Equal(evento.ProcessoPai.NumeroProcessoUnificado, state.estado.processoPai.numero.Valor)
            Assert.Equal(evento.DataDistribuicao.ToUniversalTime(), state.estado.dataDistribuicao.Valor)
            Assert.Equal(evento.SituacaoProcesso, state.estado.situacao.Valor.ToString())
            Assert.True(state.estado.segredo)
            Assert.NotEmpty(state.estado.responsaveis)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Deve Construir Processo Removido`` () =
    let evento = ProcessoRemovido(
                    "test-id", "", "8055492-30.1722.123.4567", "pasta", "descricao", "EmAndamento",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiRemovido("test-pid", "7055492-30.1722.123.4567"),
                    [|ResponsavelProcessoRemovido("r-id", "nome","emial@email.com")|])
    let aggregate = Processo.Construir([evento])
    match aggregate with
        | Inativo state -> Assert.Equal(evento.AggregateId, state.aggregateId.Valor)
        | _ -> raise(InvalidOperationException())