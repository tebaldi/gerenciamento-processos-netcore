namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation
open System.Text.RegularExpressions

type NumeroProcessoUnificado(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> 
            let f1 = valor.Replace(".", "").Replace("-", "").PadLeft(20, '0')
            Regex.Replace(f1, @"^(\d{7})(\d{2})(\d{4})(\d{3})(\d{4})$", "$1-$2.$3.$4.$5")

type ValidadorNumeroProcessoUnificado()=
    inherit AbstractValidator<NumeroProcessoUnificado>()
    do 
        base.RuleFor(fun x -> x.Valor).NotEmpty().WithMessage("Numero Processo obrigatório!") |> ignore
        base.RuleFor(fun x -> x.Valor).MaximumLength(200).WithMessage("Número do processo deve possuir no máximo 200 caracteres") |> ignore

type NumeroProcessoUnificado with
    static member ConstruirValido valor = 
        let instance = NumeroProcessoUnificado(valor)
        let validacao = ValidadorNumeroProcessoUnificado().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros