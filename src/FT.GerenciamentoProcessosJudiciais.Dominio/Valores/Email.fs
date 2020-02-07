namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type Email(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor

type ValidadorEmail()=
    inherit AbstractValidator<Email>()
    do
        base.RuleFor(fun x -> x.Valor).NotEmpty().WithMessage("Email obrigatório!") |> ignore
        base.RuleFor(fun x -> x.Valor).EmailAddress().WithMessage("Email com formato inválido!") |> ignore

type Email with
    static member ConstruirValido valor = 
        let instance = Email(valor)
        let validacao = ValidadorEmail().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros