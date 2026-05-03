using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Commands.CreateDeck;

public sealed class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, Result<CreatedDeck>>
{
    public Task<Result<CreatedDeck>> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}