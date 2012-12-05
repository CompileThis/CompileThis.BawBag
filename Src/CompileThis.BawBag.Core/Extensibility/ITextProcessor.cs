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
