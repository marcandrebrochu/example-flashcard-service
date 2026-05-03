using FlashcardService.Application.Common.Interfaces;

namespace FlashcardService.Application.Services;

public sealed class IdentifierService : IIdentifierService
{
    public Guid GenerateGuid() => Guid.NewGuid();
}