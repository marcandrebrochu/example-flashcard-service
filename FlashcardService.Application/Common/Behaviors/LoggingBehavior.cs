using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace FlashcardService.Application.Common.Behaviors;

public sealed partial class LoggingBehavior<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        LogNameAndRequest(logger, request);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Got request: {@Request}")]
    static partial void LogNameAndRequest(ILogger<TRequest> logger, TRequest request);
}