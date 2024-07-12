using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Biz.Morsink.Results.Errors
{
    public class ErrorMessage : IErrorMessage
    {
        private readonly string _message;
        private readonly IReadOnlyDictionary<CultureInfo, string> _messages;

#if NET6_0_OR_GREATER
        public ErrorMessage(string defaultMessage, IReadOnlyDictionary<CultureInfo, string>? messages = null)
#else
        public ErrorMessage(string defaultMessage, IReadOnlyDictionary<CultureInfo, string> messages = null)
#endif
        {
            _message = defaultMessage;
            _messages = messages ?? ImmutableDictionary<CultureInfo, string>.Empty;
        }

        public string GetMessage(CultureInfo culture)
            => _messages.TryGetValue(culture, out var msg) ? msg : _message;

        public static implicit operator ErrorMessage(string defaultMessage)
            => new ErrorMessage(defaultMessage);
    }
}
