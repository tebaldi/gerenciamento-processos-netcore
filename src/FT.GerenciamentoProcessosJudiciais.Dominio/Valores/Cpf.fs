namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation
open System.Text.RegularExpressions

type Cpf(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> 
            let f1 = valor.Replace(".", "").Replace("-", "").PadLeft(11, '0')
            Regex.Replace(f1, @"^(\d{3})(\d{3})(\d{3})(\d{2})$", "$1.$2.$3-$4")                

type ValidadorCpf()=
    inherit AbstractValidator<Cpf>()
    do 
        let formatoValido = fun (v:String) -> true
        base.RuleFor(fun x -> x.Valor).NotEmpty().WithMessage("Cpf obrigatório!") |> ignore
        base.RuleFor(fun x -> x.Valor).Must(formatoValido).WithMessage("Cpf com formato inválido!") |> ignore      

type Cpf with
    static member ConstruirValido valor = 
        let instance = Cpf(valor)
        let validacao = ValidadorCpf().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros