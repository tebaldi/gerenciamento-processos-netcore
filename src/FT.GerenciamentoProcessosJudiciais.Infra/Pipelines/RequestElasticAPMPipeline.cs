using Elastic.Apm;
using Elastic.Apm.Api;
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
    public class RequestElasticAPMPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        async Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (Agent.Tracer == null || Agent.Tracer.CurrentTransaction == null)
                return await next();

            var stringfiedRequest = JsonConvert.SerializeObject(request, Formatting.Indented);

            return await Agent.Tracer.CurrentTransaction.CaptureSpan($"{request.GetType().Name}: {stringfiedRequest}",
                ApiConstants.TypeRequest, async a =>
                {
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

                        ISpan span = a.StartSpan(
                            $"{response.GetType().Name}: {stringfiedResponse} \nResponse Time: {watch.Elapsed.TotalSeconds} seconds",
                            ApiConstants.TypeRequest, ApiConstants.ActionQuery);

                        span.End();

                        return response;
                    }
                    catch (Exception ex)
                    {
                        a.CaptureException(ex);
                        throw;
                    }

                });
        }
    }
}
