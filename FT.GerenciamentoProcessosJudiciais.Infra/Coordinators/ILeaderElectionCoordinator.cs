using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Coordinators
{
    public interface ILeaderElectionCoordinator
    {
        Task<string> ElectLeader(string id, string nodeId);
    }
}
