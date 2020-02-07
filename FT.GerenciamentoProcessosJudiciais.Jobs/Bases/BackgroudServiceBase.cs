using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Jobs.Bases
{
    abstract class BackgroudServiceBase : BackgroundService
    {
        protected readonly ILogger Logger;

        public BackgroudServiceBase(ILogger logger) { this.Logger = logger; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var watch = new Stopwatch();
                watch.Start();

                Logger.LogInformation($"Start ExecuteAsync on {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

                await ExecuteJobAsync(stoppingToken);

                watch.Stop();

                Logger.LogInformation($"Finish ExecuteAsync in {watch.Elapsed.TotalSeconds} seconds");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        protected abstract Task ExecuteJobAsync(CancellationToken stoppingToken);
    }
}
