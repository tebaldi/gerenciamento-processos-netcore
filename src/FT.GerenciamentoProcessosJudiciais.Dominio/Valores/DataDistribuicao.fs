namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type DataDistribuicao(valor:DateTime) =        
    member val Valor = valor.ToUniversalTime()

type ValidadorDataDistribuicao()=
    inherit AbstractValidator<DataDistribuicao>()
    do
        base.RuleFor(fun x -> x.Valor.Date.AddSeconds(1.0)).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Data deve ser menor ou igual a data atual") |> ignore

type DataDistribuicao with
    static member ConstruirValido valor = 
        let instance = DataDistribuicao(valor)
        let validacao = ValidadorDataDistribuicao().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros