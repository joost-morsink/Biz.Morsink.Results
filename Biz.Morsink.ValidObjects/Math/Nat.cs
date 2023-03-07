namespace Biz.Morsink.ValidObjects.Math;

public interface INat
{
    static abstract int Value { get; }  
    static abstract NatCounter ProcessNatCounter(NatCounter c);
}
public record struct NatCounter
{
    public static NatCounter Zero => new ();
    public int Billions { get; init; }
    public int Millions { get; init; }
    public int Thousands { get; init; }
    public int Hundreds { get; init; }
    public int Units { get; init; }
    public int Value
        => Billions * 1_000_000_000
            + Millions * 1_000_000
            + Thousands * 1_000
            + Hundreds * 100
            + Units;
    public int Positions
        => Billions > 0 ? 9 + Pos(Billions)
            : Millions > 0 ? 6 + Pos(Millions)
            : Thousands > 0 ? 3 + Pos(Thousands)
            : Hundreds > 0 ? 2 + Pos(Hundreds)
            : Pos(Units);
    private int Pos(int x)
        => x >= 100 ? 3
            : x >= 10 ? 2
            : 1;
}

public class Zero : INat, IInteger
{
    public static int Sign => 0;
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => counter;
}
public class One : One<Zero> { }
public class One<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 1 });
}
public class Two : Two<Zero> { }
public class Two<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 2 });
}
public class Three : Three<Zero> { }
public class Three<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 3 });
}
public class Four : Four<Zero> { }
public class Four<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 4 });
}
public class Five : Five<Zero> { }
public class Five<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 5 });
}
public class Six : Six<Zero> { }
public class Six<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 6 });
}
public class Seven : Seven<Zero> { }
public class Seven<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 7 });
}
public class Eight : Eight<Zero> { }
public class Eight<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 8 });
}
public class Nine : Nine<Zero> { }
public class Nine<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 9 });
}
public class Ten : Ten<Zero> { }
public class Ten<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 10 });
}
public class Eleven : Eleven<Zero> { }
public class Eleven<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 11 });
}
public class Twelve : Twelve<Zero> { }
public class Twelve<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 12 });
}
public class Thirteen : Thirteen<Zero> { }
public class Thirteen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 13 });
}
public class Fourteen : Fourteen<Zero> { }
public class Fourteen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 14 });
}
public class Fifteen : Fifteen<Zero> { }
public class Fifteen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 15 });
}
public class Sixteen : Sixteen<Zero> { }
public class Sixteen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 16 });
}
public class Seventeen : Seventeen<Zero> { }
public class Seventeen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 17 });
}
public class Eighteen : Eighteen<Zero> { }
public class Eighteen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 18 });
}
public class Nineteen : Nineteen<Zero> { }
public class Nineteen<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 19 });
}
public class Twenty : Twenty<Zero> { }
public class Twenty<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 20 });
}
public class Thirty : Thirty<Zero> { }
public class Thirty<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 30 });
}
public class Forty : Forty<Zero> { }
public class Forty<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 40 });
}
public class Fifty : Fifty<Zero> { }
public class Fifty<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 50 });
}
public class Sixty : Sixty<Zero> { }
public class Sixty<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 60 });
}
public class Seventy : Seventy<Zero> { }
public class Seventy<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 70 });
}
public class Eighty : Eighty<Zero> { }
public class Eighty<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 80 });
}
public class Ninety : Ninety<Zero> { }
public class Ninety<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Units = counter.Units + 90 });
}
public class Hundred : Hundred<Zero> { }
public class Hundred<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Hundreds = counter.Units, Units= 0 });
}
public class Thousand : Thousand<Zero> { }
public class Thousand<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Thousands = 100 * counter.Hundreds + counter.Units, Hundreds = 0, Units = 0 });
}
public class Million : Million<Zero> { }
public class Million<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Millions = 100 * counter.Hundreds + counter.Units, Hundreds = 0, Units = 0 });
}
public class Billion : Billion<Zero> { }
public class Billion<T> : INat
    where T : INat
{
    public static int Value { get; } = ProcessNatCounter(NatCounter.Zero).Value;
    public static NatCounter ProcessNatCounter(NatCounter counter)
        => T.ProcessNatCounter(counter with { Billions = 100 * counter.Hundreds + counter.Units, Hundreds = 0, Units = 0 });
}