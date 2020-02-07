namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type IdResponsavel(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor

type ValidadorIdResponsavel()=
    inherit AbstractValidator<IdResponsavel>()
    do
        base.RuleFor(fun x -> x.Valor).NotEmpty().WithMessage("Id obrigatório!") |> ignore
        base.RuleFor(fun x -> x.Valor).Must(fun v -> not (Guid.Empty.ToString().Equals(v))).WithMessage("Guid inválido") |> ignore

type IdResponsavel with
    static member ConstruirValido valor = 
        let instance = IdResponsavel(valor)
        let validacao = ValidadorIdResponsavel().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros