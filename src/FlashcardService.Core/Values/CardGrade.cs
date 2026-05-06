using FlashcardService.Core.Exceptions;

namespace FlashcardService.Core.Values;

public enum CardGrade
{
    Easy,
    Good,
    Hard,
    Again,
}

public static class CardGradeExtensions
{
    public static int ToInt(this CardGrade grade)
    {
        return grade switch
        {
            CardGrade.Easy => 4,
            CardGrade.Good => 3,
            CardGrade.Hard => 2,
            CardGrade.Again => 1,
            _ => throw new DomainException("unknown card grade")
        };
    }
}