namespace CompileThis.BawBag.Extensibility
{
    using System;

    public interface ITextProcessor
    {
        string FormatPluginResponse(string text, IPluginContext context);
        string SimplifyText(string text);
        bool SimplifiedEquals(string a, string b);
        bool SimplifiedEquals(string a, string b, StringComparison comparisonType);
    }
}

namespace CompileThis.BawBag.Extensibility.Internal
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CompileThis.Text;

    internal class TextProcessor : ITextProcessor
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");

        public string SimplifyText(string text)
        {
            return WhitespaceExpression.Replace(SimplifyTextExpression.Replace(text, ""), " ").Trim();
        }

        public bool SimplifiedEquals(string a, string b)
        {
            return SimplifiedEquals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public bool SimplifiedEquals(string a, string b, StringComparison comparisonType)
        {
            return string.Equals(SimplifyText(a), SimplifyText(b), comparisonType);
        }

        public string FormatPluginResponse(string text, IPluginContext context)
        {
            return StringFormatter.Format(text, new PluginResponseValueProvider(context));
        }
    }

    internal class PluginResponseValueProvider : IStringFormatterValueProvider
    {
        private readonly IPluginContext _context;

        public PluginResponseValueProvider(IPluginContext context)
        {
            _context = context;
        }

        public string ProvideValue(string token)
        {
            if (token.Equals("WHO", StringComparison.OrdinalIgnoreCase))
            {
                return "@" + _context.User.Name;
            }

            if (token.Equals("SOMEONE", StringComparison.OrdinalIgnoreCase))
            {
                var users = (from u in _context.Room.Users
                                     where u.Name != _context.BotName
                                     select u).ToList();

                var index = _context.RandomProvider.Next(users.Count);
                var userName = users[index].Name;

                return "@" + userName;
            }

            return "UNKNOWN";
        }
    }


}
