using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Mailing
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }
}
