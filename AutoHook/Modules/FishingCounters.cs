namespace AutoHook.Modules;

public static class FishingCounters {
    private static Dictionary<Guid, int> FishCount = [];
    private static List<Guid> FishPresetSwapped = [];
    private static List<Guid> FishBaitSwapped = [];
    private static readonly List<Guid> ToBeRemoved = [];

    // Baseline for the "any fish" counter: the session total (WorldState) at the last counter reset.
    private static int TotalFishCaughtOffset;

    public static void AddFishCount(Guid guid) {
        FishCount.TryAdd(guid, 0);
        FishCount[guid]++;
    }

    public static void AddBaitSwap(Guid guid) {
        if (!FishBaitSwapped.Contains(guid))
            FishBaitSwapped.Add(guid);
    }

    public static void AddPresetSwap(Guid guid) {
        if (!FishPresetSwapped.Contains(guid))
            FishPresetSwapped.Add(guid);
    }

    public static void RemovePresetSwap(Guid guid) {
        if (SwappedPreset(guid))
            FishPresetSwapped.Remove(guid);
    }

    // Total amount of fish caught since the last counter reset, regardless of which fish it was.
    public static int GetTotalFishCaught(WorldState world)
        => Math.Max(0, world.Fishing.FishCaughtCounts.Values.Sum() - TotalFishCaughtOffset);

    public static void ResetTotalFishCaught()
        => TotalFishCaughtOffset = WorldState.Get().Fishing.FishCaughtCounts.Values.Sum();

    public static int GetFishCount(Guid guid)
        => !FishCount.TryGetValue(guid, out var value) ? 0 : value;

    public static bool SwappedBait(Guid guid)
        => FishBaitSwapped.Any(g => g == guid);

    public static bool SwappedPreset(Guid guid)
        => FishPresetSwapped.Any(g => g == guid);

    public static void RemoveId(Guid guid) {
        FishCount.Remove(guid);
        if (SwappedPreset(guid))
            FishPresetSwapped.Remove(guid);
        if (SwappedBait(guid))
            FishBaitSwapped.Remove(guid);
    }

    public static void QueueRemove(Guid guid) {
        if (!ToBeRemoved.Contains(guid))
            ToBeRemoved.Add(guid);
    }

    public static void RemoveGuidQueue() {
        foreach (var guid in ToBeRemoved)
            RemoveId(guid);
        ToBeRemoved.Clear();
    }

    public static void Reset() {
        FishCount = [];
        FishPresetSwapped = [];
        FishBaitSwapped = [];
        ToBeRemoved.Clear();
        TotalFishCaughtOffset = 0;
    }
}
