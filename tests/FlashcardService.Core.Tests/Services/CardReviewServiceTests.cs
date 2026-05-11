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
        
        var cardToReview = CardReviewService.FindCardToReview(deck, session, new DateTime(ticks: 0));

        Assert.NotNull(cardToReview);
        Assert.Equal(card, cardToReview);
    }
    
    [Fact]
    public void FindsOnlyCardsToReview()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var session = new Session(Guid.NewGuid(), deck.Id);
        
        // Add 100 cards that we review immediately
        for (var i = 0; i < 100; i++)
        {
            var c = deck.AddCard($"Front {i}", "Back");
            c.Grade(CardGrade.Easy, new DateTime(ticks: 0));
        }
        
        var card = deck.AddCard("Front 100", "Back");
        
        CardReviewService.Seed(42);
        var cardToReview = CardReviewService.FindCardToReview(deck, session, new DateTime(ticks: 2));
        
        Assert.NotNull(cardToReview);
        Assert.Equal(card, cardToReview);
    }

    [Fact]
    public void FindsNoCardsToReview()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var session = new Session(Guid.NewGuid(), deck.Id);
        
        var cardToReview = CardReviewService.FindCardToReview(deck, session, new DateTime(ticks: 0));
        
        Assert.Null(cardToReview);
    }
    
    [Fact]
    public void FindsOnlyUnreviewedCardsToReview()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var session = new Session(Guid.NewGuid(), deck.Id);
        
        // Add 100 cards that we mark as reviewed immediately
        for (var i = 0; i < 100; i++)
        {
            var c = deck.AddCard($"Front {i}", "Back");
            session.MarkAsReviewed(c);
        }
        
        var card = deck.AddCard("Front 100", "Back");
        
        CardReviewService.Seed(42);
        var cardToReview = CardReviewService.FindCardToReview(deck, session, new DateTime(ticks: 2));
        
        Assert.NotNull(cardToReview);
        Assert.Equal(card, cardToReview);
    }
}