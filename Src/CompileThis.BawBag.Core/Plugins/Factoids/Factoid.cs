namespace CompileThis.BawBag.Plugins.Factoids
{
    using System.Collections.Generic;

    internal class Factoid
    {
        public int Id { get; set; }
        public string Trigger { get; set; }

        public List<FactiodResponse> Responses { get; set; }
    }
}