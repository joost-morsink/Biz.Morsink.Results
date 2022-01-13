namespace Biz.Morsink.Results.Errors;

public interface IErrorMessage
{
    string GetMessage(CultureInfo culture);
}
