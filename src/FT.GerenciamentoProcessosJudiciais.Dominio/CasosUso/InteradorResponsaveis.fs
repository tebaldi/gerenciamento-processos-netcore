namespace FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso

open System
open MediatR
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos
open FluentValidation.Results

type AtualizacaoResponsavel(repositorio:IRepositorioArmazenamentoResponsaveis)=
    interface IRequestHandler<AtualizarResponsavel, Validacao<ResponsavelAtualizado>> with        
        member this.Handle(request, cancellationToken) =  Async.StartAsTask(async {
            if (String.IsNullOrEmpty(request.AggregateId)) then request.AggregateId <- Guid.NewGuid().ToString()
            let id = IdResponsavel(request.AggregateId)
            let! cadastrado = repositorio.VerificarCpfCadastrado(id, Cpf(request.Cpf)) |> Async.AwaitTask            
            if cadastrado then
                return Validacao.Invalido [new ValidationFailure("Cpf", "Cpf ja cadastrado !")]
            else
                let! responsavel = repositorio.CarregarAsync(id) |> Async.AwaitTask
                match responsavel.Atualizar request with
                    | Valido evento -> 
                        do! repositorio.ArmazenarAsync([evento]) |> Async.AwaitTask
                        return Validacao.Valido evento
                    | Invalido erros -> return Validacao.Invalido erros
        })

type RemocaoResponsavel(repositorio:IRepositorioArmazenamentoResponsaveis)=
    interface IRequestHandler<RemoverResponsavel, Validacao<ResponsavelRemovido>> with        
        member this.Handle(request, cancellationToken) =  Async.StartAsTask(async {
            if (String.IsNullOrEmpty(request.AggregateId)) then request.AggregateId <- Guid.NewGuid().ToString()
            let id = IdResponsavel(request.AggregateId)
            let! possuiProcesso = repositorio.VerificarProcessoAtrelado(id) |> Async.AwaitTask            
            if possuiProcesso then
                return Validacao.Invalido [new ValidationFailure("ResponsavelProcesso", "Responsável não pode ser removido pois possui processo atrelado!")]
            else
                let! responsavel = repositorio.CarregarAsync(id) |> Async.AwaitTask
                match responsavel.Remover request with
                    | Valido evento -> 
                        do! repositorio.ArmazenarAsync([evento]) |> Async.AwaitTask
                        return Validacao.Valido evento
                    | Invalido erros -> return Validacao.Invalido erros
        })