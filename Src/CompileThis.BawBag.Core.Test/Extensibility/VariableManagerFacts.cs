namespace CompileThis.BawBag.Core.Extensibility
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Raven.Client.Indexes;

    using Xunit;

    using CompileThis.BawBag.Extensibility;
    using CompileThis.BawBag.Extensibility.Internal;

    public class VariableManagerFacts
    {
        [Fact]
        public void VariableManager_get_missing_variable_should_return_empty_enumerable()
        {
            var store = new Raven.Client.Embedded.EmbeddableDocumentStore
            {
                RunInMemory = true
            };

            store.Initialize();
            IndexCreation.CreateIndexes(typeof(VariablesByName).Assembly, store);

            using (var session = store.OpenSession())
            {
                var vm = new VariableManager(session);

                var x = vm.GetValues("Operators");

                x.Should().BeEmpty();
            }
        }

        [Fact]
        public void VariableManager_get_defined_variable_should_return_all_values()
        {
            var store = new Raven.Client.Embedded.EmbeddableDocumentStore
            {
                RunInMemory = true
            };

            store.Initialize();
            IndexCreation.CreateIndexes(typeof(VariablesByName).Assembly, store);

            using (var session = store.OpenSession())
            {
                session.Store(new Variable
                    {
                        Name = "Operators",
                        Values = new List<string> { "CompileThis", "Defize" }
                    });

                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var vm = new VariableManager(session);

                var x = vm.GetValues("Operators");

                x.Should().HaveCount(c => c == 2);
            }
        }

        [Fact]
        public void VariableManager_query_missing_variable_should_return_null()
        {
            var store = new Raven.Client.Embedded.EmbeddableDocumentStore
                {
                    RunInMemory = true
                };

            store.Initialize();
            IndexCreation.CreateIndexes(typeof(VariablesByName).Assembly, store);

            using (var session = store.OpenSession())
            {
                var vm = new VariableManager(session);

                var x = vm.GetRandomValue("Operators");

                x.Should().BeNull();
            }
        }
    }
}
