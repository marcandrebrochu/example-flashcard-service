using FlashcardService.Core.Entities;

namespace FlashcardService.Core.Tests.Builders;

public static class DeckBuilder
{
    public static Deck WithCards(params (string, string)[] cards)
    {
        var deck = new Deck("Test deck");
        foreach (var (front, back) in cards)
        {
            deck.AddCard(front, back);
        }

        return deck;
    }
}