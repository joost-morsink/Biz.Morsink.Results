using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Biz.Morsink.ValidObjects.Generator;
using Biz.Morsink.ValidObjects.Mutable;

namespace Biz.Morsink.ValidObjects.Test;

using NonEmptyString = Valid<string, NotEmpty>;
using ZipCodeString = Valid<string, DutchZipCode>;
using NaturalNumber = Valid<int, MinValue<Zero>>;

public class DutchZipCode : RegexConstraint
{
    public static DutchZipCode Instance { get; } = new();

    public DutchZipCode() : base("^[1-9][0-9]{3}[A-Z]{2}$")
    {
    }
}

public class Identifier : RegexConstraint
{
    public static Identifier Instance { get; } = new();

    public Identifier() : base("^[A-Za-z_][A-Za-z_0-9]*")
    {
    }
}

public class TestAddress : IValidObjectWithToMutable<TestAddress, TestAddress.Dto, TestAddress.Mutable,
    TestAddress.Mutable.Dto>
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }

    public static class Validators
    {
        public static readonly IObjectValidator<TestAddress, Dto> Standard = ObjectValidator.For<TestAddress, Dto>();

        public static readonly IObjectValidator<TestAddress, Mutable.Dto> Mutable =
            ObjectValidator.ForMutableDto<TestAddress, Mutable, Mutable.Dto>();

        public static readonly IObjectValidator<ImmutableList<TestAddress>, ImmutableList<Dto>> List =
            Standard.ToListValidator();

        public static readonly IObjectValidator<IImmutableSet<TestAddress>, IImmutableSet<Dto>> Set =
            Standard.ToSetValidator();

        public static readonly IObjectValidator<ImmutableList<TestAddress>, ImmutableList<Mutable>> MutableList =
            ObjectValidator.MakeMutableListValidator<TestAddress, Dto, Mutable, Mutable.Dto>();
    }


    private TestAddress(NonEmptyString street, NonEmptyString houseNumber, ZipCodeString zipCode, NonEmptyString city)
    {
        Street = street;
        HouseNumber = houseNumber;
        ZipCode = zipCode;
        City = city;
    }

    object IValidObject.GetDto() => GetDto();

    public Mutable GetMutable() => new(this);

    public Mutable.Dto GetMutableDto() => new()
    {
        Cells = new()
        {
            Street = {ValidObject = Street},
            HouseNumber = {ValidObject = HouseNumber},
            ZipCode = {ValidObject = ZipCode},
            City = {ValidObject = City}
        }
    };

    public Dto GetDto() => new()
    {
        Street = Street.Value,
        HouseNumber = HouseNumber.Value,
        ZipCode = ZipCode.Value,
        City = City.Value
    };

    public class Dto : HasConstantStateVersion, IDto<TestAddress, Dto>, IToMutable<Mutable>
    {
        public string Street { get; init; } = "";
        public string HouseNumber { get; init; } = "";
        public string ZipCode { get; init; } = "";
        public string City { get; init; } = "";

        public Result<TestAddress, ErrorList> TryCreate()
            => (Street.Constrain().With<NotEmpty>().AsResult().Prefix(nameof(Street)),
                    HouseNumber.Constrain().With<NotEmpty>().AsResult().Prefix(nameof(HouseNumber)),
                    ZipCode.Constrain().With<DutchZipCode>().AsResult().Prefix(nameof(ZipCode)),
                    City.Constrain().With<NotEmpty>().AsResult().Prefix(nameof(City)))
                .Apply((street, houseNumber, zipCode, city)
                    => new TestAddress(street, houseNumber, zipCode, city));

        public Mutable GetMutable() => new()
        {
            Street = Street,
            HouseNumber = HouseNumber,
            ZipCode = ZipCode,
            City = City
        };
    }


    public class Mutable : ValidationCell<TestAddress, Mutable.Dto>, IDto<TestAddress>
    {
        public class Dto : IDto<TestAddress>, IToMutable<Mutable>, IHasStateVersion
        {
            public struct CellsStruct
            {
                public CellsStruct()
                {
                }

                public ValidationCell<NonEmptyString, string> Street { get; } = "".Constrain().With<NotEmpty>();
                public ValidationCell<NonEmptyString, string> HouseNumber { get; } = "".Constrain().With<NotEmpty>();
                public ValidationCell<ZipCodeString, string> ZipCode { get; } = "".Constrain().With<DutchZipCode>();
                public ValidationCell<NonEmptyString, string> City { get; } = "".Constrain().With<NotEmpty>();
            }

            public CellsStruct Cells { get; internal set; } = new();

            public string Street
            {
                get => Cells.Street.Value;
                set => Cells.Street.Value = value;
            }

            public string HouseNumber
            {
                get => Cells.HouseNumber.Value;
                set => Cells.HouseNumber.Value = value;
            }

            public string ZipCode
            {
                get => Cells.ZipCode.Value;
                set => Cells.ZipCode.Value = value;
            }

            public string City
            {
                get => Cells.City.Value;
                set => Cells.City.Value = value;
            }

            public Result<TestAddress, ErrorList> TryCreate()
                => (Cells.Street.AsResult().Prefix(nameof(Street)),
                        Cells.HouseNumber.AsResult().Prefix(nameof(HouseNumber)),
                        Cells.ZipCode.AsResult().Prefix(nameof(ZipCode)),
                        Cells.City.AsResult().Prefix(nameof(City)))
                    .Apply((street, houseNumber, zipCode, city)
                        => new TestAddress(street, houseNumber, zipCode, city));

            public TestAddress.Dto GetImmutable()
                => new()
                {
                    Street = Street,
                    HouseNumber = HouseNumber,
                    ZipCode = ZipCode,
                    City = City
                };

            public StateValue GetStateVersion()
                => StateVersion.Combine(Cells.Street,
                    Cells.HouseNumber,
                    Cells.ZipCode,
                    Cells.City);

            public Mutable GetMutable()
                => new(this);
        }

        public Mutable(TestAddress validObject) : base(Validators.Mutable, validObject)
        {
        }

        public Mutable(Dto value) : base(Validators.Mutable, value)
        {
        }

        public Mutable() : base(Validators.Mutable, new Dto())
        {
        }

        public override StateValue GetStateVersion() => Value.GetStateVersion();

        public string Street
        {
            get => Value.Street;
            set
            {
                if (!ReferenceEquals(Value.Street, value))
                {
                    Value.Street = value;
                    ResetValidationCheck();
                }
            }
        }

        public string HouseNumber
        {
            get => Value.HouseNumber;
            set
            {
                if (!ReferenceEquals(Value.HouseNumber, value))
                {
                    Value.HouseNumber = value;
                    ResetValidationCheck();
                }
            }
        }

        public string ZipCode
        {
            get => Value.ZipCode;
            set
            {
                if (!ReferenceEquals(Value.ZipCode, value))
                {
                    Value.ZipCode = value;
                    ResetValidationCheck();
                }
            }
        }

        public string City
        {
            get => Value.City;
            set
            {
                if (!ReferenceEquals(Value.City, value))
                {
                    Value.City = value;
                    ResetValidationCheck();
                }
            }
        }

        Result<TestAddress, ErrorList> IDto<TestAddress>.TryCreate()
            => AsResult();
    }
}

