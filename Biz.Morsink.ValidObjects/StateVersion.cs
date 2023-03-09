namespace Biz.Morsink.ValidObjects;

public class StateVersion
{
    private static StateValue NewNumber()
        => StateValue.Next();

    public static StateValue Single() => NewNumber();
    
    private StateValue _value;
    private StateValue _multiplier;
    private StateValue _addition;
    public StateVersion()
    {
        _value = NewNumber();
        _multiplier = NewNumber() | 1;
        _addition = NewNumber();
    }
    public StateValue Value => _value;

    public void Next()
    {
        _value *= _multiplier;
        _value += _addition;
        _value = _value.Rotate();
    }

    public static implicit operator StateValue(StateVersion sv) => sv.Value;
    public static StateValue Combine(params IHasStateVersion[] hasStateVersions)
    {
        var result = default(StateValue);
        foreach (var hasStateVersion in hasStateVersions)
            result ^= hasStateVersion.GetStateVersion();
        return result;
    }

    public static StateValue Combine(IEnumerable<IHasStateVersion> hasStateVersions)
    {
        var result = default(StateValue);
        foreach (var hasStateVersion in hasStateVersions)
            result ^= hasStateVersion.GetStateVersion();
        return result;
    }
}