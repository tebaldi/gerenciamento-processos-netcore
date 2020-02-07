module TesteResponsavel

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos

[<Fact>]
let ``Deve Construir Responsavel Vazio`` () =
    let aggregate = Responsavel.Construir([])
    match aggregate with
        | Ativo state ->
            Assert.Equal("", state.aggregateId.Valor)
            Assert.Equal("", state.nome.Valor)
            Assert.Equal("", state.cpf.Valor)
            Assert.Equal("", state.email.Valor)
            Assert.Equal("", state.foto.Valor)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Deve Construir Responsavel Atualizado`` () =
    let evento = ResponsavelAtualizado("test-id", "", "nome", "805.549.230-17", "email@email.com", "")
    let aggregate = Responsavel.Construir([evento])
    match aggregate with
        | Ativo state ->
            Assert.Equal(evento.AggregateId, state.aggregateId.Valor)
            Assert.Equal(evento.Nome, state.nome.Valor)
            Assert.Equal(evento.Cpf, state.cpf.Valor)
            Assert.Equal(evento.Email, state.email.Valor)
            Assert.Equal(evento.Foto, state.foto.Valor)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Deve Construir Responsavel Removido`` () =
    let evento = ResponsavelRemovido("test-id", "", "nome", "805.549.230-17", "email@email.com", "")
    let aggregate = Responsavel.Construir([evento])
    match aggregate with
        | Inativo state -> Assert.Equal(evento.AggregateId, state.aggregateId.Valor)
        | _ -> raise(InvalidOperationException())