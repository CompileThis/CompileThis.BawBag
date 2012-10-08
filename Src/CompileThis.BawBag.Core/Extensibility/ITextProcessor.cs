namespace CompileThis.BawBag.Extensibility
{
    using System;

    public interface ITextProcessor
    {
        string SimplifyText(string text);
        bool SimplifiedEquals(string a, string b);
        bool SimplifiedEquals(string a, string b, StringComparison comparisonType);
    }
}

namespace CompileThis.BawBag.Extensibility.Internal
{
    using System;
    using System.Text.RegularExpressions;

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
    }



}
