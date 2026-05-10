using FlashcardService.Application.Common.Dtos;
using FlashcardService.Application.Common.Errors;
using FlashcardService.Application.Common.Interfaces;
using FlashcardService.Application.Decks.Commands.CreateCard;
using FlashcardService.Application.Decks.Commands.CreateDeck;
using FlashcardService.Application.Decks.Commands.UpdateCard;
using FlashcardService.Application.Decks.Commands.UpdateDeck;
using FlashcardService.Application.Decks.Queries.GetDeckById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardService.API.Controllers;

[ApiController]
[Route("decks")]
public sealed class DeckController(
    IDeckRepository deckRepository,
    IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeckDto>>> GetAllDecks()
    {
        var allDecks = await deckRepository.FindAllDecksAsync();
        return Ok(allDecks);
    }

    [HttpGet("{deckId:guid}")]
    public async Task<ActionResult<DeckDto>> GetDeckById(Guid deckId)
    {
        var query = new GetDeckByIdQuery(deckId);
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

    [HttpPut("{deckId:guid}")]
    public async Task UpdateDeck(Guid deckId, [FromBody] UpdateDeckCommand request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{deckId:guid}")]
    public async Task DeleteDeck(Guid deckId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{deckId:guid}/cards")]
    public async Task CreateDeckCard(Guid deckId, [FromBody] CreateCardCommand request)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{deckId:guid}/cards/{cardId:guid}")]
    public async Task UpdateDeckCard(Guid deckId, Guid cardId, [FromBody] UpdateCardCommand request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{deckId:guid}/cards/{cardId:guid}")]
    public async Task DeleteDeckCard(Guid deckId, Guid cardId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{deckId:guid}/cards/{cardId:guid}")]
    public async Task GetDeckCard(Guid deckId, Guid cardId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{deckId:guid}/cards")]
    public async Task GetDeckAllCards(Guid deckId)
    {
        throw new NotImplementedException();
    }
}