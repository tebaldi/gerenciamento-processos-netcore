using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Interfaces
{
    public interface IRepositorioLeituraResponsaveis
    {
        Task<DadosResponsavel> Load(string aggregateId);

        Task<DadosPaginados<DadosResponsavel>> Query(
            Expression<Func<DadosResponsavel, bool>> predicateResponsavel,
            Expression<Func<DadosProcesso, bool>> predicateProcesso, int pageIndex, int pageSize);
    }
}
