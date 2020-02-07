namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation
open System.Text.RegularExpressions

type NomeResponsavel(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor               

type ValidadorNomeResponsavel()=
    inherit AbstractValidator<NomeResponsavel>()
    do
        base.RuleFor(fun x -> x.Valor).NotEmpty().WithMessage("Nome obrigatório!") |> ignore
        base.RuleFor(fun x -> x.Valor).MaximumLength(150).WithMessage("Nome deve possuir no máximo 150 caracteres") |> ignore

type NomeResponsavel with
    static member ConstruirValido valor = 
        let instance = NomeResponsavel(valor)
        let validacao = ValidadorNomeResponsavel().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros