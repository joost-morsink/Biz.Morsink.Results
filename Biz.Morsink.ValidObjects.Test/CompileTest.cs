using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.IO.Pipelines;
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

public class TestAddress : IValidObject<TestAddress, TestAddress.Dto, TestAddress.Mutable.Dto>
{
    public static readonly IObjectValidator<TestAddress, Dto> Validator = ObjectValidator.For<TestAddress, Dto>();

    public static readonly IObjectValidator<TestAddress, Mutable.Dto> MutableValidator =
        ObjectValidator.For<TestAddress, Mutable.Dto>(a => a.GetMutableDto());

    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }

    private TestAddress(NonEmptyString street, NonEmptyString houseNumber, ZipCodeString zipCode, NonEmptyString city)
    {
        Street = street;
        HouseNumber = houseNumber;
        ZipCode = zipCode;
        City = city;
    }

    object IValidObject.GetDto() => GetDto();

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

    public class Dto : IDto<TestAddress, Dto>
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
    }


    public class Mutable : ValidationCell<TestAddress, Mutable.Dto>
    {
        public class Dto : IDto<TestAddress>
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

            public Dto GetDto()
                => new()
                {
                    Street = Street,
                    HouseNumber = HouseNumber,
                    ZipCode = ZipCode,
                    City = City
                };
        }

        public Mutable(TestAddress validObject) : base(ObjectValidator.For<TestAddress, Dto>(a => a.GetMutableDto()),
            validObject)
        {
        }

        public Mutable(Dto value) : base(ObjectValidator.For<TestAddress, Dto>(a => a.GetMutableDto()), value)
        {
        }

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
    }
}

public class TestPerson : IComplexValidObject<TestPerson, TestPerson.Intermediate, TestPerson.Dto,
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
        => new (FirstName, LastName, Age, MainAddress, Addresses);
        
    public Mutable.Dto GetMutableDto()
    {
        var res = new Mutable.Dto()
        {
            Cells = new()
            {
                FirstName = {ValidObject = FirstName},
                LastName = {ValidObject = LastName},
                Age = {ValidObject = Age},
            },
            MainAddress = {ValidObject = MainAddress}
        };
        res.Addresses.AddRange(Addresses.Select(a => new TestAddress.Mutable(a)));
        return res;
    }

    public class Dto : IComplexDto<TestPerson, Intermediate, Dto>
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
                    TestAddress.Validator.ToListValidator().TryCreate(Addresses).Prefix(nameof(Addresses)))
                .Apply((t, u, v, w, x)
                    => new Intermediate(t, u, v, w, x));

        public Result<TestPerson, ErrorList> TryCreate()
            => TryCreateIntermediate().SelectMany(x => x.TryCreate());
    }

    public record Intermediate(Valid<string, NotEmpty> FirstName, Valid<string, NotEmpty> LastName,
            Valid<int, MinValue<Zero>> Age, TestAddress MainAddress, ImmutableList<TestAddress> Addresses)
        : IIntermediateDto<TestPerson, Dto>
    {
        public Dto GetDto()
            => new Dto
            {
                FirstName = FirstName.GetDto(),
                LastName = LastName.GetDto(),
                Age = Age.GetDto(),
                MainAddress = MainAddress.GetDto(),
                Addresses = TestAddress.Validator.ToListValidator().GetDto(Addresses)
            };

        public Result<TestPerson, ErrorList> TryCreate()
        {
            var res = new TestPerson(FirstName, LastName, Age, MainAddress, Addresses);
            return res.Check().ToErrorList()
                .IfValidThen(() => res);
        }
    }

    public class Mutable
    {
        public class Dto : IDto<TestPerson>
        {
            public struct CellStruct
            {
                public CellStruct()
                {
                }

                public ValidationCell<NonEmptyString, string> FirstName { get; } = "".Constrain().With<NotEmpty>();
                public ValidationCell<NonEmptyString, string> LastName { get; } = "".Constrain().With<NotEmpty>();
                public ValidationCell<NaturalNumber, int> Age { get; } = 0.Constrain().With<MinValue<Zero>>();
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

            public TestAddress.Mutable MainAddress { get; } = new(new TestAddress.Mutable.Dto());

            public MutableList<TestAddress.Mutable> Addresses { get; } = new(ImmutableList<TestAddress.Mutable>.Empty);

            public Result<TestPerson, ErrorList> TryCreate()
                => (Cells.FirstName.AsResult().Prefix(nameof(FirstName)),
                        Cells.LastName.AsResult().Prefix(nameof(LastName)),
                        Cells.Age.AsResult().Prefix(nameof(Age)),
                        MainAddress.AsResult().Prefix(nameof(MainAddress)),
                        Addresses.Select(x => x.AsResult()).SequenceList().Prefix(nameof(Addresses)))
                    .Apply((firstName, lastName, age, mainAddress, addresses)
                        => new TestPerson(firstName, lastName, age, mainAddress, addresses));
        }
    }
}

