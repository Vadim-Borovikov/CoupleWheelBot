using AbstractBot;
using CoupleWheelBot.Contexts;
using JetBrains.Annotations;

namespace CoupleWheelBot.Save;

public sealed class Data : SaveData
{
    [UsedImplicitly]
    public Dictionary<long, Answer> AnswerContexts { get; set; } = new();
}