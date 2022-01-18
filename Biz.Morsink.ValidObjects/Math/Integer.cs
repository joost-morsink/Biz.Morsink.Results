namespace Biz.Morsink.ValidObjects.Math;

public interface ISign
{
    static abstract int Sign { get; }
}
public interface IInteger
{
    static abstract int Sign { get; }
    static abstract int Value { get; }
}
public class PlusSign : ISign 
{
    public static int Sign => 1;
}
public class MinusSign : ISign 
{
    public static int Sign => -1;
}
public class Integer<Sgn,Num> : IInteger
    where Sgn : ISign
    where Num : INat
{
    public static int Sign => System.Math.Sign(Sgn.Sign);
    public static int Value => Num.Value * Sign;
}
public class Plus<Num> : Integer<PlusSign, Num>
    where Num : INat
{
}
public class Minus<Num> : Integer<MinusSign, Num>
    where Num : INat
{
}
