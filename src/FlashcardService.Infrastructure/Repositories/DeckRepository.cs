using FlashcardService.Application.Common.Interfaces;
using FlashcardService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashcardService.Infrastructure.Repositories;

public sealed class DeckRepository(ApplicationDbContext context) : IDeckRepository
{
    public async Task<IEnumerable<Deck>> FindAllDecksAsync()
    {
        return await context.Decks.ToListAsync();
    }
}