using System.Text.RegularExpressions;

namespace Biz.Morsink.ValidObjects.Constraints;

public abstract class RegexConstraint : IConstraint<string>
{
    private readonly Regex _regex;

    protected RegexConstraint(string regex)
    {
        _regex = new Regex(regex, RegexOptions.Compiled);
        _error = ConstraintErrors.Current.Regex(regex);
    }

    private readonly Error _error;

    protected virtual Error GetError()
        => _error;

    public Result<string, ErrorList> Check(string value)
    {
        if (_regex.IsMatch(value))
            return value;
        else
            return GetError().ToList();
    }
}

public abstract class RegexConstraint<R> : IConstraint<string, R>
{
    private Regex _regex;
    private Error _error;

    internal RegexConstraint(string regex) 
    {
        _regex = new Regex(regex, RegexOptions.Compiled);
        _error = ConstraintErrors.Current.Regex(regex);
        if (!ValidateRegex(_regex))
            throw new ArgumentOutOfRangeException(nameof(regex), "Regex should have at least one capture group.");
    }

    public Result<(string, R), ErrorList> Check(string value)
    {
        var match = _regex.Match(value);
        if (match.Success)
            return (value, ConstructResult(match));
        else
            return _error.ToList();
    }

    protected abstract bool ValidateRegex(Regex regex);
    protected abstract R ConstructResult(Match match);

    Result<string, ErrorList> IConstraint<string>.Check(string item)
        => Check(item).Select(t => t.Item1);
}

public class RegexConstraint1 : RegexConstraint<string>
{
    protected RegexConstraint1(string regex) : base(regex)
    {
    }

    protected override bool ValidateRegex(Regex regex)
        => regex.GetGroupNumbers().Length >= 2;

    protected override string ConstructResult(Match match)
        => match.Groups[1].Value;
}

public class RegexConstraint2 : RegexConstraint<(string, string)>
{
    protected RegexConstraint2(string regex) : base(regex)
    {
    }

    protected override bool ValidateRegex(Regex regex)
        => regex.GetGroupNumbers().Length >= 3;

    protected override (string, string) ConstructResult(Match match)
        => (match.Groups[1].Value, match.Groups[2].Value);
}
public class RegexConstraint3 : RegexConstraint<(string, string, string)>
{
    protected RegexConstraint3(string regex) : base(regex)
    {
    }

    protected override bool ValidateRegex(Regex regex)
        => regex.GetGroupNumbers().Length >= 4;

    protected override (string, string, string) ConstructResult(Match match)
        => (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
}
public class RegexConstraint4 : RegexConstraint<(string, string, string, string)>
{
    protected RegexConstraint4(string regex) : base(regex)
    {
    }

    protected override bool ValidateRegex(Regex regex)
        => regex.GetGroupNumbers().Length >= 5;

    protected override (string, string, string, string) ConstructResult(Match match)
        => (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value);
}
public class RegexConstraint5 : RegexConstraint<(string, string, string, string, string)>
{
    protected RegexConstraint5(string regex) : base(regex)
    {
    }

    protected override bool ValidateRegex(Regex regex)
        => regex.GetGroupNumbers().Length >= 6;

    protected override (string, string, string, string, string) ConstructResult(Match match)
        => (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value);
}

public class RegexConstraintList : RegexConstraint<ImmutableList<string>>
{
    protected RegexConstraintList(string regex): base(regex)
    {
    }

    protected override bool ValidateRegex(Regex regex)
        => true;

    protected override ImmutableList<string> ConstructResult(Match match)
        => match.Groups.OfType<Group>().Skip(1).Select(g => g.Value).ToImmutableList();
}