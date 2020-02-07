using FT.GerenciamentoProcessosJudiciais.Infra.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Coordinators
{
    public class LeaderElectionCoordinator : ILeaderElectionCoordinator
    {
        public readonly RavenDbContext dbContext;

        public LeaderElectionCoordinator(RavenDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<string> ILeaderElectionCoordinator.ElectLeader(string id, string nodeId)
        {
            using (var session = dbContext.Store.OpenAsyncSession(RavenDbContext.DatabaseName))
            {
                var leader = await session.LoadAsync<LeaderElector>(id);
                if (leader == null)
                {
                    leader = new LeaderElector
                    {
                        LeaderId = nodeId,
                        LeaseLockedTime = DateTime.UtcNow,
                        LeaseLockSecondsTimeout = 30
                    };
                    await session.StoreAsync(leader, id);
                    await session.SaveChangesAsync();
                }
                else
                {
                    var lockExpired = DateTime.UtcNow.Subtract(leader.LeaseLockedTime).TotalSeconds > leader.LeaseLockSecondsTimeout;
                    if (lockExpired)
                    {
                        leader.LeaderId = nodeId;
                        leader.LeaseLockedTime = DateTime.UtcNow;
                        var changeVector = session.Advanced.GetChangeVectorFor(leader);
                        await session.StoreAsync(leader, changeVector, id);
                        await session.SaveChangesAsync();
                    }
                }
                return leader.LeaderId;
            }
        }
    }
}
