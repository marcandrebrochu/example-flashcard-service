using FlashcardService.Core.Exceptions;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Entities;

public sealed class Session(Guid id, Guid deckId) : Entity(id)
{
    private readonly List<Card> _reviewedCards = [];
    
    public IReadOnlyCollection<Card> ReviewedCards => _reviewedCards;

    public Guid DeckId { get; } = deckId;
    
    public Session(Guid deckId) : this(Guid.NewGuid(), deckId)
    {
    }

    public void MarkAsReviewed(Card card)
    {
        if (_reviewedCards.Contains(card))
            throw new DomainException("Card already reviewed");
        
        _reviewedCards.Add(card);
    }
}