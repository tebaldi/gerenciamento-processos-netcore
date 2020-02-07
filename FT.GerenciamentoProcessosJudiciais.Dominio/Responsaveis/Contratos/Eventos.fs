namespace FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos

open FT.GerenciamentoProcessosJudiciais.Dominio.Valores


type ResponsavelAtualizado(aggregateId:string, correlationId:string, nome:string, cpf:string, email:string, foto:string) =
    inherit EventoPadrao(aggregateId, correlationId)    
    member val Nome=nome with get, set
    member val Cpf=cpf with get, set
    member val Email=email with get, set
    member val Foto=foto with get, set
    new() = ResponsavelAtualizado("", "", "", "", "", "")

type ResponsavelRemovido(aggregateId:string, correlationId:string, nome:string, cpf:string, email:string, foto:string) =
    inherit EventoPadrao(aggregateId, correlationId)    
    member val Nome=nome with get, set
    member val Cpf=cpf with get, set
    member val Email=email with get, set
    member val Foto=foto with get, set
    new() = ResponsavelRemovido("", "", "", "", "", "")