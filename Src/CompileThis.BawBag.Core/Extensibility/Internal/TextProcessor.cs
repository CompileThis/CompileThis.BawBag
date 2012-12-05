namespace CompileThis.BawBag.Extensibility.Internal
{
    using System;
    using System.Text.RegularExpressions;

    using CompileThis.Text;

    internal class TextProcessor : ITextProcessor
    {
        private static readonly Regex WhitespaceExpression = new Regex(@"\s+");
        private static readonly Regex SimplifyTextExpression = new Regex(@"[^\sa-zA-Z0-9']+");

        public string SimplifyText(string text)
        {
            return WhitespaceExpression.Replace(SimplifyTextExpression.Replace(text, string.Empty), " ").Trim();
        }

        public bool SimplifiedEquals(string a, string b)
        {
            return this.SimplifiedEquals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public bool SimplifiedEquals(string a, string b, StringComparison comparisonType)
        {
            return string.Equals(this.SimplifyText(a), this.SimplifyText(b), comparisonType);
        }

        public string FormatPluginResponse(string text, IPluginContext context)
        {
            return StringFormatter.Format(text, new PluginResponseValueProvider(context));
        }
    }
}