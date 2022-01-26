using System.Collections.Immutable;
using Biz.Morsink.Results;
namespace Biz.Morsink.ValidObjects.Test;

using NonEmptyString = Valid<string, NotEmpty>;
using ZipCodeString = Valid<string, DutchZipCode>;
using NaturalNumber = Valid<int, MinValue<Zero>>;

public class DutchZipCode : RegexConstraint
{
    public static DutchZipCode Instance { get; } = new ();
    public DutchZipCode() : base("^[0-9]{4}[A-Z]{2}$")
    {
    }
}
public class Identifier : RegexConstraint
{
    public static Identifier Instance { get; } = new ();
    public Identifier() : base("^[A-Za-z_][A-Za-z_0-9]*")
    {
    }
}
[Generate]
public partial class Address
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
}
[Generate]
public partial class Person
{
    public NonEmptyString FirstName { get; }
    public NonEmptyString LastName { get; }
    public NaturalNumber Age { get; }
    public ImmutableList<Address> Addresses { get; }
    public IImmutableSet<Valid<string, Identifier>> Tags { get; }
}


public struct Natural
{
    private readonly Valid<int, MinValue<Zero>> _value;
    public Natural(Valid<int, MinValue<Zero>> value)
    {
        _value = value;
    }
    public int Value => _value.Value;
    public static implicit operator Natural(Valid<int, MinValue<Zero>> value)
        => new (value);
    public static implicit operator Valid<int, MinValue<Zero>>(Natural value)
        => value._value;
    public static implicit operator int(Natural value)
        => value.Value;
    
    public static Natural operator +(Natural x, Natural y)
        => new (Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());
    public static Natural operator *(Natural x, Natural y)
        => new (Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());
}

// [Generate]
// public partial class GenerationTest
// {
//     public void Test()
//     {
//         Console.WriteLine(Hallo);
//     }
// }