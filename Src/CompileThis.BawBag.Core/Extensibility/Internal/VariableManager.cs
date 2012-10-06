namespace CompileThis.BawBag.Extensibility.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Client;

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
}
