using AbstractBot;
using JetBrains.Annotations;

namespace CoupleWheelBot.Contexts;

public sealed class Answer : Context
{
    [UsedImplicitly]
    public Guid Guid { get; init; }
}