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
