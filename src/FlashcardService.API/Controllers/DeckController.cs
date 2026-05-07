using FlashcardService.Application.Common.Dtos;
using FlashcardService.Application.Common.Errors;
using FlashcardService.Application.Decks.Commands.CreateDeck;
using FlashcardService.Application.Decks.Queries.GetDeckById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardService.API.Controllers;

[ApiController]
[Route("decks")]
public sealed class DeckController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeckDto>>> GetAllDecks()
    {
        return Ok((IEnumerable<DeckDto>)[]);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeckDto>> GetDeckById(Guid id)
    {
        var query = new GetDeckByIdQuery(id);
        var result = await mediator.Send(query);

        if (result.HasError<NotFoundError>())
            return NotFound();

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<DeckDto>> CreateDeck([FromBody] CreateDeckCommand request)
    {
        var result = await mediator.Send(request);
        
        const string routeName = nameof(GetDeckById);
        var routeValues = new { id = Guid.NewGuid() }; // fills in the {id} slot in the get url (i.e. routeName)
        return CreatedAtAction(routeName, routeValues, result.Value);
    }
}