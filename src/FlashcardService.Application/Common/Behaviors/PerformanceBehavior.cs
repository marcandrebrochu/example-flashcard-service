using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FlashcardService.Application.Common.Behaviors;

public sealed partial class PerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _stopwatch = new();
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _stopwatch.Start();
        var response = await next(cancellationToken);
        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds > 500)
        {
            LogWarningRequestTookLongTime(logger, _stopwatch.ElapsedMilliseconds, request);
        }
        else
        {
            LogRequestTime(logger, _stopwatch.ElapsedMilliseconds, request);
        }
        
        return response;
    }

    [LoggerMessage(LogLevel.Warning, "Request took {ElapsedMilliseconds} milliseconds: {@Request}")]
    static partial void LogWarningRequestTookLongTime(
        ILogger<TRequest> logger,
        long elapsedMilliseconds,
        TRequest request);

    [LoggerMessage(LogLevel.Information, "Request took {ElapsedMilliseconds} milliseconds: {@Request}")]
    static partial void LogRequestTime(
        ILogger<TRequest> logger,
        long elapsedMilliseconds,
        TRequest request);
}