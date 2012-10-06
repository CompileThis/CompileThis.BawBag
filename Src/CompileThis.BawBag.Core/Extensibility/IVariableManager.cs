namespace CompileThis.BawBag.Extensibility
{
    using System.Collections.Generic;

    public interface IVariableManager
    {
        void AddVariable(string name, string value);
        IEnumerable<string> GetValues(string name);
        string GetRandomValue(string name);
    }
}