namespace FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso

open System
open MediatR
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos
open FluentValidation.Results

type AtualizacaoProcesso(repositorio:IRepositorioArmazenamentoProcessos)=
    interface IRequestHandler<AtualizarProcesso, Validacao<ProcessoAtualizado>> with        
        member this.Handle(request, cancellationToken) =  Async.StartAsTask(async {
            if (String.IsNullOrEmpty(request.AggregateId)) then request.AggregateId <- Guid.NewGuid().ToString()
            let id = IdProcesso(request.AggregateId)
            let numero = NumeroProcessoUnificado(request.NumeroProcessoUnificado)
            let mutable erros = []
            let! cadastrado = repositorio.VerificarNumeroCadastrado(id, numero) |> Async.AwaitTask            
            let! processo = repositorio.CarregarAsync(id) |> Async.AwaitTask
            if cadastrado then
                erros <- erros @ [new ValidationFailure("NumeroProcessoUnificado", "Numero Processo ja cadastrado !")]
            if not (String.IsNullOrEmpty(request.ProcessoPai.AggregateId)) then
                let! processoPaiAtrelado = repositorio.VerificarProcessoPaiAtrelado(id, IdProcesso(request.ProcessoPai.AggregateId)) |> Async.AwaitTask
                if processoPaiAtrelado then
                    erros <- erros @ [new ValidationFailure("ProcessoPai", "Processo Pai ja está atrelado a outro processo!!")]            
                else
                    let! hierarquia = repositorio.CarregarHierarquiaProcessoPorNivel(id) |> Async.AwaitTask
                    match hierarquia with
                        | var1 when var1 <> null && var1.Count > 0 ->
                            let processoPaiNaoInformado = String.IsNullOrEmpty(processo.Estado.processoPai.aggregateId.Valor)
                            if hierarquia.Count >= 4 && processoPaiNaoInformado then
                                erros <- erros @ [new ValidationFailure("Hierarquia", "Limite para hierarquia ja foi atingido!")]
                        | _ -> ()
            if erros.Length > 0 then
                return Invalido erros
            else
                let notificacoes = List.ofSeq request.Responsaveis |> List.map(fun x -> {
                                   Assunto=sprintf "Você foi cadastrado como envolvido no processo de número %s" numero.Valor;
                                   Mensagem="Mais informaçãoes no link:"; Email=x.Email
                               })
                match processo.Atualizar request with
                    | Valido evento -> 
                        do! repositorio.ArmazenarAsync([evento], notificacoes) |> Async.AwaitTask
                        return Validacao.Valido evento
                    | Invalido erros -> return Validacao.Invalido erros
        })

type RemocaoProcesso(repositorio:IRepositorioArmazenamentoProcessos)=
    interface IRequestHandler<RemoverProcesso, Validacao<ProcessoRemovido>> with        
        member this.Handle(request, cancellationToken) =  Async.StartAsTask(async {
            if (String.IsNullOrEmpty(request.AggregateId)) then request.AggregateId <- Guid.NewGuid().ToString()
            let id = IdProcesso(request.AggregateId)
            let mutable erros = []
            let! hierarquia = repositorio.CarregarHierarquiaProcessoPorNivel(id) |> Async.AwaitTask
            match hierarquia with
                | var1 when var1 <> null && var1.Count > 0 ->
                    let nivel =  List.ofSeq hierarquia |> List.findIndex(fun x -> x.Valor = id.Valor)
                    let contemFilhos = nivel + 1 < hierarquia.Count
                    if contemFilhos then
                        erros <- erros @ [new ValidationFailure("Hierarquia", "Processo não pode ser removido pois contem processos filhos!")]
                | _ ->()
            if erros.Length > 0 then
                return Invalido erros
            else
                let! processo = repositorio.CarregarAsync(id) |> Async.AwaitTask
                let numeroProcesso = processo.Estado.numero.Valor
                let notificacoes = processo.Estado.responsaveis |> List.map(fun x -> {
                    Assunto=sprintf "O processo de número %s foi removido." numeroProcesso;
                    Mensagem="Mais informaçãoes no link:"; Email=x.email.Valor
                })
                match processo.Remover request with
                    | Valido evento -> 
                        do! repositorio.ArmazenarAsync([evento], notificacoes) |> Async.AwaitTask
                        return Validacao.Valido evento
                    | Invalido erros -> return Validacao.Invalido erros
        })