using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Pipelines
{
    public class RequestLoggingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger logger;

        public RequestLoggingPipeline(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<RequestLoggingPipeline<TRequest, TResponse>>();
        }

        async Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stringfiedRequest = JsonConvert.SerializeObject(request, Formatting.Indented);
            logger.LogDebug($"{request.GetType().Name}: {stringfiedRequest}");

            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var response = await next();

                watch.Stop();

                var stringfiedResponse = response?.ToString() ?? "";
                try
                {
                    stringfiedResponse = JsonConvert.SerializeObject(response, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new JsonPrivateResolver()
                                });
                }
                catch (Exception) { }

                logger.LogDebug($"{response.GetType().Name}: {stringfiedResponse}");
                logger.LogDebug($"Response Time: {watch.Elapsed.TotalSeconds} seconds");

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
