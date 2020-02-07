using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.EventHub
{
    public interface IEventHub
    {
        Task Publish(object message);
    }
}
