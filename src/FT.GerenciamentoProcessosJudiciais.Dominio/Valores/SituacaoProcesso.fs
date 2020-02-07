namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation

type SituacoesProcesso = NaoInformada=0 | EmAndamento=1 | Desmembrado=2 | EmRecurso=3 | Finalizado=4 | Arquivado=5

type SituacaoProcesso(valor:SituacoesProcesso)=        
    member val Valor = valor
    member val Finalizado =
        match valor with
            | var1 when var1 = SituacoesProcesso.Arquivado || var1 = SituacoesProcesso.Finalizado -> true
            | _ -> false

type ValidadorSituacaoProcesso()=
    inherit AbstractValidator<SituacaoProcesso>()
    do
        base.RuleFor(fun x -> x.Valor).NotEqual(SituacoesProcesso.NaoInformada).WithMessage("Situação processo obrigatoria!") |> ignore

type SituacaoProcesso with
    static member ConstruirValido valor = 
        let instance = SituacaoProcesso(valor)
        let validacao = ValidadorSituacaoProcesso().Validate(instance)
        let erros = List.ofSeq validacao.Errors
        match erros with
            | [] -> Validacao.Valido instance
            | _ -> Validacao.Invalido erros