module TesteNomeResponsavel

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

type ValidInputData()=
    let values: seq<obj[]>=
        seq {
           yield [|"nome"|]
           yield [|"nome_".PadRight(150)|]
        }
    interface seq<obj[]> with
        member this.GetEnumerator() = values.GetEnumerator()
        member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

[<Theory>]
[<ClassData(typeof<ValidInputData>)>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorNomeResponsavel().Validate(NomeResponsavel(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())


type InvalidInputData()=
    let values: seq<obj[]>=
        seq {
           yield [|null|]
           yield [|""|]
           yield [|"nome_".PadRight(151)|]
        }
    interface seq<obj[]> with
        member this.GetEnumerator() = values.GetEnumerator()
        member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

[<Theory>]
[<ClassData(typeof<InvalidInputData>)>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorNomeResponsavel().Validate(NomeResponsavel(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)