[ValidObject(CellDtos = true)]
public partial class Address
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
    public NonEmptyString Country { get; }

    public class Mutable : AbstractMutable<Address, Dto>
    {
        public Mutable(Address address, Action<Dto, Dto>? updateDto = null) : base(address, updateDto)
        {
        }

        public Mutable(Dto value, Action<Dto, Dto>? updateDto = null) : base(value, updateDto)
        {
        }

        public string Street
        {
            get => Value.Street;
            set
            {
                Value = Value with {Street = value};
                OnPropertyChanged(nameof(Street));
            }
        }

        public string HouseNumber
        {
            get => Value.HouseNumber;
            set
            {
                Value = Value with {HouseNumber = value};
                OnPropertyChanged(nameof(HouseNumber));
            }
        }

        public string ZipCode
        {
            get => Value.ZipCode;
            set
            {
                Value = Value with {ZipCode = value};
                OnPropertyChanged(nameof(ZipCode));
            }
        }

        public string City
        {
            get => Value.City;
            set
            {
                Value = Value with {City = value};
                OnPropertyChanged(nameof(City));
            }
        }
    }
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

    public class Mutable : AbstractMutable<Person, Dto>
    {
        public Mutable(Person person) : base(person)
        {
        }

        public Mutable(Dto value) : base(value)
        {
        }

        public string FirstName
        {
            get => Value.FirstName;
            set
            {
                Value = Value with {FirstName = value};
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => Value.LastName;
            set
            {
                Value = Value with {LastName = value};
                OnPropertyChanged(nameof(LastName));
            }
        }

        public int Age
        {
            get => Value.Age;
            set
            {
                Value = Value with {Age = value};
                OnPropertyChanged(nameof(Age));
            }
        }

        private Address.Mutable? _mainAddress;

        public Address.Mutable MainAddress
        {
            get
            {
                if (_mainAddress == null)
                {
                    var vo = ValidObject;
                    _mainAddress = vo is not null
                        ? new Address.Mutable(vo.MainAddress, (_, dto) => Value = Value with {MainAddress = dto})
                        : new Address.Mutable(Value.MainAddress, (_, dto) => Value = Value with {MainAddress = dto});
                    _mainAddress.PropertyChanged += (s, e) => OnPropertyChanged(nameof(MainAddress));
                }

                return _mainAddress;
            }
        }

        private MutableList<Address.Mutable>? _addresses;

        public MutableList<Address.Mutable> Addresses
        {
            get
            {
                if (_addresses == null)
                {
                    _addresses =
                        ValidObject?.Addresses.Select((a, i) => new Address.Mutable(a,
                                (_, dto) => Value = Value with {Addresses = Value.Addresses.SetItem(i, dto)}))
                            .ToMutableList()
                        ?? Value.Addresses.Select((a, i) => new Address.Mutable(a,
                                (_, dto) => Value = Value with {Addresses = Value.Addresses.SetItem(i, dto)}))
                            .ToMutableList();
                    _addresses.CollectionChanged += AddressesOnCollectionChanged;
                }

                return _addresses;
            }
        }

        private MutableSet<string>? _tags;

        private void AddressesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ResetValidationCheck();
            OnPropertyChanged(nameof(Addresses));
        }

        public MutableSet<string> Tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags =
                        ValidObject?.Tags.Select(t => t.Value).ToMutableSet()
                        ?? Value.Tags.ToMutableSet();
                    _tags.CollectionChanged += TagsOnCollectionChanged;
                }

                return _tags;
            }
        }

        private void TagsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Tags));
        }
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
        var rnd = new Random();
    }
}
