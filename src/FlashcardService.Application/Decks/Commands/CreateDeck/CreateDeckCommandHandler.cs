using FlashcardService.Application.Common.Dtos;
using FlashcardService.Application.Common.Interfaces;
using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Commands.CreateDeck;

public sealed class CreateDeckCommandHandler(IIdentifierService identifierService)
    : IRequestHandler<CreateDeckCommand, Result<DeckDto>>
{
    public async Task<Result<DeckDto>> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        return Result.Ok(new DeckDto { Id = identifierService.GenerateGuid(), Name = request.Name });
    }
}