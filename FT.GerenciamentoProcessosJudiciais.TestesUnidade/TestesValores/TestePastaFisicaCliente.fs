module TestePastaFisicaCliente

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

type ValidInputData()=
    let values: seq<obj[]>=
        seq {
           yield [|null|]
           yield [|""|]
           yield [|"pasta"|]
           yield [|"pasta_".PadRight(50)|]
        }
    interface seq<obj[]> with
        member this.GetEnumerator() = values.GetEnumerator()
        member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

[<Theory>]
[<ClassData(typeof<ValidInputData>)>]
let ``Constuir Valido`` (valor) =
    match List.ofSeq (ValidadorPastaFisicaCliente().Validate(PastaFisicaCliente(valor))).Errors with
    | [] -> Assert.True(true)
    | _ -> raise(InvalidOperationException())


type InvalidInputData()=
    let values: seq<obj[]>=
        seq {          
           yield [|"pasta_".PadRight(51)|]
        }
    interface seq<obj[]> with
        member this.GetEnumerator() = values.GetEnumerator()
        member this.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

[<Theory>]
[<ClassData(typeof<InvalidInputData>)>]
let ``Constuir Inválido`` (valor) =
    match List.ofSeq (ValidadorPastaFisicaCliente().Validate(PastaFisicaCliente(valor))).Errors with
    | [] -> raise(InvalidOperationException())
    | _ -> Assert.True(true)