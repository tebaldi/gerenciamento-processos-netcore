namespace FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos

open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open System
open System.Collections.Generic
open System.Linq


type ProcessoPaiAtualizado(aggregateId:string, numeroProcessoUnificado:string)=
    member val AggregateId=aggregateId with get, set
    member val NumeroProcessoUnificado=numeroProcessoUnificado with get, set
    new() = ProcessoPaiAtualizado("", "")

type ResponsavelProcessoAtualizado(aggregateId:string, nome:string, email:string)=
    member val AggregateId=aggregateId with get, set
    member val Nome=nome with get, set
    member val Email=email with get, set
    new() = ResponsavelProcessoAtualizado("", "", "")

type ProcessoAtualizado(aggregateId:string, correlationId:string,
    numeroProcessoUnificado:string, pastaFisicaCliente:string, descricao:string,
    situacao:string, dataDistribuicao:DateTime, segredo:bool, finalizado:bool,
    processoPai:ProcessoPaiAtualizado, responsaveis:ResponsavelProcessoAtualizado[]) =
    inherit EventoPadrao(aggregateId, correlationId)    
    member val NumeroProcessoUnificado=numeroProcessoUnificado with get, set
    member val PastaFisicaCliente=pastaFisicaCliente with get, set
    member val DescricaoProcesso=descricao with get, set
    member val SituacaoProcesso=situacao with get, set
    member val DataDistribuicao=dataDistribuicao with get, set
    member val SegredoDeJustica=segredo with get, set
    member val Finalizado=finalizado with get, set
    member val ProcessoPai=processoPai with get, set
    member val Responsaveis=responsaveis with get, set
    new() = ProcessoAtualizado(
                "", "", "", "", "", SituacoesProcesso.NaoInformada.ToString(), DateTime.MinValue, false,
                false, new ProcessoPaiAtualizado(), Array.empty)

type ProcessoPaiRemovido(aggregateId:string, numeroProcessoUnificado:string)=
    member val AggregateId=aggregateId with get, set
    member val NumeroProcessoUnificado=numeroProcessoUnificado with get, set
    new() = ProcessoPaiRemovido("", "")

type ResponsavelProcessoRemovido(aggregateId:string, nome:string, email:string)=
    member val AggregateId=aggregateId with get, set
    member val Nome=nome with get, set
    member val Email=email with get, set
    new() = ResponsavelProcessoRemovido("", "", "")

type ProcessoRemovido(aggregateId:string, correlationId:string,
    numeroProcessoUnificado:string, pastaFisicaCliente:string, descricao:string,
    situacao:string, dataDistribuicao:DateTime, segredo:bool, finalizado:bool,
    processoPai:ProcessoPaiRemovido, responsaveis:ResponsavelProcessoRemovido[]) =
    inherit EventoPadrao(aggregateId, correlationId)    
    member val NumeroProcessoUnificado=numeroProcessoUnificado with get, set
    member val PastaFisicaCliente=pastaFisicaCliente with get, set
    member val DescricaoProcesso=descricao with get, set
    member val SituacaoProcesso=situacao with get, set
    member val DataDistribuicao=dataDistribuicao with get, set
    member val SegredoDeJustica=segredo with get, set
    member val Finalizado=finalizado with get, set
    member val ProcessoPai=processoPai with get, set
    member val Responsaveis=responsaveis with get, set
    new() = ProcessoRemovido(
                    "", "", "", "", "", SituacoesProcesso.NaoInformada.ToString(), DateTime.MinValue, false,
                    false, new ProcessoPaiRemovido(), Array.empty)