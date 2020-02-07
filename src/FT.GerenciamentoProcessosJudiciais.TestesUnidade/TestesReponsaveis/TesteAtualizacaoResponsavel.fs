module TesteAtualizacaoResponsavel

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
let ``Deve Atualizar Responsável Valido`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoResponsaveis>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdResponsavel>()))
        .Returns(Task.FromResult(Responsavel.Construir([]))) |> ignore
    let comando = AtualizarResponsavel("resp-testid", "nome", "805.549.230-17", "email@mail.com", "foto")
    let interador = AtualizacaoResponsavel(repositorio.Object) :> IRequestHandler<AtualizarResponsavel, Validacao<ResponsavelAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Valido state ->
            Assert.Equal(comando.AggregateId, state.AggregateId)
            Assert.Equal(comando.Nome, state.Nome)
            Assert.Equal(comando.Cpf, state.Cpf)
            Assert.Equal(comando.Email, state.Email)
            Assert.Equal(comando.Foto, state.Foto)
        | _ -> raise(InvalidOperationException())


[<Fact>]
let ``Não Deve Atualizar Responsável Inválido`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoResponsaveis>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdResponsavel>()))
        .Returns(Task.FromResult(Responsavel.Construir([]))) |> ignore
    let comando = AtualizarResponsavel("", "", "", "", "")
    let interador = AtualizacaoResponsavel(repositorio.Object) :> IRequestHandler<AtualizarResponsavel, Validacao<ResponsavelAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.NotEmpty(erros)
        | _ -> raise(InvalidOperationException())

[<Fact>]
let ``Não Deve Atualizar Responsável Com Cpf Duplicado`` () =
    let repositorio = new Mock<IRepositorioArmazenamentoResponsaveis>()
    repositorio
        .Setup(fun m -> m.CarregarAsync(It.IsAny<IdResponsavel>()))
        .Returns(Task.FromResult(Responsavel.Construir([]))) |> ignore
    repositorio
        .Setup(fun m -> m.VerificarCpfCadastrado(It.IsAny<IdResponsavel>(), It.IsAny<Cpf>()))
        .Returns(Task.FromResult(true)) |> ignore
    let comando = AtualizarResponsavel("resp-testid", "nome", "805.549.230-17", "email@mail.com", "foto")
    let interador = AtualizacaoResponsavel(repositorio.Object) :> IRequestHandler<AtualizarResponsavel, Validacao<ResponsavelAtualizado>>
    let response = interador.Handle(comando, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
    match response with
        | Validacao.Invalido erros -> Assert.Contains(erros, fun e -> e.PropertyName = "Cpf")
        | _ -> raise(InvalidOperationException())