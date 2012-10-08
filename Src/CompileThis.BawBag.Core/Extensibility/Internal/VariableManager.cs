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
            Guard.NullParameter(session, () => session);

            _session = session;
        }

        public void AddVariable(string name, string value)
        {
            Guard.NullParameter(name, () => name);
            Guard.NullParameter(value, () => value);

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
            Guard.NullParameter(name, () => name);

            var variable = _session.Query<Variable>().SingleOrDefault(x => x.Name == name);

            return variable == null ? Enumerable.Empty<string>() : variable.Values;
        }

        public string GetRandomValue(string name)
        {
            Guard.NullParameter(name, () => name);

            return _session.Query<VariableValue, VariablesByName>()
                .Customize(x => x.RandomOrdering())
                .Where(x => x.Name == "Operators")
                .Select(x => x.Value)
                .FirstOrDefault();
        }
    }
}
