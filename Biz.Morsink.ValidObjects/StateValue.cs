using SVT = System.UInt64;

namespace Biz.Morsink.ValidObjects;
public struct StateValue : IEquatable<StateValue>
{

    private readonly SVT _value;

    public StateValue(SVT value)
    {
        _value = value;
    }
    public bool Equals(StateValue other) => _value == other._value;
    public override bool Equals(object? obj) => obj is StateValue other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();

    public static StateValue Zero => 0;
    public static StateValue One => 1;
    public static implicit operator SVT(StateValue stateValue)
        => stateValue._value;
    public static implicit operator StateValue(SVT svt)
        => new(svt);
    public static StateValue operator +(StateValue left, StateValue right)
        => left._value + right._value;
    public static StateValue operator *(StateValue left, StateValue right)
        => left._value * right._value;
    public static StateValue operator ^(StateValue left, StateValue right)
        => left._value ^ right._value;
    public static bool operator ==(StateValue left, StateValue right)
        => left._value == right._value;
    public static bool operator !=(StateValue left, StateValue right)
        => left._value != right._value;
    public StateValue Rotate()
        => SVT.RotateLeft(this, 11);
    private static Random _rnd = new Random();

    public static StateValue Next()
        => (ulong)_rnd.NextInt64();
}