namespace FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos

open MediatR
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores

type AtualizarResponsavel(aggregateId:string, nome:string, cpf:string, email:string, foto:string)=
    interface IRequest<Validacao<ResponsavelAtualizado>>
    member val AggregateId=aggregateId with get, set
    member val Nome=nome with get, set
    member val Cpf=cpf with get, set
    member val Email=email with get, set
    member val Foto=foto with get, set
    new() = AtualizarResponsavel("", "", "", "", "")

type RemoverResponsavel(aggregateId:string)=
    interface IRequest<Validacao<ResponsavelRemovido>>
    member val AggregateId=aggregateId with get, set
    new() = RemoverResponsavel("")