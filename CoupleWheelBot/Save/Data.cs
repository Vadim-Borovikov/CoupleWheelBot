﻿using AbstractBot;
using CoupleWheelBot.Contexts;
using JetBrains.Annotations;

namespace CoupleWheelBot.Save;

public sealed class Data : SaveData
{
    [UsedImplicitly]
    public Dictionary<long, Partner> PartnerContexts { get; set; } = new();

    internal IEnumerable<KeyValuePair<long, Partner>> GetContextsWith(Guid guid)
    {
        return PartnerContexts.Where(p => p.Value.CoupleId == guid).OrderByDescending(p => p.Value.IsInitiator);
    }
}