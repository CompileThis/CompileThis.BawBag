namespace CompileThis.BawBag.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Abstractions.Indexing;
    using Raven.Client;
    using Raven.Client.Indexes;

    public interface IVariableManager
    {
        void AddVariable(string name, string value);
        IEnumerable<string> GetValues(string name);
        string GetRandomValue(string name);
    }

    internal class VariableManager : IVariableManager
    {
        private readonly IDocumentSession _session;

        public VariableManager(IDocumentSession session)
        {
            _session = session;
        }

        public void AddVariable(string name, string value)
        {
            var variable = _session.Query<Variable>().SingleOrDefault(x => x.Name == name);
            if (variable == null)
            {
                variable = new Variable
                    {
                        Name = name,
                        Values = new List<string> { value }
                    };

                _session.Store(variable);
            }
            else
            {
                variable.Values.Add(value);
            }
        }

        public IEnumerable<string> GetValues(string name)
        {
            var variable = _session.Query<Variable>().SingleOrDefault(x => x.Name == name);

            return variable == null ? Enumerable.Empty<string>() : variable.Values;
        }

        public string GetRandomValue(string name)
        {
            return _session.Query<VariableValue, VariablesByName>()
                .Customize(x => x.RandomOrdering())
                .Where(x => x.Name == "Operators")
                .Select(x => x.Value)
                .FirstOrDefault();
        }
    }

    internal class Variable
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
    }

    internal class VariableValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    internal class VariablesByName : AbstractIndexCreationTask<Variable, VariableValue>
    {
        public VariablesByName()
        {
            Map = variables => from variable in variables
                               from value in variable.Values
                               select new { variable.Name, Value = value };

            Store(x => x.Value, FieldStorage.Yes);
        }
    }
}
