namespace FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos

open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open System
open System.Collections.Generic
open MediatR
open System.Linq

type ProcessoPaiAtualizacao(aggregateId:string, numeroProcessoUnificado:string)=
    member val AggregateId=aggregateId with get, set
    member val NumeroProcessoUnificado=numeroProcessoUnificado with get, set
    new() = ProcessoPaiAtualizacao("", "")

type ResponsavelProcessoAtualizacao(aggregateId:string, nome:string, email:string)=
    member val AggregateId=aggregateId with get, set
    member val Nome=nome with get, set
    member val Email=email with get, set
    new() = ResponsavelProcessoAtualizacao("", "", "")

type AtualizarProcesso(aggregateId:string,
    numeroProcessoUnificado:string, pastaFisicaCliente:string, descricao:string,
    situacao:string, dataDistribuicao:DateTime, segredo:bool, finalizado:bool,
    processoPai:ProcessoPaiAtualizacao, responsaveis:IEnumerable<ResponsavelProcessoAtualizacao>) =
    interface IRequest<Validacao<ProcessoAtualizado>>
    member val AggregateId=aggregateId with get, set
    member val NumeroProcessoUnificado=numeroProcessoUnificado with get, set
    member val PastaFisicaCliente=pastaFisicaCliente with get, set
    member val DescricaoProcesso=descricao with get, set
    member val SituacaoProcesso=situacao with get, set
    member val DataDistribuicao=dataDistribuicao with get, set
    member val SegredoDeJustica=segredo with get, set
    member val Finalizado=finalizado with get, set
    member val ProcessoPai=processoPai with get, set
    member val Responsaveis=responsaveis with get, set
    new() = AtualizarProcesso(
                "", "", "", "", SituacoesProcesso.NaoInformada.ToString(), DateTime.MinValue, false,
                false, new ProcessoPaiAtualizacao(), Enumerable.Empty<ResponsavelProcessoAtualizacao>())

type RemoverProcesso(aggregateId:string) =
    interface IRequest<Validacao<ProcessoRemovido>>
    member val AggregateId=aggregateId with get, set
    new() = RemoverProcesso("")