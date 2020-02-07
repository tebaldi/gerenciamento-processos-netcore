namespace FT.GerenciamentoProcessosJudiciais.Dominio.Processos

open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos
open FluentValidation.Results
open System
open FluentValidation

type ProcessoPai={ aggregateId: IdProcesso; numero:NumeroProcessoUnificado }

type ResponsavelProcesso={ aggregateId: IdResponsavel; nome: NomeResponsavel; email: Email }

type EstadoProcesso={ numero:NumeroProcessoUnificado; pasta:PastaFisicaCliente;
    descricao:DescricaoProcesso; dataDistribuicao:DataDistribuicao; situacao:SituacaoProcesso;
    segredo:bool; processoPai:ProcessoPai; responsaveis:ResponsavelProcesso list }

type ProcessoAtivo = { aggregateId: IdProcesso; estado:EstadoProcesso }
type ProcessoFinalizado = { aggregateId: IdProcesso; estado:EstadoProcesso }
type ProcessoInativo = { aggregateId: IdProcesso; }

type Processo = 
    | Ativo of ProcessoAtivo
    | Finalizado of ProcessoFinalizado
    | Inativo of ProcessoInativo

type ValidadorProcessoAtivo()=
    inherit AbstractValidator<ProcessoAtivo>()
    do
        base.RuleFor(fun x -> x.aggregateId).SetValidator((new ValidadorIdProcesso())) |> ignore
        base.RuleFor(fun x -> x.estado.numero).SetValidator((new ValidadorNumeroProcessoUnificado())) |> ignore

type ProcessoAtivo with
    member this.Atualizar (comando:AtualizarProcesso)=
        ProcessoAtualizado(
            IdProcesso(comando.AggregateId).Valor, Guid.NewGuid().ToString(),            
            NumeroProcessoUnificado(comando.NumeroProcessoUnificado).Valor, PastaFisicaCliente(comando.PastaFisicaCliente).Valor,
            DescricaoProcesso(comando.DescricaoProcesso).Valor, SituacaoProcesso(Enum.Parse<SituacoesProcesso>(comando.SituacaoProcesso)).Valor.ToString(),
            DataDistribuicao(comando.DataDistribuicao).Valor, comando.SegredoDeJustica,
            SituacaoProcesso(Enum.Parse<SituacoesProcesso>(comando.SituacaoProcesso)).Finalizado,
            ProcessoPaiAtualizado(comando.ProcessoPai.AggregateId, comando.ProcessoPai.NumeroProcessoUnificado),
            Array.ofSeq comando.Responsaveis |> 
            Array.map (fun x -> ResponsavelProcessoAtualizado(x.AggregateId, x.Nome, x.Email))
            )

    member this.Remover (comando:RemoverProcesso)=
        ProcessoRemovido(
            IdProcesso(comando.AggregateId).Valor, Guid.NewGuid().ToString(),            
            this.estado.numero.Valor, this.estado.pasta.Valor, this.estado.descricao.Valor,
            this.estado.situacao.Valor.ToString(), this.estado.dataDistribuicao.Valor, this.estado.segredo, 
            this.estado.situacao.Finalizado, ProcessoPaiRemovido(
                this.estado.processoPai.aggregateId.Valor, this.estado.processoPai.numero.Valor),
            Array.ofList this.estado.responsaveis |> Array.map (fun x -> ResponsavelProcessoRemovido(x.aggregateId.Valor, x.nome.Valor, x.email.Valor))
            )

type Processo with
    static member private Apply (state:Processo) (event:EventoPadrao) = 
        match state with
            | Inativo _ -> state
            | _ ->
                match event with
                | :? ProcessoAtualizado as e ->
                    let estado = {
                         numero=NumeroProcessoUnificado(e.NumeroProcessoUnificado);
                        descricao=DescricaoProcesso(e.DescricaoProcesso); pasta=PastaFisicaCliente(e.PastaFisicaCliente);
                        dataDistribuicao=DataDistribuicao(e.DataDistribuicao); segredo=e.SegredoDeJustica;
                        situacao=SituacaoProcesso(Enum.Parse<SituacoesProcesso>(e.SituacaoProcesso));
                        processoPai={ aggregateId=IdProcesso(e.ProcessoPai.AggregateId);
                            numero=NumeroProcessoUnificado(e.ProcessoPai.NumeroProcessoUnificado)};
                        responsaveis=
                            List.ofSeq e.Responsaveis |> List.map (fun x -> {
                                aggregateId=IdResponsavel(x.AggregateId);
                                nome=NomeResponsavel(x.Nome); email=Email(x.Email)})
                        }
                    if e.Finalizado
                    then Processo.Finalizado {aggregateId=IdProcesso(e.AggregateId); estado=estado}
                    else Processo.Ativo {aggregateId=IdProcesso(e.AggregateId); estado=estado}
                | _ -> Processo.Inativo { aggregateId=IdProcesso(event.AggregateId) }

    static member Construir((historico:EventoPadrao list)) =
        let state = Processo.Ativo { aggregateId=IdProcesso(""); estado={
            numero=NumeroProcessoUnificado("");
            descricao=DescricaoProcesso(""); pasta=PastaFisicaCliente("");
            dataDistribuicao=DataDistribuicao(DateTime.MinValue); segredo=false;
            situacao=SituacaoProcesso(SituacoesProcesso.NaoInformada);
            processoPai={ aggregateId=IdProcesso(""); numero=NumeroProcessoUnificado("") };
            responsaveis=[]
            }}
        match historico with
            | [] -> state
            | _ -> historico |> List.fold (fun acc e -> Processo.Apply acc e) state
    
    member private this.Validar =
        match this with
            | Ativo state -> List.ofSeq (ValidadorProcessoAtivo().Validate(state)).Errors
            | _ -> []

    member this.Estado=
        match this with
        | Ativo state -> state.estado
        | Finalizado state -> state.estado
        | _ -> { numero=NumeroProcessoUnificado(""); descricao=DescricaoProcesso(""); pasta=PastaFisicaCliente("");
                dataDistribuicao=DataDistribuicao(DateTime.MinValue); segredo=false;
                situacao=SituacaoProcesso(SituacoesProcesso.NaoInformada);
                processoPai={ aggregateId=IdProcesso(""); numero=NumeroProcessoUnificado("") };
                responsaveis=[] }

    member this.Atualizar comando = 
        match this with
        | Ativo state -> 
            let evento = state.Atualizar comando
            let update = Processo.Apply this evento
            match update.Validar with
                | [] -> Validacao.Valido evento
                | erros -> Validacao.Invalido erros
        | _ -> Validacao.Invalido [new ValidationFailure("Processo", "Estado inválido")]

    member this.Remover comando = 
        match this with
        | Ativo state -> 
            let evento = state.Remover comando
            let update = Processo.Apply this evento
            match update.Validar with
                | [] -> Validacao.Valido evento
                | erros -> Validacao.Invalido erros
        | _ -> Validacao.Invalido [new ValidationFailure("Processo", "Estado inválido")]