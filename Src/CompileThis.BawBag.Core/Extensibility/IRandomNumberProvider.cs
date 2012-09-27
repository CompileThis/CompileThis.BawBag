namespace CompileThis.BawBag.Extensibility
{
    public interface IRandomNumberProvider
    {
        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
    }
}