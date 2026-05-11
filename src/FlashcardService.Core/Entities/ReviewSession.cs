using FlashcardService.Core.Exceptions;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Entities;

public sealed class ReviewSession(Guid id, Guid deckId) : Entity(id)
{
    private readonly List<Card> _reviewedCards = [];
    
    public IReadOnlyCollection<Card> ReviewedCards => _reviewedCards;

    public Guid DeckId { get; } = deckId;

    public void MarkAsReviewed(Card card)
    {
        if (_reviewedCards.Contains(card))
            throw new DomainException("Card already reviewed");
        
        _reviewedCards.Add(card);
    }

    public bool HasBeenReviewed(Card card)
    {
        return _reviewedCards.Contains(card);
    }
}