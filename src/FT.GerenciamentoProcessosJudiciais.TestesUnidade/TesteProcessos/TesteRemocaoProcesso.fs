module TesteRemocaoProcesso

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
let ``Deve Remover Processo Existente`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    let comando = RemoverProcesso("proc-testid")
    let interador = RemocaoProcesso(repositorio.Object) :> IRequestHandler<RemoverProcesso, Validacao<ProcessoRemovido>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Valido state -> Assert.NotNull(state)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Não Deve Remover Processo Com Filhos`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoProcessos>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(Processo.Construir([]))) |> ignore
    repositorio
        .Setup(fun m -> m.CarregarHierarquiaProcessoPorNivel(It.IsAny<IdProcesso>()))
        .Returns(Task.FromResult(new ResizeArray<IdProcesso>([IdProcesso("proc-testid"); IdProcesso("proc-f")]))) |> ignore
    let comando = RemoverProcesso("proc-testid")
    let interador = RemocaoProcesso(repositorio.Object) :> IRequestHandler<RemoverProcesso, Validacao<ProcessoRemovido>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.Contains(erros, fun e -> e.PropertyName = "Hierarquia")
        | _ -> raise(InvalidOperationException())