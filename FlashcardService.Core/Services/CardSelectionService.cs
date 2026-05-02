using System.Diagnostics;
using FlashcardService.Core.Entities;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Services;

public class CardSelectionService
{
    public static CardSelectionService Default => _defaultInstance ??= new CardSelectionService();
    private static CardSelectionService? _defaultInstance;

    private readonly Random _random;

    private CardSelectionService()
    {
        _random = new Random();
    }

    public CardSelectionService(int seed)
    {
        _random = new Random(seed);
    }
}