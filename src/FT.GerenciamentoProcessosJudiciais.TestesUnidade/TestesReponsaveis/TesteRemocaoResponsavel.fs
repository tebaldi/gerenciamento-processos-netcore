module TesteRemocaoResponsavel

open System
open Xunit
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos
open FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso
open Moq
open MediatR
open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open System.Threading
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis
open System.Threading.Tasks

[<Fact>]
let ``Deve Remover Responsável Existente`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoResponsaveis>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdResponsavel>()))
        .Returns(Task.FromResult(Responsavel.Construir([]))) |> ignore
    let comando = RemoverResponsavel("resp-testid")
    let interador = RemocaoResponsavel(repositorio.Object) :> IRequestHandler<RemoverResponsavel, Validacao<ResponsavelRemovido>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Valido state -> Assert.NotNull(state)
        | _ -> raise(InvalidOperationException())


[<Fact>]
let ``Não Deve Remover Responsável Com Processo Atrelado`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoResponsaveis>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdResponsavel>()))
        .Returns(Task.FromResult(Responsavel.Construir([]))) |> ignore
    repositorio
        .Setup(fun m -> m.VerificarProcessoAtrelado(It.IsAny<IdResponsavel>()))
        .Returns(Task.FromResult(true)) |> ignore
    let comando = RemoverResponsavel("resp-testid")
    let interador = RemocaoResponsavel(repositorio.Object) :> IRequestHandler<RemoverResponsavel, Validacao<ResponsavelRemovido>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
      | Validacao.Invalido erros -> Assert.Contains(erros, fun e -> e.PropertyName = "ResponsavelProcesso")
      | _ -> raise(InvalidOperationException())
