module TesteEmail

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

[<Theory>]
[<InlineData("fabiotebaldi@gmail.com")>]
[<InlineData("fabiotebaldi@email.com.br")>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorEmail().Validate(Email(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("fabiotebaldi@@email.com")>]
[<InlineData("fabiotebaldi@email")>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorEmail().Validate(Email(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)