public class TestPerson : IComplexValidObjectWithToMutable<TestPerson, TestPerson.Intermediate, TestPerson.Dto,
    TestPerson.Mutable,
    TestPerson.Mutable.Dto>
{
    public NonEmptyString FirstName { get; }
    public NonEmptyString LastName { get; }
    public NaturalNumber Age { get; }
    public TestAddress MainAddress { get; }
    public ImmutableList<TestAddress> Addresses { get; }

    [ValidationMethod]
    private IEnumerable<string> Check()
    {
        if (Equals(FirstName, LastName))
            yield return "First and lastnames should be different.";
    }

    public static class Validators
    {
        public static readonly IComplexObjectValidator<TestPerson, Intermediate, Dto> Standard =
            ObjectValidator.For<TestPerson, Intermediate, Dto>();

        public static readonly IObjectValidator<ImmutableList<TestPerson>, ImmutableList<Dto>> List =
            Standard.ToListValidator();

        public static readonly IObjectValidator<IImmutableSet<TestPerson>, IImmutableSet<Dto>> Set =
            Standard.ToSetValidator();

        public static readonly IObjectValidator<TestPerson, Mutable.Dto> Mutable =
            ObjectValidator.ForMutableDto<TestPerson, Mutable, Mutable.Dto>();

        public static readonly IObjectValidator<ImmutableList<TestPerson>, ImmutableList<Mutable>> MutableList =
            ObjectValidator.MakeMutableListValidator<TestPerson, Dto, Mutable, Mutable.Dto>();
    }

    private TestPerson(NonEmptyString firstName, NonEmptyString lastName, NaturalNumber age, TestAddress mainAddress,
        ImmutableList<TestAddress> addresses)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        MainAddress = mainAddress;
        Addresses = addresses;
    }

    public Dto GetDto()
        => new Dto
        {
            FirstName = FirstName.GetDto(),
            LastName = LastName.GetDto(),
            Age = Age.GetDto(),
            MainAddress = MainAddress.GetDto(),
            Addresses = Addresses.Select(x => x.GetDto()).ToImmutableList()
        };

    public Intermediate GetIntermediate()
        => new(FirstName, LastName, Age, MainAddress, Addresses);

    public Mutable GetMutable() => new(this);

    public Mutable.Dto GetMutableDto()
    {
        var res = new Mutable.Dto()
        {
            Cells = new()
            {
                FirstName = {ValidObject = FirstName},
                LastName = {ValidObject = LastName},
                Age = {ValidObject = Age},
                MainAddress = {ValidObject = MainAddress}
            }
        };
        res.Addresses.AddRange(Addresses.Select(a => a.GetMutable()));
        return res;
    }

    public class Dto : IComplexDto<TestPerson, Intermediate, Dto>, IToMutable<Mutable>
    {
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public int Age { get; init; }
        public TestAddress.Dto MainAddress { get; init; } = new();
        public ImmutableList<TestAddress.Dto> Addresses { get; init; } = ImmutableList<TestAddress.Dto>.Empty;

        public Result<Intermediate, ErrorList> TryCreateIntermediate()
            =>
                (Valid<string, NotEmpty>.TryCreate(FirstName).Prefix(nameof(FirstName)),
                    Valid<string, NotEmpty>.TryCreate(LastName).Prefix(nameof(LastName)),
                    Valid<int, MinValue<Zero>>.TryCreate(Age).Prefix(nameof(Age)),
                    MainAddress.TryCreate().Prefix(nameof(MainAddress)),
                    TestAddress.Validators.List.TryCreate(Addresses).Prefix(nameof(Addresses)))
                .Apply((t, u, v, w, x)
                    => new Intermediate(t, u, v, w, x));

        public Result<TestPerson, ErrorList> TryCreate()
            => TryCreateIntermediate().SelectMany(x => x.TryCreate());

        public Mutable GetMutable() => new()
        {
            FirstName = FirstName,
            LastName = LastName,
            Age = Age,
            MainAddress = MainAddress.GetMutable(),
            Addresses = new(Addresses.Select(x => x.GetMutable()).ToImmutableList())
        };
    }

    public record Intermediate(Valid<string, NotEmpty> FirstName, Valid<string, NotEmpty> LastName,
            Valid<int, MinValue<Zero>> Age, TestAddress MainAddress, ImmutableList<TestAddress> Addresses)
        : IIntermediateDto<TestPerson, Dto>, IToMutable<Mutable>
    {
        public Dto GetDto()
            => new Dto
            {
                FirstName = FirstName.GetDto(),
                LastName = LastName.GetDto(),
                Age = Age.GetDto(),
                MainAddress = MainAddress.GetDto(),
                Addresses = TestAddress.Validators.List.GetDto(Addresses)
            };

        public Result<TestPerson, ErrorList> TryCreate()
        {
            var res = new TestPerson(FirstName, LastName, Age, MainAddress, Addresses);
            return res.Check().ToErrorList()
                .IfValidThen(() => res);
        }

        public Mutable GetMutable()
        {
            var res = new Mutable(new Mutable.Dto()
            {
                Cells = new()
                {
                    FirstName = {ValidObject = FirstName},
                    LastName = {ValidObject = LastName},
                    Age = {ValidObject = Age},
                    MainAddress = {ValidObject = MainAddress}
                }
            });
            res.Addresses.AddRange(Addresses.Select(x => x.GetMutable()));
            return res;
        }
    }

    public class Mutable : ValidationCell<TestPerson, Mutable.Dto>, IDto<TestPerson>
    {
        public class Dto : IDto<TestPerson>, IToMutable<Mutable>, IHasStateVersion
        {
            public struct CellStruct
            {
                public CellStruct()
                {
                }

                public ValidationCell<NonEmptyString, string> FirstName { get; } = "".Constrain().With<NotEmpty>();
                public ValidationCell<NonEmptyString, string> LastName { get; } = "".Constrain().With<NotEmpty>();
                public ValidationCell<NaturalNumber, int> Age { get; } = 0.Constrain().With<MinValue<Zero>>();
                public TestAddress.Mutable MainAddress { get; } = new();
                public MutableList<TestAddress.Mutable> Addresses { get; } = new();
            }

            public CellStruct Cells { get; internal set; } = new();

            public string FirstName
            {
                get => Cells.FirstName.Value;
                set => Cells.FirstName.Value = value;
            }

            public string LastName
            {
                get => Cells.LastName.Value;
                set => Cells.LastName.Value = value;
            }

            public int Age
            {
                get => Cells.Age.Value;
                set => Cells.Age.Value = value;
            }

            public TestAddress.Mutable MainAddress
            {
                get => Cells.MainAddress;
            }

            public MutableList<TestAddress.Mutable> Addresses
            {
                get => Cells.Addresses;
            }


            public Result<TestPerson, ErrorList> TryCreate()
                => (Cells.FirstName.AsResult().Prefix(nameof(FirstName)),
                        Cells.LastName.AsResult().Prefix(nameof(LastName)),
                        Cells.Age.AsResult().Prefix(nameof(Age)),
                        Cells.MainAddress.AsResult().Prefix(nameof(MainAddress)),
                        //Cells.Addresses.AsResult().Prefix(nameof(Addresses)))
                        TestAddress.Validators.MutableList.TryCreate(Cells.Addresses.ToImmutableList())
                            .Prefix(nameof(Addresses)))
                    .Apply((firstName, lastName, age, mainAddress, addresses)
                        => new TestPerson(firstName, lastName, age, mainAddress, addresses));

            public StateValue GetStateVersion()
                => StateVersion.Combine(Cells.FirstName,
                    Cells.LastName,
                    Cells.Age,
                    MainAddress,
                    Addresses);

            public Mutable GetMutable() => new(this);
        }

        public Mutable(TestPerson validObject) : base(Validators.Mutable, validObject)
        {
        }

        public Mutable(Dto value) : base(Validators.Mutable, value)
        {
        }

        public Mutable() : base(Validators.Mutable, new Dto())
        {
        }

        public override StateValue GetStateVersion()
            => Value.GetStateVersion();

        public string FirstName
        {
            get => Value.FirstName;
            set
            {
                if (!ReferenceEquals(Value.FirstName, value))
                {
                    Value.FirstName = value;
                    ResetValidationCheck();
                }
            }
        }

        public string LastName
        {
            get => Value.LastName;
            set
            {
                if (!ReferenceEquals(Value.LastName, value))
                {
                    Value.LastName = value;
                    ResetValidationCheck();
                }
            }
        }

        public int Age
        {
            get => Value.Age;
            set
            {
                if (Value.Age != value)
                {
                    Value.Age = value;
                    ResetValidationCheck();
                }
            }
        }

        public TestAddress.Mutable MainAddress
        {
            get => Value.MainAddress;
            set
            {
                if (!ReferenceEquals(Value.MainAddress, value))
                {
                    value.AsResult().Act(
                        vo => MainAddress.ValidObject = vo,
                        _ => MainAddress.Value = value.Value);
                    ResetValidationCheck();
                }
            }
        }

        public MutableList<TestAddress.Mutable> Addresses
        {
            get => Value.Addresses;
            set
            {
                if (!ReferenceEquals(Value.Addresses, value))
                {
                    Value.Addresses.Clear();
                    Value.Addresses.AddRange(value);
                    ResetValidationCheck();
                }
            }
        }

        Result<TestPerson, ErrorList> IDto<TestPerson>.TryCreate()
            => AsResult();
    }
}

