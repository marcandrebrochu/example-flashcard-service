namespace FlashcardService.Application.Decks.Commands.CreateDeck;

public sealed record CreatedDeck
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
}