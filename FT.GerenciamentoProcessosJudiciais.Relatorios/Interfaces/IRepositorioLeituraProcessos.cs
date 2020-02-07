using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Relatorios.Interfaces
{
    public interface IRepositorioLeituraProcessos
    {
        Task<DadosProcesso> Load(string aggregateId);

        Task<IEnumerable<DadosHierarquiaProcesso>> LoadHierarquia(string aggregateId);

        Task<DadosPaginados<DadosProcesso>> Query(
            Expression<Func<DadosProcesso, bool>> predicateProcesso, int pageIndex, int pageSize);
    }
}