[ValidObject]
public partial class Address
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
    public NonEmptyString Country { get; }
}

[ValidObject]
public partial class Person
{
    public NonEmptyString FirstName { get; }
    public NonEmptyString LastName { get; }
    public NaturalNumber Age { get; }
    public Address MainAddress { get; }
    public ImmutableList<Address> Addresses { get; }
    public NaturalNumber LuckyNumber { get; }
    public IImmutableSet<Valid<string, Identifier>> Tags { get; }

    [ValidationMethod]
    private IEnumerable<string> Check()
    {
        if (Equals(FirstName, LastName))
            yield return "First and lastnames should be different.";
    }
}

[ValidObject]
public partial class DictContainer
{
    public ImmutableDictionary<string, string> Regular { get; }
    public ImmutableSortedDictionary<Valid<string, Identifier>, Valid<string, NotEmpty>> Both { get; }
    public IImmutableDictionary<string, Valid<string, NotEmpty>> Value { get; }
    public ImmutableList<string> Strings { get; }
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
        => new(value);

    public static implicit operator Valid<int, MinValue<Zero>>(Natural value)
        => value._value;

    public static implicit operator int(Natural value)
        => value.Value;

    public static Natural operator +(Natural x, Natural y)
        => new(Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());

    public static Natural operator *(Natural x, Natural y)
        => new(Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());
}

