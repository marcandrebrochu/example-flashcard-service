using Microsoft.AspNetCore.Mvc;

namespace FlashcardService.API.Controllers;

[ApiController]
[Route("decks")]
public sealed class DeckController : ControllerBase
{
    [HttpGet("magic")]
    public int GetMagic() => 42;
}