module TesteIdProcesso

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

[<Theory>]
[<InlineData("id")>]
[<InlineData("123")>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorIdProcesso().Validate(IdProcesso(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("00000000-0000-0000-0000-000000000000")>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorIdProcesso().Validate(IdProcesso(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)