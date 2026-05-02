using System.Collections;
using System.Diagnostics;
using FlashcardService.Core.Exceptions;

namespace FlashcardService.Core.Entities;

public sealed class Deck(Guid id) : Entity(id), IReadOnlyList<Card>
{
    private readonly List<Card> _cards = [];
    
    public int Count => _cards.Count;

    public Card this[int index] => _cards[index];

    public Deck() : this(Guid.NewGuid())
    {
    }

    public IEnumerator<Card> GetEnumerator()
    {
        return _cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_cards).GetEnumerator();
    }
    
    public Card AddCard(string front, string back)
    {
        var duplicateExists = CardWithFront(front) is not null;
        if (duplicateExists)
            throw new DomainException("no two cards in the same deck may share the same front");
        
        var card = new Card(Guid.NewGuid(), front, back);
        _cards.Add(card);
        return card;
    }

    public void UpdateCardFront(Card card, string frontNew)
    {
        Debug.Assert(_cards.Contains(card));
        
        var duplicateExists = CardWithFront(frontNew) is { } c && c != card;
        if (duplicateExists)
            throw new DomainException("no two cards in the same deck may share the same front");

        card.Front = frontNew;
    }

    private Card? CardWithFront(string front)
    {
        return _cards.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x.Front, front));
    }
}
