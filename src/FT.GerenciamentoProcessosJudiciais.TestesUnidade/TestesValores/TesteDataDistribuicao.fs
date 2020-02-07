module TesteDataDistribuicao

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

type ValidInputData()=
    let values: seq<obj[]>=
        seq {
           yield [|DateTime.MinValue|]
           yield [|DateTime.MinValue.ToUniversalTime()|]
           yield [|DateTime.Now|]
           yield [|DateTime.Now.ToUniversalTime()|]
        }
    interface seq<obj[]> with
        member this.GetEnumerator() = values.GetEnumerator()
        member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

[<Theory>]
[<ClassData(typeof<ValidInputData>)>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorDataDistribuicao().Validate(DataDistribuicao(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())

type InvalidInputData()=
    let values: seq<obj[]>=
        seq {
           yield [|DateTime.Now.AddDays(1.0)|]
           yield [|DateTime.Now.AddDays(1.0).ToUniversalTime() |]
        }
    interface seq<obj[]> with
        member this.GetEnumerator() = values.GetEnumerator()
        member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

[<Theory>]
[<ClassData(typeof<InvalidInputData>)>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorDataDistribuicao().Validate(DataDistribuicao(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)