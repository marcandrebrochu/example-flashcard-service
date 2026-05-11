using FlashcardService.Core.Exceptions;
using FlashcardService.Core.Values;

namespace FlashcardService.Core.Entities;

public sealed class Card(Guid id, string front, string back) : Entity(id)
{
    public string Front { get; set; } = front;

    public string Back { get; } = back;

    public DateTime? LastGradingDate { get; private set; }

    public void Grade(CardGrade grade, DateTime now)
    {
        double[] w =
            // [
            //     0.1033, 0.1033, 1.0978, 20.3628, 6.4315, 0.6296, 2.4602, 0.0014, 1.3171, 0.1323, 1.0180, 1.6567, 0.0968,
            //     0.2038, 1.9805, 0.2279, 4.2558, 0.9442, 0.6892, 0.2048, 0.1201
            // ];
            [
                0.212, 1.2931, 2.3065, 8.2956, 6.4133, 0.8334, 3.0194, 0.001, 1.8722,
                0.1666, 0.796, 1.4835, 0.0614, 0.2629, 1.6483, 0.6014, 1.8729, 0.5425,
                0.0912, 0.0658, 0.1542,
            ];
        var g = grade.ToInt();

        if (_state is null)
        {
            _state = new MemoryState(
                Stability: w[g - 1],
                Difficulty: Math.Clamp(w[4] - Math.Exp(w[5] * (g - 1)) + 1, 1, 10));
        }
        else
        {
            if (LastGradingDate is null)
                throw new StateException("last grading date should not be null when card is not new");
            
            if (now.CompareTo(LastGradingDate.Value) <= 0)
                throw new DomainException("grading a card may only happen chronologically");

            var (s, d) = _state;

            var daysSinceLastReview = now.Subtract(LastGradingDate.Value).TotalDays;
            var isSameDayReview = daysSinceLastReview == 0;
            
            var factor = Math.Pow(0.9, -1 / w[20]) - 1;
            var r = Math.Pow(1 + factor * daysSinceLastReview / s, -w[20]);

            var initialDifficultyForEasyGrade = Math.Clamp(w[4] - Math.Exp(w[5] * 3) + 1, 1, 10);
            var deltaD = -w[6] * (g - 3);
            var difficultyLinearDamping = d + deltaD * (10 - d) / 9;
            var newD = Math.Clamp(w[7] * initialDifficultyForEasyGrade + (1 - w[7]) * difficultyLinearDamping, 1, 10);

            var hardFactor = g == 2 ? w[15] : 1;
            var easyFactor = g == 4 ? w[16] : 1;

            var stabilityIncrease =
                Math.Exp(w[8])
                * (11 - d)
                * Math.Pow(s, -w[9])
                * (Math.Exp(w[10] * (1 - r)) - 1)
                * hardFactor
                * easyFactor
                + 1;

            var stabilityWhenNotAgain = Math.Clamp(s * stabilityIncrease, 0.001, 36500);
            var stabilityWhenAgain =
                Math.Clamp(
                    w[11] * 
                    Math.Pow(d, -w[12]) * 
                    (Math.Pow(s + 1, w[13]) - 1) * 
                    Math.Exp(w[14] * (1 - r)),
                    0.001, 36500);
            var shortTermStabilityInc = Math.Pow(s, -w[19]) * Math.Exp(w[17] * (g - 3 + w[18]));
            var maskedSinc = g >= 3 ? Math.Max(shortTermStabilityInc, 1) : shortTermStabilityInc;
            var shortTermStability = Math.Clamp(s * maskedSinc, 0.001, 36500);

            var newS = isSameDayReview
                ? shortTermStability
                : grade == CardGrade.Again
                    ? stabilityWhenAgain
                    : stabilityWhenNotAgain;

            _state = new MemoryState(
                Stability: newS,
                Difficulty: newD);
        }

        LastGradingDate = now;
    }

    public double Interval => (_state?.Stability).GetValueOrDefault(0);
    
    public DateTime NextReviewDate => LastGradingDate.GetValueOrDefault(new DateTime(ticks: 0)).AddDays(Interval);

    public bool IsReadyForReview(DateTime now)
    {
        return now.CompareTo(NextReviewDate) >= 0;
    }

    private MemoryState? _state;
}