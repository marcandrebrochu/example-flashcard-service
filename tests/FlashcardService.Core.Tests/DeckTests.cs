using FlashcardService.Core.Entities;
using FlashcardService.Core.Exceptions;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Tests;

public class DeckTests
{
    [Fact]
    public void ShouldAddCard()
    {
        var deck = new Deck(Guid.NewGuid(), "Test deck");
        
        var card = deck.AddCard("Front", "Back");

        Assert.Single(deck);
        Assert.Equal(card, deck[0]);
    }
    
    [Fact]
    public void ShouldNotAllowAddingCardsWithSameFront()
    {
        var deck = new Deck(Guid.NewGuid(), "Test deck");

        var exception = Record.Exception(() =>
        {
            _ = deck.AddCard("front", "Back 1");
            _ = deck.AddCard("Front", "Back 2");
        });
        
        Assert.NotNull(exception);
        Assert.IsType<DomainException>(exception);
    }
    
    [Fact]
    public void ShouldUpdateCardFront()
    {
        var deck = new Deck(Guid.NewGuid(), "Test deck");
        var card = deck.AddCard("Front (original)", "Back");

        deck.UpdateCardFront(card, "Front (new)");
        
        Assert.Equal("Front (new)", card.Front);
    }

    [Fact]
    public void ShouldNotAllowUpdatingCardFrontToDuplicate()
    {
        var deck = new Deck(Guid.NewGuid(), "Test deck");
        var card = deck.AddCard("Front 1", "Back");
        _ = deck.AddCard("Front 2", "Back");
        
        var exception = Record.Exception(() => deck.UpdateCardFront(card, "Front 2"));
        
        Assert.NotNull(exception);
        Assert.IsType<DomainException>(exception);
    }

    [Fact]
    public void NotAllowEmptyNameOnConstruction()
    {
        var exception = Record.Exception(() => new Deck(Guid.NewGuid(), ""));
        
        Assert.NotNull(exception);
        Assert.IsType<DomainException>(exception);
    }

    [Fact]
    public void NotAllowSettingEmptyName()
    {
        var deck = new Deck(Guid.NewGuid(), "Test deck");

        var exception = Record.Exception(() => deck.Name = "");
        
        Assert.NotNull(exception);
        Assert.IsType<DomainException>(exception);
    }

    [Fact]
    public void FindsCardsReadyForReview()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var card1 = deck.AddCard("Front 1", "Back");
        var card2 = deck.AddCard("Front 2", "Back");
        var card3 = deck.AddCard("Front 3", "Back");
        
        card3.Grade(CardGrade.Easy, new DateTime(ticks: 0));

        var cardsReadyForReview = deck.GetCardsReadyForReview(new DateTime(ticks: 0));
        
        Assert.Equal(2, cardsReadyForReview.Count);
        Assert.Equal(card1, cardsReadyForReview[0]);
        Assert.Equal(card2, cardsReadyForReview[1]);
    }
    
    [Fact]
    public void FindsCardsReadyForReviewAfterInterval()
    {
        var deck = new Deck(Guid.NewGuid(), "Deck");
        var card1 = deck.AddCard("Front 1", "Back");
        var card2 = deck.AddCard("Front 2", "Back");
        var card3 = deck.AddCard("Front 3", "Back");
        
        card3.Grade(CardGrade.Again, new DateTime(ticks: 0));

        var cardsReadyForReview = deck.GetCardsReadyForReview(card3.NextReviewDate);
        
        Assert.Equal(3, cardsReadyForReview.Count);
        Assert.Equal(card1, cardsReadyForReview[0]);
        Assert.Equal(card2, cardsReadyForReview[1]);
        Assert.Equal(card3, cardsReadyForReview[2]);
    }
}