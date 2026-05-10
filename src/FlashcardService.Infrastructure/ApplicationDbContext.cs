using FlashcardService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashcardService.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Deck> Decks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deck>(deck =>
        {
            deck.HasKey(nameof(Deck.Id));
        });
    }
}