namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type PastaFisicaCliente(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor               

type ValidadorPastaFisicaCliente()=
    inherit AbstractValidator<PastaFisicaCliente>()
    do
        base.RuleFor(fun x -> x.Valor).MaximumLength(50).WithMessage("Pasta fisica deve possuir no máximo 50 caracteres") |> ignore

type PastaFisicaCliente with
    static member ConstruirValido valor = 
        let instance = PastaFisicaCliente(valor)
        let validacao = ValidadorPastaFisicaCliente().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros