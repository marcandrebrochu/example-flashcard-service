using FlashcardService.Application.Common.Dtos;
using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Commands.CreateDeck;

public sealed class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, Result<DeckDto>>
{
    public Task<Result<DeckDto>> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}