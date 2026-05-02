using FlashcardService.Core.Entities;
using FlashcardService.Core.Exceptions;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Tests;

public class CardTests
{
    [Fact]
    public void NewCardHasZeroInterval()
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");
        
        Assert.Equal(0, card.Interval);
    }

    [Fact]
    public void NewCardIsReadyForReview()
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");
        
        Assert.True(card.IsReadyForReview(DateTime.MaxValue));
    }
    
    [Fact]
    public void GivesInitialGrading()
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");

        card.Grade(CardGrade.Again, DateTime.MinValue);
        
        Assert.True(card.IsGraded);
        Assert.Equal(DateTime.MinValue, card.LastGradingDate);
        Assert.Equal(0.5, card.Interval);
    }

    [Fact]
    public void NotReadyForReviewImmediatelyAfterGrading()
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");
        
        card.Grade(CardGrade.Easy, new DateTime(0));
        
        Assert.False(card.IsReadyForReview(new DateTime(1)));
    }

    [Fact]
    public void ReadyForReviewAfterEnoughTimePassed()
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");
        
        card.Grade(CardGrade.Easy, new DateTime(0));
        
        Assert.True(card.IsReadyForReview(new DateTime(TimeSpan.FromDays(card.Interval).Ticks)));
    }

    [Fact]
    public void FailsToGradeInPast()
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");
        card.Grade(CardGrade.Hard, DateTime.MinValue);

        var exception = Record.Exception(() => card.Grade(CardGrade.Easy, DateTime.MinValue));
        
        Assert.NotNull(exception);
        Assert.IsType<DomainException>(exception);
    }

    [Theory] // https://open-spaced-repetition.github.io/anki_fsrs_visualizer, decimals adjusted for rounding problems
    [InlineData(3, 2.31, 3, 10.96, 3, 46.26, 3, 162.68)]
    [InlineData(3, 2.31, 3, 10.96, 3, 46.26, 2, 116.27)]
    [InlineData(3, 2.31, 3, 10.96, 3, 46.26, 1, 2.93)]
    [InlineData(2, 1.29, 3, 4.55, 3, 16.54, 3, 50.11)]
    [InlineData(1, 0.21, 3, 1.89, 3, 6.26, 3, 17.34)]
    [InlineData(4, 8.30, 3, 38.91, 3, 153.00, 1, 4.83)]
    public void GradingUpdatesStability(
        int grade1, decimal stability1,
        int grade2, decimal stability2,
        int grade3, decimal stability3,
        int grade4, decimal stability4)
    {
        var card = new Card(Guid.NewGuid(), "Front", "Back");
        int[] grades = [grade1, grade2, grade3, grade4];
        
        var calculated = new double[4];
        double daysElapsed = 0;
        for (var i = 0; i < 4; i++)
        {
            card.Grade(ConvertToCardGrade(grades[i]), new DateTime(TimeSpan.FromDays(daysElapsed).Ticks));
            calculated[i] = card.Interval;

            const double decay = -0.1542;
            var decayFactor = Math.Round(Math.Exp(Math.Pow(decay, -1) * Math.Log(0.9)) - 1, 8);
            var intervalModifier = Math.Round((Math.Pow(0.9, 1 / decay) - 1) / decayFactor, 8);
            var interval = Math.Min(
                Math.Max(1, Math.Round(card.Interval * intervalModifier)),
                36500); // 36500 = default max interval
            
            daysElapsed += interval;
        }

        Assert.Equal((double)stability1, calculated[0], precision: 2);
        Assert.Equal((double)stability2, calculated[1], precision: 2);
        Assert.Equal((double)stability3, calculated[2], precision: 2);
        Assert.Equal((double)stability4, calculated[3], precision: 2);
    }

    private static CardGrade ConvertToCardGrade(int gradeValue)
    {
        return gradeValue switch
        {
            4 => CardGrade.Easy,
            3 => CardGrade.Good,
            2 => CardGrade.Hard,
            1 => CardGrade.Again,
            _ => throw new DomainException("unknown card grade value")
        };
    }
}