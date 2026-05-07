using FlashcardService.Application.Common.Dtos;
using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Queries.GetDeckById;

public sealed record GetDeckByIdQuery(Guid Id) : IRequest<Result<DeckDto>>;