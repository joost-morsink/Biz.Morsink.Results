namespace Biz.Morsink.ValidObjects.Math;
using static Point;

internal static class Point
{
    public struct PositionAndValue
    {
        public PositionAndValue(int value, int positions)
        {
            Value = value;
            Positions = positions;
        }
        public int Value { get; }
        public int Positions { get; }
        public PositionAndValue Combine(PositionAndValue other)
            => new (Value * (int)PowersOfTen.Get(other.Positions) + other.Value, Positions + other.Positions);
    }
    public static PositionAndValue GetPositionAndValue<T>()
        where T : INat
        => new(T.Value, T.ProcessNatCounter(NatCounter.Zero).Positions);
}

public interface IReal
{
    static abstract decimal Value { get; }
}
public interface IFractional
{
    static abstract decimal Fraction { get; }
}
internal class PowersOfTen
{
    private static decimal[] Powers;
    private static decimal[] NegativePowers;
    static PowersOfTen()
    {
        Powers = new decimal[20];
        var x = 1m;
        for (int i = 0; i < Powers.Length; i++, x *= 10m)
            Powers[i] = x;
        
        NegativePowers = new decimal[20];
        x = 1m;
        for (int i = 0; i < Powers.Length; i++, x *= 0.1m)
            NegativePowers[i] = x;
    }
    public static decimal Get(int power)
        => power < 0 ? NegativePowers[-power] : Powers[power];
}
public class Point<T> : IFractional
    where T : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = GetPositionAndValue<T>();
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Point<T, U> : IFractional
    where T : INat
    where U : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = new []
            {
                GetPositionAndValue<T>(),
                GetPositionAndValue<U>()
            }.Aggregate((t, u) => t.Combine(u));
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Point<T, U, V> : IFractional
    where T : INat
    where U : INat
    where V : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = new []
            {
                GetPositionAndValue<T>(),
                GetPositionAndValue<U>(),
                GetPositionAndValue<V>()
            }.Aggregate((t, u) => t.Combine(u));
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Point<T, U, V, W> : IFractional
    where T : INat
    where U : INat
    where V : INat
    where W : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = new []
            {
                GetPositionAndValue<T>(),
                GetPositionAndValue<U>(),
                GetPositionAndValue<V>(),
                GetPositionAndValue<W>()
            }.Aggregate((t, u) => t.Combine(u));
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Point<T, U, V, W, X> : IFractional
    where T : INat
    where U : INat
    where V : INat
    where W : INat
    where X : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = new []
            {
                GetPositionAndValue<T>(),
                GetPositionAndValue<U>(),
                GetPositionAndValue<V>(),
                GetPositionAndValue<W>(),
                GetPositionAndValue<X>()
            }.Aggregate((t, u) => t.Combine(u));
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Point<T, U, V, W, X, Y> : IFractional
    where T : INat
    where U : INat
    where V : INat
    where W : INat
    where X : INat
    where Y : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = new []
            {
                GetPositionAndValue<T>(),
                GetPositionAndValue<U>(),
                GetPositionAndValue<V>(),
                GetPositionAndValue<W>(),
                GetPositionAndValue<X>(),
                GetPositionAndValue<Y>()
            }.Aggregate((t, u) => t.Combine(u));
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Point<T, U, V, W, X, Y, Z> : IFractional
    where T : INat
    where U : INat
    where V : INat
    where W : INat
    where X : INat
    where Y : INat
    where Z : INat
{
    public static decimal Fraction
    {
        get
        {
            var pav = new []
            {
                GetPositionAndValue<T>(),
                GetPositionAndValue<U>(),
                GetPositionAndValue<V>(),
                GetPositionAndValue<W>(),
                GetPositionAndValue<X>(),
                GetPositionAndValue<Y>(),
                GetPositionAndValue<Z>()
            }.Aggregate((t, u) => t.Combine(u));
            return PowersOfTen.Get(-pav.Positions) * pav.Value;
        }
    } 
}
public class Real<I,F> : IReal
    where I : IInteger
    where F : IFractional
{
    public static decimal Value =>  I.Value + F.Fraction * I.Sign;
}

