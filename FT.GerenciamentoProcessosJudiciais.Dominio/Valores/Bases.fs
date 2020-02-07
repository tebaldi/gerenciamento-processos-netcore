namespace FT.GerenciamentoProcessosJudiciais.Dominio.Valores

open System
open FluentValidation.Results

type Validacao<'T> =
    | Valido of 'T
    | Invalido of ValidationFailure list

type EventoPadrao(aggregateId:string, correlationId:string) as this =
    let time = DateTime.UtcNow
    member val AggregateId=aggregateId with get, set
    member val AggregateVersion=0L with get, set
    member val CorrelationId=correlationId with get, set
    member val EventType=this.GetType().FullName with get, set
    member val EventName=this.GetType().Name with get, set
    member val EventCreatedTime=time with get, set
    member this.SetVersion version = this.AggregateVersion <- version
    new() = EventoPadrao("", "")

type NotificacaoEmail={ Email:string; Assunto:string; Mensagem:string }