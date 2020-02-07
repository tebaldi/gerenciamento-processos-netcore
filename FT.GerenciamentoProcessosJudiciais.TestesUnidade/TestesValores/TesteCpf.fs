module TesteCpf

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

[<Theory>]
[<InlineData("805.549.230-17")>]
[<InlineData("63612361023")>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorCpf().Validate(Cpf(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorCpf().Validate(Cpf(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)