namespace CompileThis.BawBag.Extensibility
{
    public interface ITextProcessor
    {
        string Simplify(string text);
    }
}

namespace CompileThis.BawBag.Extensibility.Internal
{
    internal class TextProcessor : ITextProcessor
    {
        public string Simplify(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
