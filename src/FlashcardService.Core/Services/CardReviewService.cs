using System.Diagnostics;
using FlashcardService.Core.Entities;

namespace FlashcardService.Core.Services;

public static class CardReviewService
{
    private static Random _random = new();

    public static void Seed(int seed)
    {
        _random = new Random(seed);
    }

    public static Card FindCardToReview(Deck deck, Session session)
    {
        Debug.Assert(session.DeckId == deck.Id);

        return deck[0];
    }
}