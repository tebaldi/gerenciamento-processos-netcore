using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.EventHub
{
    public class InMemoryEventHub : IEventHub
    {
        Task IEventHub.Publish(object message)
        {
            return Task.Delay(TimeSpan.FromMilliseconds(500));
        }
    }
}