public class CompileTest
{
    [Test]
    public void IntTest()
    {
        var p = new TestPerson.Mutable()
        {
            FirstName = "Joost",
            LastName = "Morsink",
            Age = 43,
            MainAddress =
            {
                Street = "Teststraat",
                HouseNumber = "1",
                ZipCode = "1234AB",
                City = "Teststad"
            },
            Addresses =
            {
                new TestAddress.Mutable()
                {
                    Street = "Teststraat",
                    HouseNumber = "1",
                    ZipCode = "1234AB",
                    City = "Teststad"
                }
            }
        };
        p.ValidObject.Should().NotBeNull();
        p.Errors.Should().HaveCount(0);
        p.Addresses[0].ValidObject.Should().Be(p.ValidObject!.Addresses[0]);
        ReferenceEquals(p.Addresses[0].ValidObject, p.ValidObject!.Addresses[0]).Should().BeTrue();
        
        p.Addresses[0].ZipCode = "12345";
        p.IsValid.Should().BeFalse();
        p.Errors.Should().Contain(e => e.Key.ToString() == "Addresses.0.ZipCode");
        p.MainAddress.IsValid.Should().BeTrue();
        p.Addresses[0].IsValid.Should().BeFalse();
    }

    [Test]
    public void AddressTest()
    {
        var p =
            new TestAddress.Mutable()
            {
                Street = "Teststraat",
                HouseNumber = "1",
                ZipCode = "1234AB",
                City = "Teststad"
            };
        var vo = p.ValidObject!;
        p.ValidObject.Should().NotBeNull();
        p.Errors.Should().HaveCount(0);
        
        p.ZipCode = "12345";
        p.IsValid.Should().BeFalse();
        p.Errors.Should().Contain(e => e.Key.ToString() == "ZipCode");

        p.ZipCode = "1234AB";
        p.ValidObject.Should().BeEquivalentTo(vo);
        p.ValidObject.Should().NotBe(vo);

    }
}