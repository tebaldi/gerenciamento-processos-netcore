namespace FT.GerenciamentoProcessosJudiciais.Dominio.CasosUso

open FT.GerenciamentoProcessosJudiciais.Dominio.Valores
open FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis
open System.Threading.Tasks
open FT.GerenciamentoProcessosJudiciais.Dominio.Processos
open System.Collections.Generic

type IRepositorioArmazenamentoResponsaveis =
    abstract CarregarAsync: IdResponsavel -> Task<Responsavel>
    abstract ArmazenarAsync: IEnumerable<EventoPadrao> -> Task
    abstract VerificarCpfCadastrado: IdResponsavel * Cpf -> Task<bool>
    abstract VerificarProcessoAtrelado: IdResponsavel -> Task<bool>

type IRepositorioArmazenamentoProcessos =
    abstract CarregarAsync: IdProcesso -> Task<Processo>
    abstract ArmazenarAsync: IEnumerable<EventoPadrao> * IEnumerable<NotificacaoEmail> -> Task
    abstract VerificarNumeroCadastrado: IdProcesso * NumeroProcessoUnificado -> Task<bool>
    abstract VerificarProcessoPaiAtrelado: IdProcesso * IdProcesso -> Task<bool>
    abstract CarregarHierarquiaProcessoPorNivel: IdProcesso -> Task<List<IdProcesso>>