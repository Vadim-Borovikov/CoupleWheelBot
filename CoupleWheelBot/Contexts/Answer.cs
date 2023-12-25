using AbstractBot;
using JetBrains.Annotations;

namespace CoupleWheelBot.Contexts;

public sealed class Answer : Context
{
    [UsedImplicitly]
    public CoupleCondition CoupleCondition { get; init; }

    public Answer() { }

    public Answer(long userId, byte questionsNumber) => CoupleCondition = new CoupleCondition(userId, questionsNumber);
}