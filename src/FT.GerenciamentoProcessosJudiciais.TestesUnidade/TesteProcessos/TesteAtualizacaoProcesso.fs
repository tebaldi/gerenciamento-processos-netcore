module TesteAtualizacaoProcesso

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open Moq
open FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso
open System.Threading.Tasks
open MediatR
open System.Threading

[<Fact>]
let ``Deve Atualizar Processo Valido`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    let comando = AtualizarProcesso(
                    "test-id", "8055492-30.1722.123.4567", "pasta", "descricao", "EmAndamento",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiAtualizacao("test-pid", "7055492-30.1722.123.4567"),
                    [ResponsavelProcessoAtualizacao("r-id", "nome","emial@email.com")])
    let interador = AtualizacaoProcesso(repositorio.Object) :> IRequestHandler<AtualizarProcesso, Validacao<ProcessoAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Valido state ->
            Assert.Equal(comando.AggregateId, state.AggregateId)
            Assert.Equal(comando.NumeroProcessoUnificado, state.NumeroProcessoUnificado)
            Assert.Equal(comando.DescricaoProcesso, state.DescricaoProcesso)
            Assert.Equal(comando.PastaFisicaCliente, state.PastaFisicaCliente)
            Assert.Equal(comando.ProcessoPai.AggregateId, state.ProcessoPai.AggregateId)
            Assert.Equal(comando.ProcessoPai.NumeroProcessoUnificado, state.ProcessoPai.NumeroProcessoUnificado)
            Assert.Equal(comando.DataDistribuicao.ToUniversalTime(), state.DataDistribuicao)
            Assert.Equal(comando.SituacaoProcesso, state.SituacaoProcesso)
            Assert.True(state.SegredoDeJustica)
            Assert.NotEmpty(state.Responsaveis)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Não Deve Atualizar Processo Inválido`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    let comando = AtualizarProcesso(
                    "", "", "", "", "NaoInformada",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiAtualizacao("test-pid", "7055492-30.1722.123.4567"),
                    [ResponsavelProcessoAtualizacao("r-id", "nome","emial@email.com")])
    let interador = AtualizacaoProcesso(repositorio.Object) :> IRequestHandler<AtualizarProcesso, Validacao<ProcessoAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.NotEmpty(erros)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Não Deve Atualizar Processo Com Número Duplicado`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    repositorio
        .Setup(fun m -> m.VerificarNumeroCadastrado(It.IsAny<IdProcesso>(), It.IsAny<NumeroProcessoUnificado>()))
        .Returns(Task.FromResult(true)) |> ignore
    let comando = AtualizarProcesso(
                    "test-id", "8055492-30.1722.123.4567", "pasta", "descricao", "EmAndamento",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiAtualizacao("test-pid", "7055492-30.1722.123.4567"),
                    [ResponsavelProcessoAtualizacao("r-id", "nome","emial@email.com")])
    let interador = AtualizacaoProcesso(repositorio.Object) :> IRequestHandler<AtualizarProcesso, Validacao<ProcessoAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.Contains(erros, fun e -> e.PropertyName = "NumeroProcessoUnificado")
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Não Deve Atualizar Processo Com Processo Pai Atrelado`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    repositorio
        .Setup(fun m -> m.VerificarProcessoPaiAtrelado(It.IsAny<IdProcesso>(), It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(true)) |> ignore
    let comando = AtualizarProcesso(
                    "test-id", "8055492-30.1722.123.4567", "pasta", "descricao", "EmAndamento",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiAtualizacao("test-pid", "7055492-30.1722.123.4567"),
                    [ResponsavelProcessoAtualizacao("r-id", "nome","emial@email.com")])
    let interador = AtualizacaoProcesso(repositorio.Object) :> IRequestHandler<AtualizarProcesso, Validacao<ProcessoAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.Contains(erros, fun e -> e.PropertyName = "ProcessoPai")
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Não Deve Atualizar Processo Com Limite Hierarquia Atingido`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    repositorio
        .Setup(fun m -> m.CarregarHierarquiaProcessoPorNivel(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(new ResizeArray<IdProcesso>([
                                    IdProcesso("proc-testid"); IdProcesso("proc-f");
                                    IdProcesso("proc-f2"); IdProcesso("proc-f3")]
                                    ))) |> ignore
    let comando = AtualizarProcesso(
                    "test-id", "8055492-30.1722.123.4567", "pasta", "descricao", "EmAndamento",
                    DateTime.Today.AddDays(-1.0), true, false, ProcessoPaiAtualizacao("test-pid", "7055492-30.1722.123.4567"),
                    [ResponsavelProcessoAtualizacao("r-id", "nome","emial@email.com")])
    let interador = AtualizacaoProcesso(repositorio.Object) :> IRequestHandler<AtualizarProcesso, Validacao<ProcessoAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.Contains(erros, fun e -> e.PropertyName = "Hierarquia")
        | _ -> raise(InvalidOperationException())