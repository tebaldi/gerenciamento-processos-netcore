namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type DescricaoProcesso(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor               

type ValidadorDescricaoProcesso()=
    inherit AbstractValidator<DescricaoProcesso>()
    do
        base.RuleFor(fun x -> x.Valor).MaximumLength(1000).WithMessage("Nome deve possuir no máximo 1000 caracteres") |> ignore

type DescricaoProcesso with
    static member ConstruirValido valor = 
        let instance = DescricaoProcesso(valor)
        let validacao = ValidadorDescricaoProcesso().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros