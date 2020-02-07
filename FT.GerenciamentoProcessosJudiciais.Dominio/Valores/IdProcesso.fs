namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type IdProcesso(valor:String) =        
    member val Valor =
        match valor with
        | var1 when String.IsNullOrEmpty(var1) -> ""        
        | _ -> valor

type ValidadorIdProcesso()=
    inherit AbstractValidator<IdProcesso>()
    do
        base.RuleFor(fun x -> x.Valor).NotEmpty().WithMessage("Id obrigatório!") |> ignore
        base.RuleFor(fun x -> x.Valor).Must(fun v -> not (Guid.Empty.ToString().Equals(v))).WithMessage("Guid inválido") |> ignore

type IdProcesso with
    static member ConstruirValido valor = 
        let instance = IdProcesso(valor)
        let validacao = ValidadorIdProcesso().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros