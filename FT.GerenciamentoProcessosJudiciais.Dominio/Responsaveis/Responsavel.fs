namespace FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis

open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos
open FluentValidation.Results
open System
open FluentValidation

type ResponsavelAtivo = { aggregateId: IdResponsavel; cpf:Cpf; nome:NomeResponsavel; email:Email; foto:Foto }
type ResponsavelInativo = { aggregateId: IdResponsavel; }

type Responsavel = 
    | Ativo of ResponsavelAtivo
    | Inativo of ResponsavelInativo

type ValidadorResponsavelAtivo()=
    inherit AbstractValidator<ResponsavelAtivo>()
    do
        base.RuleFor(fun x -> x.aggregateId).SetValidator((new ValidadorIdResponsavel())) |> ignore
        base.RuleFor(fun x -> x.cpf).SetValidator((new ValidadorCpf())) |> ignore

type ResponsavelAtivo with
    member this.Atualizar (comando:AtualizarResponsavel)=
        ResponsavelAtualizado(
            IdResponsavel(comando.AggregateId).Valor, Guid.NewGuid().ToString(),            
            NomeResponsavel(comando.Nome).Valor, Cpf(comando.Cpf).Valor,
            Email(comando.Email).Valor, Foto(comando.Foto).Valor)

    member this.Remover (comando:RemoverResponsavel)=
        ResponsavelRemovido(
            IdResponsavel(comando.AggregateId).Valor, Guid.NewGuid().ToString(), 
            this.nome.Valor, this.cpf.Valor, this.email.Valor, this.foto.Valor)

type Responsavel with
    static member private Apply (state:Responsavel) (event:EventoPadrao) = 
        match state with
            | Inativo _ -> state
            | _ ->
                match event with
                | :? ResponsavelAtualizado as e -> Responsavel.Ativo {
                    aggregateId=IdResponsavel(e.AggregateId); cpf=Cpf(e.Cpf);
                    nome=NomeResponsavel(e.Nome); email=Email(e.Email); foto=Foto(e.Foto) }
                | _ -> Responsavel.Inativo { aggregateId=IdResponsavel(event.AggregateId) }

    static member Construir((historico:EventoPadrao list)) =
        let state = Responsavel.Ativo { 
            aggregateId=IdResponsavel(""); nome=NomeResponsavel(""); 
            email=Email(""); cpf=Cpf(""); foto=Foto("") }
        match historico with
            | [] -> state
            | _ -> historico |> List.fold (fun acc e -> Responsavel.Apply acc e) state
    
    member private this.Validar =
        match this with
            | Ativo state -> List.ofSeq (ValidadorResponsavelAtivo().Validate(state)).Errors
            | _ -> []

    member this.Atualizar comando = 
        match this with
        | Ativo state -> 
            let evento = state.Atualizar comando
            let update = Responsavel.Apply this evento
            match update.Validar with
                | [] -> Validacao.Valido evento
                | erros -> Validacao.Invalido erros
        | _ -> Validacao.Invalido [new ValidationFailure("Responsavel", "Estado inválido")]

    member this.Remover comando = 
        match this with
        | Ativo state -> 
            let evento = state.Remover comando
            let update = Responsavel.Apply this evento
            match update.Validar with
                | [] -> Validacao.Valido evento
                | erros -> Validacao.Invalido erros
        | _ -> Validacao.Invalido [new ValidationFailure("Responsavel", "Estado inválido")]