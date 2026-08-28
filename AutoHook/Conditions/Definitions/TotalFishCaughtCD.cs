using static AutoHook.Conditions.IConditionDefinition;

namespace AutoHook.Conditions.Definitions;

// Counts every fish caught since the last counter reset, no matter which fish it was.
// (FishCaughtCounterCD is the same idea, but scoped to one specific fish.)
public sealed class TotalFishCaughtCD : IConditionDefinition {
    public string Id => nameof(TotalFishCaughtCD);
    public string Name => "Total fish caught (any)";
    public ConditionScopeFlags AllowedScopes => ConditionScopeFlags.Hook | ConditionScopeFlags.FishIgnore | ConditionScopeFlags.AutoCast;

    public bool Evaluate(WorldState world, IReadOnlyDictionary<string, object> parameters) {
        var args = GetIntCompareParams(parameters, defaultValue: 1);
        var total = FishingManager.FishingHelper.GetTotalFishCaught(world);
        return args.Apply(CompareInt(total, args.Value, args.Op));
    }

    public void DrawParams(Condition condition)
        => DrawIntCompareParams(condition, "##total_fish_caught_op", "Count", defaultValue: 1, clamp: v => Math.Max(0, v), valueWidth: 60);

    public string DescribeParameters(IReadOnlyDictionary<string, object> parameters)
        => $"any fish {ConditionParameterFormat.FormatIntCompare(parameters, defaultValue: 1)}";
}
