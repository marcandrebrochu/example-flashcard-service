using FlashcardService.Core.Entities;
using FlashcardService.Core.Services;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Tests.Services;

public sealed class CardReviewServiceTests
{
    [Fact]
    public void FindsCardToReview()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var session = new Session(Guid.NewGuid(), deck.Id);
        var card = deck.AddCard("Front", "Back");
        
        var cardToReview = CardReviewService.FindCardToReview(deck, session);

        Assert.Equal(card, cardToReview);
    }
    
    [Fact]
    public void FindsOnlyUnreviewedCardToReview()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var session = new Session(Guid.NewGuid(), deck.Id);
        
        // Add 100 cards that we review immediately
        for (var i = 0; i < 100; i++)
        {
            var c = deck.AddCard($"Front {i}", "Back");
            c.Grade(CardGrade.Easy, new DateTime(ticks: i));
        }
        
        var card = deck.AddCard("Front 100", "Back");
        
        CardReviewService.Seed(42);
        var cardToReview = CardReviewService.FindCardToReview(deck, session);
        
        Assert.Equal(card, cardToReview);
    }
}