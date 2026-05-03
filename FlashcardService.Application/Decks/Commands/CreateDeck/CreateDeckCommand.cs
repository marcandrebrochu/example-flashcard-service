using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Commands.CreateDeck;

public sealed record CreateDeckCommand : IRequest<Result<CreatedDeck>>
{
    public required string Name { get; init; }
}