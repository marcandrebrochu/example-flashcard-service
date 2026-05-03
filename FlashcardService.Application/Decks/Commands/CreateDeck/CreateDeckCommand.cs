using FlashcardService.Application.Common.Dtos;
using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Commands.CreateDeck;

public sealed record CreateDeckCommand : IRequest<Result<DeckDto>>
{
    public required string Name { get; init; }
}