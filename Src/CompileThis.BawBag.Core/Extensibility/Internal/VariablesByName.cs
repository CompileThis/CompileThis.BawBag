namespace CompileThis.BawBag.Extensibility.Internal
{
    using System.Linq;

    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;

    internal class VariablesByName : AbstractIndexCreationTask<Variable, VariableValue>
    {
        public VariablesByName()
        {
            Map = variables => from variable in variables
                               from value in variable.Values
                               select new
                                          {
                                              variable.Name,
                                              Value = value
                                          };

            Store(x => x.Value, FieldStorage.Yes);
        }
    }
}