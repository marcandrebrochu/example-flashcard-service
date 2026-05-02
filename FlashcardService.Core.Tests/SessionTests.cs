using FlashcardService.Core.Entities;
using FlashcardService.Core.Exceptions;

namespace FlashcardService.Core.Tests;

public class SessionTests
{
    [Fact]
    public void InitiallyHaveNoCardsReviewed()
    {
        var deck = new Deck("Test deck");
        var session = new Session(deck.Id);
        
        Assert.Empty(session.ReviewedCards);
    }

    [Fact]
    public void MarksAsReviewed()
    {
        var deck = new Deck("Test deck");
        var session = new Session(deck.Id);
        var card = deck.AddCard("When was Quebec City founded?", "July 3, 1608");

        session.MarkAsReviewed(card);
        
        Assert.Single(session.ReviewedCards);
        Assert.Equal(card, session.ReviewedCards.First());
    }

    [Fact]
    public void PreventsMarkingAsReviewedTwice()
    {
        var deck = new Deck("Test deck");
        var session = new Session(deck.Id);
        var card = deck.AddCard("What is the name for a group of crows?", "Murder");
        
        var exception = Record.Exception(() =>
        {
            session.MarkAsReviewed(card);
            session.MarkAsReviewed(card);
        });
        
        Assert.NotNull(exception);
        Assert.IsType<DomainException>(exception);
    }
}