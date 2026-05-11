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

    public static Card? FindCardToReview(Deck deck, ReviewSession session, DateTime now)
    {
        Debug.Assert(session.DeckId == deck.Id);

        var cardsReadyForReview = deck.GetCardsReadyForReview(now);
        if (cardsReadyForReview.Count == 0)
            return null;
        
        var unreviewedCardsReadyForReview = cardsReadyForReview.Where(card => !session.HasBeenReviewed(card)).ToList();
        if (unreviewedCardsReadyForReview.Count == 0)
            return null;
        
        var randomIndex = _random.Next(0, unreviewedCardsReadyForReview.Count);
        
        return unreviewedCardsReadyForReview[randomIndex];
    }
}