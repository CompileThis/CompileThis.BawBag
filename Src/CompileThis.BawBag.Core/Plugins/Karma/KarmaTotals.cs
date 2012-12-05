namespace CompileThis.BawBag.Plugins.Karma
{
    using System.Linq;

    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;

    internal class KarmaTotals : AbstractIndexCreationTask<Karma, KarmaTotal>
    {
        public KarmaTotals()
        {
            this.Map = karmas => from karma in karmas select new { karma.Name, karma.Quantity };

            this.Reduce =
                results =>
                from result in results
                group result by result.Name into g
                select new
                        {
                            Name = g.Key,
                            Quantity = g.Sum(x => x.Quantity)
                        };

            this.Sort(x => x.Quantity, SortOptions.Int);
        }
    }
}