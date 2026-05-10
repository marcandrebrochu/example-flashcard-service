using FlashcardService.Core.Entities;

namespace FlashcardService.Application.Common.Interfaces;

public interface IDeckRepository
{
    public Task<IEnumerable<Deck>> FindAllDecksAsync();
}