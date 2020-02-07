module TesteSituacaoProcesso

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

[<Theory>]
[<InlineData(SituacoesProcesso.Desmembrado)>]
[<InlineData(SituacoesProcesso.EmAndamento)>]
[<InlineData(SituacoesProcesso.EmRecurso)>]
[<InlineData(SituacoesProcesso.Arquivado)>]
[<InlineData(SituacoesProcesso.Finalizado)>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorSituacaoProcesso().Validate(SituacaoProcesso(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())

[<Theory>]
[<InlineData(SituacoesProcesso.NaoInformada)>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorSituacaoProcesso().Validate(SituacaoProcesso(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)