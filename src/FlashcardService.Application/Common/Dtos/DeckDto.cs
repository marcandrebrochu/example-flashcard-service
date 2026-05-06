namespace FlashcardService.Application.Common.Dtos;

public sealed record DeckDto
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
}