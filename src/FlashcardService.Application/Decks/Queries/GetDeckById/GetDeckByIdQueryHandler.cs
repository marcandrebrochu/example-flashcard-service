using FlashcardService.Application.Common.Dtos;
using FluentResults;
using MediatR;

namespace FlashcardService.Application.Decks.Queries.GetDeckById;

public class GetDeckByIdQueryHandler : IRequestHandler<GetDeckByIdQuery, Result<DeckDto>>
{
    public Task<Result<DeckDto>> Handle(GetDeckByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}