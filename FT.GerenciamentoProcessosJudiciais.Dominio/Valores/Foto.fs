namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation
open System.Text.RegularExpressions

type Foto(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor               

type ValidadorFoto()=
    inherit AbstractValidator<Foto>()

type Foto with
    static member ConstruirValido valor = 
        let instance = Foto(valor)
        let validacao = ValidadorFoto().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros